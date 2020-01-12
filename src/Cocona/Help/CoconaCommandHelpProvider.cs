using Cocona.Command;
using Cocona.Help.DocumentModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Cocona.Help
{
    public class CoconaCommandHelpProvider : ICoconaCommandHelpProvider
    {
        private readonly Func<string> _getExecutionSystemCommandName;

        public CoconaCommandHelpProvider(Func<string>? getExecutionSystemCommandName = null)
        {
            _getExecutionSystemCommandName = getExecutionSystemCommandName ?? (() => Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location));
        }

        public HelpMessage CreateCommandHelp(CommandDescriptor command)
        {
            var help = new HelpMessage();

            // Usage
            help.Children.Add(new HelpSection(new HelpHeading($"Usage: {_getExecutionSystemCommandName()}{(command.Options.Any() ? " [options...]" : "")}{(command.Arguments.Any() ? " arg0 ... argN" : "")}")));

            // Description
            if (!string.IsNullOrWhiteSpace(command.Description))
            {
                help.Children.Add(new HelpSection(new HelpParagraph(command.Description)));
            }

            // Arguments
            if (command.Arguments.Any())
            {
                help.Children.Add(new HelpSection(
                    new HelpHeading("Arguments:"),
                    new HelpSection(
                        new HelpLabelDescriptionList(
                            command.Arguments
                                .Select((x, i) =>
                                    new HelpLabelDescriptionListItem(
                                        $"{i}: {x.Name}",
                                        $"{x.Description}{(x.IsRequired ? " (Required)" : (" (DefaultValue=" + x.DefaultValue.Value + ")"))}"
                                    )
                                )
                                .ToArray()
                        )
                    )
                ));
            }

            // Options
            if (command.Options.Any())
            {
                help.Children.Add(new HelpSection(
                    new HelpHeading("Options:"),
                    new HelpSection(
                        new HelpLabelDescriptionList(
                            command.Options
                                .Select((x, i) =>
                                    new HelpLabelDescriptionListItem(
                                        $"--{x.Name}" + (x.ShortName.Any() ? ", " + string.Join(", ", x.ShortName.Select(x => $"-{x}")) : "") + (x.OptionType != typeof(bool) ? $" <{x.OptionType.Name}>" : ""),
                                        $"{x.Description}{(x.IsRequired ? " (Required)" : (" (DefaultValue=" + x.DefaultValue.Value + ")"))}"
                                    )
                                )
                                .ToArray()
                        )
                    )
                ));
            }

            return help;
        }
    }
}
