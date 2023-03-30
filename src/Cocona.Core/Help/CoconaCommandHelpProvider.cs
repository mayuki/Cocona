using Cocona.Application;
using Cocona.Command;
using Cocona.Command.BuiltIn;
using Cocona.Filters.Internal;
using Cocona.Help.DocumentModel;
using System.Text;
using Cocona.Localization.Internal;
using Cocona.Resources;

namespace Cocona.Help
{
    public class CoconaCommandHelpProvider : ICoconaCommandHelpProvider
    {
        private readonly ICoconaApplicationMetadataProvider _applicationMetadataProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly CoconaLocalizerWrapper _localizer;

        public CoconaCommandHelpProvider(ICoconaApplicationMetadataProvider applicationMetadataProvider, IServiceProvider serviceProvider)
        {
            _applicationMetadataProvider = applicationMetadataProvider;
            _serviceProvider = serviceProvider;
            _localizer = new CoconaLocalizerWrapper(_serviceProvider);
        }

        private string CreateUsageCommandOptionsAndArgs(CommandDescriptor command, IReadOnlyList<CommandDescriptor> subCommandStack)
        {
            var sb = new StringBuilder();
            sb.Append(_applicationMetadataProvider.GetExecutableName());

            if (subCommandStack.Count > 0)
            {
                sb.Append(" ");
                sb.Append(string.Join(" ", subCommandStack.Select(x => x.Name)));
            }

            if (!command.IsPrimaryCommand)
            {
                sb.Append(" ");
                sb.Append(command.Name);
            }

            if (command.Options.Any(x => !x.IsHidden))
            {
                foreach (var opt in command.Options.Where(x => !x.IsHidden))
                {
                    sb.Append(" ");
                    if (opt.UnwrappedOptionType == typeof(bool))
                    {
                        if (opt.DefaultValue.HasValue && opt.DefaultValue.Value != null && opt.DefaultValue.Value.Equals(true))
                        {
                            sb.Append($"[--{opt.Name}=<true|false>]");
                        }
                        else
                        {
                            sb.Append($"[--{opt.Name}]");
                        }
                    }
                    else if (opt.IsEnumerableLike)
                    {
                        sb.Append($"[--{opt.Name} <{opt.ValueName}>...]");
                    }
                    else
                    {
                        sb.Append($"[--{opt.Name} <{opt.ValueName}>]");
                    }
                }
            }

            if (command.OptionLikeCommands.Any(x => !x.IsHidden))
            {
                foreach (var opt in command.OptionLikeCommands.Where(x => !x.IsHidden))
                {
                    sb.Append(" ");
                    sb.Append($"[--{opt.Name}]");
                }
            }

            if (command.Arguments.Any())
            {
                foreach (var arg in command.Arguments)
                {
                    sb.Append(" ");
                    if (arg.IsEnumerableLike)
                    {
                        sb.Append($"{arg.Name}0 ... {arg.Name}N");
                    }
                    else
                    {
                        sb.Append(arg.Name);
                    }
                }
            }

            return sb.ToString();
        }

        public HelpMessage CreateCommandHelp(CommandDescriptor command, IReadOnlyList<CommandDescriptor> subCommandStack)
        {
            var help = new HelpMessage();

            // Usage
            help.Children.Add(new HelpSection(HelpSectionId.Usage, new HelpUsage(string.Format(Strings.Help_Index_Usage, CreateUsageCommandOptionsAndArgs(command, subCommandStack)))));

            // Description
            if (!string.IsNullOrWhiteSpace(command.Description))
            {
                help.Children.Add(new HelpSection(HelpSectionId.Description, new HelpDescription(_localizer.GetCommandDescription(command))));
            }

            // Arguments
            AddHelpForCommandArguments(help, command, command.Arguments);

            // Options
            AddHelpForCommandOptions(help, command, command.Options.OfType<ICommandOptionDescriptor>().Concat(command.OptionLikeCommands));

            // Transform help document
            var transformers = FilterHelper.GetFilters<ICoconaHelpTransformer>(command.Method, _serviceProvider);
            foreach (var transformer in transformers)
            {
                transformer.TransformHelp(help, command);
            }

            return help;
        }

        public HelpMessage CreateCommandsIndexHelp(CommandCollection commandCollection, IReadOnlyList<CommandDescriptor> subCommandStack)
        {
            var help = new HelpMessage();

            // Usage
            var usageSection = new HelpSection(HelpSectionId.Usage);
            var subCommandParams = (subCommandStack.Count > 0) ? string.Join(" ", subCommandStack.Select(x => x.Name)) + " " : "";
            if (commandCollection.All.Count != 1)
            {
                usageSection.Children.Add(new HelpUsage(string.Format(Strings.Help_Index_Usage_Multple, _applicationMetadataProvider.GetExecutableName(), subCommandParams)));
            }
            if (commandCollection.Primary != null && (commandCollection.All.Count == 1 || commandCollection.Primary.Options.Any() || commandCollection.Primary.Arguments.Any()))
            {
                usageSection.Children.Add(new HelpUsage(string.Format(Strings.Help_Index_Usage, CreateUsageCommandOptionsAndArgs(commandCollection.Primary, subCommandStack))));
            }
            help.Children.Add(usageSection);

            // Description
            var description = !string.IsNullOrWhiteSpace(commandCollection.Description)
                ? commandCollection.Description
                : !string.IsNullOrWhiteSpace(commandCollection.Primary?.Description)
                    ? _localizer.GetCommandDescription(commandCollection.Primary!)
                    : !string.IsNullOrWhiteSpace(_applicationMetadataProvider.GetDescription())
                        ? _applicationMetadataProvider.GetDescription()
                        : string.Empty;

            if (!string.IsNullOrWhiteSpace(description))
            {
                help.Children.Add(new HelpSection(HelpSectionId.Description, new HelpDescription(description!)));
            }

            // Commands
            var commandsExceptPrimary = commandCollection.All.Where(x => !x.IsPrimaryCommand && !x.IsHidden).ToArray();
            if (commandsExceptPrimary.Any())
            {
                help.Children.Add(new HelpSection(HelpSectionId.Commands,
                    new HelpHeading(Strings.Help_Heading_Commands),
                    new HelpSection(
                        new HelpLabelDescriptionList(
                            commandsExceptPrimary
                                .Select((x, i) =>
                                    new HelpLabelDescriptionListItem(x.Name, _localizer.GetCommandDescription(x))
                                )
                                .ToArray()
                        )
                    )
                ));
            }

            // Show helps for primary command.
            if (commandCollection.Primary != null)
            {
                // Arguments
                AddHelpForCommandArguments(help, commandCollection.Primary, commandCollection.Primary.Arguments);

                // Options
                AddHelpForCommandOptions(help, commandCollection.Primary, commandCollection.Primary.Options.OfType<ICommandOptionDescriptor>().Concat(commandCollection.Primary.OptionLikeCommands));
            }

            // Transform help document
            if (commandCollection.Primary != null)
            {
                var transformers = FilterHelper.GetFilters<ICoconaHelpTransformer>(commandCollection.Primary.Method, _serviceProvider);

                // TODO: This is ad-hoc workaround for default primary command.
                if (BuiltInPrimaryCommand.IsBuiltInCommand(commandCollection.Primary))
                {
                    transformers = commandCollection.All
                        .Select(x => x.CommandType)
                        .Distinct()
                        .SelectMany(x => FilterHelper.GetFilters<ICoconaHelpTransformer>(x, _serviceProvider))
                        .ToArray();
                }

                foreach (var transformer in transformers)
                {
                    transformer.TransformHelp(help, commandCollection.Primary);
                }
            }

            return help;
        }

        public HelpMessage CreateVersionHelp()
        {
            var prodName = _applicationMetadataProvider.GetProductName();
            var version = _applicationMetadataProvider.GetVersion();

            return new HelpMessage(new HelpSection(new HelpHeading($"{prodName} {version}")));
        }

        private void AddHelpForCommandArguments(HelpMessage help, CommandDescriptor command, IReadOnlyList<CommandArgumentDescriptor> arguments)
        {
            if (arguments.Any())
            {
                help.Children.Add(new HelpSection(HelpSectionId.Arguments,
                    new HelpHeading(Strings.Help_Heading_Arguments),
                    new HelpSection(
                        new HelpLabelDescriptionList(
                            arguments
                                .Select((x, i) =>
                                    new HelpLabelDescriptionListItem(
                                        $"{i}: {x.Name}",
                                        BuildParameterDescription(_localizer.GetArgumentDescription(command, x), x.IsRequired, x.UnwrappedArgumentType, x.DefaultValue)
                                    )
                                )
                                .ToArray()
                        )
                    )
                ));
            }
        }

        private void AddHelpForCommandOptions(HelpMessage help, CommandDescriptor command, IEnumerable<ICommandOptionDescriptor> options)
        {
            if (options.Any(x => !x.Flags.HasFlag(CommandOptionFlags.Hidden)))
            {
                help.Children.Add(new HelpSection(HelpSectionId.Options,
                    new HelpHeading(Strings.Help_Heading_Options),
                    new HelpSection(
                        new HelpLabelDescriptionList(
                            options
                                .Where(x => !x.Flags.HasFlag(CommandOptionFlags.Hidden))
                                .Select((x, i) =>
                                    x is CommandOptionDescriptor option
                                        ? new HelpLabelDescriptionListItem(
                                            BuildParameterLabel(option),
                                            BuildParameterDescription(_localizer.GetOptionDescription(command, x), option.IsRequired, option.UnwrappedOptionType, option.DefaultValue)
                                        )
                                        : x is CommandOptionLikeCommandDescriptor optionLikeCommand
                                            ? new HelpLabelDescriptionListItem(
                                                BuildParameterLabel(optionLikeCommand),
                                                _localizer.GetCommandDescription(optionLikeCommand.Command)
                                            )
                                            : throw new NotSupportedException()
                                )
                                .ToArray()
                        )
                    )
                ));
            }
        }

        private string BuildParameterLabel(CommandOptionDescriptor option)
        {
            return (option.ShortName.Any() ? string.Join(", ", option.ShortName.Select(x => $"-{x}")) + ", " : "") +
                $"--{option.Name}" +
                (
                    option.UnwrappedOptionType == typeof(bool)
                        ? option.DefaultValue.HasValue && option.DefaultValue.Value != null && option.DefaultValue.Value.Equals(true)
                            ? "=<true|false>"
                            : ""
                        : option.IsEnumerableLike
                            ? $" <{option.ValueName}>..."
                            : $" <{option.ValueName}>"
                );
        }


        private string BuildParameterLabel(CommandOptionLikeCommandDescriptor optionLikeCommand)
        {
            return (optionLikeCommand.ShortName.Any() ? string.Join(", ", optionLikeCommand.ShortName.Select(x => $"-{x}")) + ", " : "") +
                   $"--{optionLikeCommand.Name}";
        }

        private string BuildParameterDescription(string description, bool isRequired, Type valueType, CoconaDefaultValue defaultValue)
        {
            return 
                description +
                    (isRequired
                        ? string.Format(" ({0})", Strings.Help_Description_Required)
                        : (valueType == typeof(bool) && defaultValue.Value != null && defaultValue.Value.Equals(false))
                            ? ""
                            : (defaultValue.Value is null || (defaultValue.Value is string defaultValueStr && string.IsNullOrEmpty(defaultValueStr)))
                                ? ""
                                : (" " + string.Format("({0}: {1})", Strings.Help_Description_Default, defaultValue.Value))) +
                    (valueType.IsEnum
                        ? " " + string.Format("({0}: {1})", Strings.Help_Description_AllowedValues, string.Join(", ", Enum.GetNames(valueType)))
                        : "");
        }
    }
}
