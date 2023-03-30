using Cocona.Command;
using System.Diagnostics.CodeAnalysis;

namespace Cocona.CommandLine
{
    /// <summary>
    /// Cocona default implementation of command-line parser.
    /// </summary>
    public class CoconaCommandLineParser : ICoconaCommandLineParser
    {
        public bool TryGetCommandName(IReadOnlyList<string> args, [NotNullWhen(true)] out string? commandName)
        {
            if (args.Count == 0)
            {
                commandName = null;
                return false;
            }

            if (args[0].StartsWith("-"))
            {
                commandName = null;
                return false;
            }

            commandName = args[0];
            return true;
        }

        public ParsedCommandLine ParseCommand(IReadOnlyList<string> args, IReadOnlyList<ICommandOptionDescriptor> optionDescriptors, IReadOnlyList<CommandArgumentDescriptor> argumentDescriptors)
        {
            var optionByLongName = new Dictionary<string, ICommandOptionDescriptor>(optionDescriptors.Count);
            var optionByShortName = new Dictionary<char, ICommandOptionDescriptor>(optionDescriptors.Count);
            foreach (var option in optionDescriptors)
            {
                optionByLongName[option.Name] = option;
                foreach (var shortName in option.ShortName)
                {
                    optionByShortName[shortName] = option;
                }
            }

            var arguments = new List<CommandArgument>(10);
            var options = new List<CommandOption>(10);
            var unknownOptions = new List<string>();
            var optionsCompleted = false;

            // parse options and arguments
            var index = 0;
            for (var i = 0; i < args.Count; i++)
            {
                if (!args[i].StartsWith("-") || optionsCompleted)
                {
                    arguments.Add(new CommandArgument(args[i], i));
                    continue;
                }

                index++;

                if (args[i].StartsWith("--"))
                {
                    // end of options
                    if (args[i] == "--")
                    {
                        optionsCompleted = true;
                        continue;
                    }

                    // long-named
                    var equalPos = args[i].IndexOf('=');
                    if (equalPos > -1)
                    {
                        // --option=value
                        var partLeft = args[i].Substring(2, equalPos - 2);
                        var partRight = args[i].Substring(equalPos + 1);

                        if (optionByLongName.TryGetValue(partLeft, out var option))
                        {
                            if (option is CommandOptionDescriptor optionDesc)
                            {
                                // If the flag has StopParsingOptions, no further options will be parsed.
                                if (option.Flags.HasFlag(CommandOptionFlags.StopParsingOptions))
                                {
                                    optionsCompleted = true;
                                }

                                if (optionDesc.UnwrappedOptionType == typeof(bool))
                                {
                                    // Boolean (flag)
                                    var flag = string.Equals(partRight, "true", StringComparison.OrdinalIgnoreCase) || string.Equals(partRight, "1", StringComparison.OrdinalIgnoreCase)
                                        ? "true"
                                        : "false";
                                    options.Add(new CommandOption(optionDesc, flag, i));
                                }
                                else
                                {
                                    // Non-boolean (the option may have some value)
                                    options.Add(new CommandOption(optionDesc, partRight, i));
                                }
                            }
                            else if (option is CommandOptionLikeCommandDescriptor optionLikeCommand)
                            {
                                // OptionLikeCommand should be terminate process here.
                                options.Add(new CommandOption(optionLikeCommand, null, i));
                                break;
                            }
                            else
                            {
                                throw new NotImplementedException();
                            }

                            continue;
                        }
                    }
                    else
                    {
                        // --option
                        // --option value
                        if (optionByLongName.TryGetValue(args[i].Substring(2), out var option))
                        {
                            if (option is CommandOptionDescriptor optionDesc)
                            {
                                // If the flag has StopParsingOptions, no further options will be parsed.
                                if (option.Flags.HasFlag(CommandOptionFlags.StopParsingOptions))
                                {
                                    optionsCompleted = true;
                                }

                                if (optionDesc.UnwrappedOptionType == typeof(bool))
                                {
                                    // Boolean (flag)
                                    options.Add(new CommandOption(optionDesc, "true", i));
                                }
                                else
                                {
                                    // Non-boolean (the option may have some value)
                                    options.Add(new CommandOption(optionDesc, (i + 1 == args.Count) ? null : args[++i], i)); // consume a next argument
                                    index++;
                                }
                            }
                            else if (option is CommandOptionLikeCommandDescriptor optionLikeCommand)
                            {
                                // OptionLikeCommand should be terminate process here.
                                options.Add(new CommandOption(optionLikeCommand, null, i));
                                break;
                            }
                            else
                            {
                                throw new NotImplementedException();
                            }

                            continue;
                        }
                    }

                    unknownOptions.Add(args[i].Substring(2).ToString());
                }
                else if (args[i].Length == 1)
                {
                    // '-' hyphen
                    arguments.Add(new CommandArgument("-", i));
                }
                else
                {
                    // short-named
                    for (var j = 1; j < args[i].Length; j++)
                    {
                        if (optionByShortName.TryGetValue(args[i][j], out var option))
                        {
                            if (option is CommandOptionDescriptor optionDesc)
                            {
                                // If the flag has StopParsingOptions, no further options will be parsed.
                                if (option.Flags.HasFlag(CommandOptionFlags.StopParsingOptions))
                                {
                                    optionsCompleted = true;
                                }

                                if (optionDesc.UnwrappedOptionType == typeof(bool))
                                {
                                    // Boolean (flag)
                                    options.Add(new CommandOption(optionDesc, "true", i));
                                }
                                else
                                {
                                    // Non-boolean (the option may have some value)
                                    if (args.Count == i + 1 && args[i].Length == j + 1)
                                    {
                                        // ["-foo", "-I"]
                                        // -foo -I
                                        // ------^
                                        options.Add(new CommandOption(optionDesc, null, i));
                                    }
                                    else if (args[i].Length - 1 == j)
                                    {
                                        // ["-foo", "-I", "../path/", "-fgh", "-i", "-j"]
                                        // -foo -I ../path/ -fgh -i -j
                                        // ------^^
                                        options.Add(new CommandOption(optionDesc, args[++i], i));
                                        index++;
                                    }
                                    else if (args[i][j + 1] == '=')
                                    {
                                        // ["-foo", "-I=../path/", "-fgh", "-i", "-j"]
                                        // -foo -I=../path/ -fgh -i -j
                                        // ------^^
                                        options.Add(new CommandOption(optionDesc, args[i].Substring(j + 2), i));
                                    }
                                    else
                                    {
                                        // ["-foo", "-I../path/", "-fgh", "-i", "-j"]
                                        // -foo -I../path/ -fgh -i -j
                                        // ------^
                                        options.Add(new CommandOption(optionDesc, args[i].Substring(j + 1), i));
                                    }

                                    break;
                                }
                            }
                            else if (option is CommandOptionLikeCommandDescriptor optionLikeCommand)
                            {
                                // OptionLikeCommand should be terminate process here.
                                options.Add(new CommandOption(optionLikeCommand, null, i));
                                break;
                            }
                            else
                            {
                                throw new NotImplementedException();
                            }
                        }
                        else
                        {
                            unknownOptions.Add(args[i][j].ToString());
                        }
                    }
                }
            }

            return new ParsedCommandLine(options, arguments, unknownOptions);
        }
    }
}
