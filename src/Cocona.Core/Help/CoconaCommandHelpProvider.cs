using Cocona.Application;
using Cocona.Command;
using Cocona.Command.BuiltIn;
using Cocona.Filters.Internal;
using Cocona.Help.DocumentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cocona.Internal;

namespace Cocona.Help
{
    public class CoconaCommandHelpProvider : ICoconaCommandHelpProvider
    {
        private readonly ICoconaApplicationMetadataProvider _applicationMetadataProvider;
        private readonly IServiceProvider _serviceProvider;

        public CoconaCommandHelpProvider(ICoconaApplicationMetadataProvider applicationMetadataProvider, IServiceProvider serviceProvider)
        {
            _applicationMetadataProvider = applicationMetadataProvider;
            _serviceProvider = serviceProvider;
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
                    if (opt.OptionType == typeof(bool))
                    {
                        if (opt.DefaultValue.HasValue && opt.DefaultValue.Value.Equals(true))
                        {
                            sb.Append($"[--{opt.Name}=<true|false>]");
                        }
                        else
                        {
                            sb.Append($"[--{opt.Name}]");
                        }
                    }
                    else if (DynamicListHelper.IsArrayOrEnumerableLike(opt.OptionType))
                    {
                        sb.Append($"[--{opt.Name} <{opt.ValueName}>...]");
                    }
                    else
                    {
                        sb.Append($"[--{opt.Name} <{opt.ValueName}>]");
                    }
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
            help.Children.Add(new HelpSection(HelpSectionId.Usage, new HelpUsage($"Usage: {CreateUsageCommandOptionsAndArgs(command, subCommandStack)}")));

            // Description
            if (!string.IsNullOrWhiteSpace(command.Description))
            {
                help.Children.Add(new HelpSection(HelpSectionId.Description, new HelpDescription(command.Description)));
            }

            // Arguments
            AddHelpForCommandArguments(help, command.Arguments);

            // Options
            AddHelpForCommandOptions(help, command.Options);

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
                usageSection.Children.Add(new HelpUsage($"Usage: {_applicationMetadataProvider.GetExecutableName()} {subCommandParams}[command]"));
            }
            if (commandCollection.Primary != null && (commandCollection.All.Count == 1 || commandCollection.Primary.Options.Any() || commandCollection.Primary.Arguments.Any()))
            {
                usageSection.Children.Add(new HelpUsage($"Usage: {CreateUsageCommandOptionsAndArgs(commandCollection.Primary, subCommandStack)}"));
            }
            help.Children.Add(usageSection);

            // Description
            var description = !string.IsNullOrWhiteSpace(commandCollection.Description)
                ? commandCollection.Description
                : !string.IsNullOrWhiteSpace(commandCollection.Primary?.Description)
                    ? commandCollection.Primary?.Description
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
                    new HelpHeading("Commands:"),
                    new HelpSection(
                        new HelpLabelDescriptionList(
                            commandsExceptPrimary
                                .Select((x, i) =>
                                    new HelpLabelDescriptionListItem(x.Name, x.Description)
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
                AddHelpForCommandArguments(help, commandCollection.Primary.Arguments);

                // Options
                AddHelpForCommandOptions(help, commandCollection.Primary.Options);
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

        private void AddHelpForCommandArguments(HelpMessage help, IEnumerable<CommandArgumentDescriptor> arguments)
        {
            if (arguments.Any())
            {
                help.Children.Add(new HelpSection(HelpSectionId.Arguments,
                    new HelpHeading("Arguments:"),
                    new HelpSection(
                        new HelpLabelDescriptionList(
                            arguments
                                .Select((x, i) =>
                                    new HelpLabelDescriptionListItem(
                                        $"{i}: {x.Name}",
                                        BuildParameterDescription(x.Description, x.IsRequired, x.ArgumentType, x.DefaultValue)
                                    )
                                )
                                .ToArray()
                        )
                    )
                ));
            }
        }

        private void AddHelpForCommandOptions(HelpMessage help, IEnumerable<CommandOptionDescriptor> options)
        {
            if (options.Any(x => !x.IsHidden))
            {
                help.Children.Add(new HelpSection(HelpSectionId.Options,
                    new HelpHeading("Options:"),
                    new HelpSection(
                        new HelpLabelDescriptionList(
                            options
                                .Where(x => !x.IsHidden)
                                .Select((x, i) =>
                                    new HelpLabelDescriptionListItem(
                                        BuildParameterLabel(x),
                                        BuildParameterDescription(x.Description, x.IsRequired, x.OptionType, x.DefaultValue)
                                    )
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
                    option.OptionType == typeof(bool)
                        ? option.DefaultValue.HasValue && option.DefaultValue.Value.Equals(true)
                            ? "=<true|false>"
                            : ""
                        : DynamicListHelper.IsArrayOrEnumerableLike(option.OptionType)
                            ? $" <{option.ValueName}>..."
                            : $" <{option.ValueName}>"
                );
        }

        private string BuildParameterDescription(string description, bool isRequired, Type valueType, CoconaDefaultValue defaultValue)
        {
            return 
                description +
                    (isRequired
                        ? " (Required)"
                        : (valueType == typeof(bool) && defaultValue.Value.Equals(false))
                            ? ""
                            : (" (Default: " + defaultValue.Value + ")")) +
                    (valueType.IsEnum
                        ? " (Allowed values: " + string.Join(", ", Enum.GetNames(valueType)) + ")"
                        : "");
        }
    }
}
