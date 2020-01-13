using Cocona.Command;
using Cocona.CommandLine;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Cocona.Test.CommandLine
{
    public class CoconaCommandLineParserTest
    {
        private CommandOptionDescriptor CreateCommandOption(Type optionType, string name, IReadOnlyList<char> shortName, string description, CoconaDefaultValue defaultValue)
        {
            return new CommandOptionDescriptor(optionType, name, shortName, description, defaultValue, null);
        }

        [Fact]
        public void TryGetCommandName_Empty()
        {
            var args = new string[] { };
            var result = new CoconaCommandLineParser().TryGetCommandName(args, out var commandName);
            result.Should().BeFalse();
            commandName.Should().BeNull();
        }

        [Fact]
        public void TryGetCommandName_CommandName()
        {
            var args = new string[] { "commandName" };
            var result = new CoconaCommandLineParser().TryGetCommandName(args, out var commandName);
            result.Should().BeTrue();
            commandName.Should().Be("commandName");
        }

        [Fact]
        public void TryGetCommandName_ShortOption()
        {
            var args = new string[] { "-h" };
            var result = new CoconaCommandLineParser().TryGetCommandName(args, out var commandName);
            result.Should().BeFalse();
            commandName.Should().BeNull();
        }

        [Fact]
        public void TryGetCommandName_LongOption()
        {
            var args = new string[] { "--h" };
            var result = new CoconaCommandLineParser().TryGetCommandName(args, out var commandName);
            result.Should().BeFalse();
            commandName.Should().BeNull();
        }

        [Fact]
        public void ParseCommand_ArgumentsOnly()
        {
            var args = new[] { "src1", "src2", "src3", "dest" };
            var parsed = new CoconaCommandLineParser().ParseCommand(
                args,
                new CommandOptionDescriptor[] { },
                new CommandArgumentDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string[]), "src", 0, "", CoconaDefaultValue.None),
                    new CommandArgumentDescriptor(typeof(string), "dest", 0, "", CoconaDefaultValue.None),
                }
            );
            parsed.Should().NotBeNull();
            parsed.Options.Should().BeEmpty();
            parsed.Arguments.Should().HaveCount(4);
            parsed.UnknownOptions.Should().BeEmpty();
        }

        [Fact]
        public void ParseCommand_ArgumentsOnly_NonRequiredOptions()
        {
            var args = new[] { "src1", "src2", "src3", "dest" };
            var parsed = new CoconaCommandLineParser().ParseCommand(
                args,
                new CommandOptionDescriptor[]
                {
                    CreateCommandOption(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                    CreateCommandOption(typeof(bool), "force", new [] { 'f' }, "", new CoconaDefaultValue(false)),
                },
                new CommandArgumentDescriptor[]
                {
                        new CommandArgumentDescriptor(typeof(string[]), "src", 0, "", CoconaDefaultValue.None),
                        new CommandArgumentDescriptor(typeof(string), "dest", 0, "", CoconaDefaultValue.None),
                }
            );
            parsed.Should().NotBeNull();
            parsed.Options.Should().BeEmpty();
            parsed.Arguments.Should().HaveCount(4);
            parsed.UnknownOptions.Should().BeEmpty();
        }

        [Fact]
        public void ParseCommand_LongOption_Value()
        {
            var args = new[] { "--message", "Hello" };
            var parsed = new CoconaCommandLineParser().ParseCommand(
                args,
                new CommandOptionDescriptor[]
                {
                    CreateCommandOption(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                    CreateCommandOption(typeof(string), "message", new [] { 'm' }, "", new CoconaDefaultValue(string.Empty)),
                },
                new CommandArgumentDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string[]), "src", 0, "", CoconaDefaultValue.None),
                    new CommandArgumentDescriptor(typeof(string), "dest", 0, "", CoconaDefaultValue.None),
                }
            );
            parsed.Should().NotBeNull();
            parsed.Options.Should().HaveCount(1);
            parsed.Options[0].Value.Should().Be("Hello");
            parsed.Arguments.Should().BeEmpty();
            parsed.UnknownOptions.Should().BeEmpty();
        }

        [Fact]
        public void ParseCommand_LongOption_EndOfOptions()
        {
            var args = new[] { "--recursive","src1", "src2", "src3", "dest" };
            var parsed = new CoconaCommandLineParser().ParseCommand(
                args,
                new CommandOptionDescriptor[]
                {
                    CreateCommandOption(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                },
                new CommandArgumentDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string[]), "src", 0, "", CoconaDefaultValue.None),
                    new CommandArgumentDescriptor(typeof(string), "dest", 0, "", CoconaDefaultValue.None),
                }
            );
            parsed.Should().NotBeNull();
            parsed.Options.Should().HaveCount(1);
            parsed.Arguments.Should().HaveCount(4);
            parsed.Arguments[0].Value.Should().Be("src1");
            parsed.Arguments[1].Value.Should().Be("src2");
            parsed.Arguments[2].Value.Should().Be("src3");
            parsed.Arguments[3].Value.Should().Be("dest");
            parsed.UnknownOptions.Should().BeEmpty();
        }
        [Fact]
        public void ParseCommand_LongOption_With_EndOfOptions()
        {
            var args = new[] { "--recursive", "--", "src1", "src2", "src3", "dest" };
            var parsed = new CoconaCommandLineParser().ParseCommand(
                args,
                new CommandOptionDescriptor[]
                {
                        CreateCommandOption(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                },
                new CommandArgumentDescriptor[]
                {
                        new CommandArgumentDescriptor(typeof(string[]), "src", 0, "", CoconaDefaultValue.None),
                        new CommandArgumentDescriptor(typeof(string), "dest", 0, "", CoconaDefaultValue.None),
                }
            );
            parsed.Should().NotBeNull();
            parsed.Options.Should().HaveCount(1);
            parsed.Arguments.Should().HaveCount(4);
            parsed.Arguments[0].Value.Should().Be("src1");
            parsed.Arguments[1].Value.Should().Be("src2");
            parsed.Arguments[2].Value.Should().Be("src3");
            parsed.Arguments[3].Value.Should().Be("dest");
            parsed.UnknownOptions.Should().BeEmpty();
        }

        [Fact]
        public void ParseCommand_LongOptionEqualValue()
        {
            var args = new[] { "--recursive", "--message=Message", "--", "src1", "src2", "src3", "dest" };
            var parsed = new CoconaCommandLineParser().ParseCommand(
                args,
                new CommandOptionDescriptor[]
                {
                    CreateCommandOption(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                    CreateCommandOption(typeof(bool), "force", new [] { 'f' }, "", new CoconaDefaultValue(false)),
                    CreateCommandOption(typeof(string), "message", new [] { 'm' }, "", new CoconaDefaultValue(string.Empty)),
                },
                new CommandArgumentDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string[]), "src", 0, "", CoconaDefaultValue.None),
                    new CommandArgumentDescriptor(typeof(string), "dest", 0, "", CoconaDefaultValue.None),
                }
            );
            parsed.Should().NotBeNull();
            parsed.Options.Should().HaveCount(2);
            parsed.Arguments.Should().HaveCount(4);
            parsed.Arguments[0].Value.Should().Be("src1");
            parsed.Arguments[1].Value.Should().Be("src2");
            parsed.Arguments[2].Value.Should().Be("src3");
            parsed.Arguments[3].Value.Should().Be("dest");
            parsed.UnknownOptions.Should().BeEmpty();
        }

        [Fact]
        public void ParseCommand_LongOptionEqualValue_Empty()
        {
            var args = new[] { "--recursive", "--message=", "--", "src1", "src2", "src3", "dest" };
            var parsed = new CoconaCommandLineParser().ParseCommand(
                args,
                new CommandOptionDescriptor[]
                {
                    CreateCommandOption(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                    CreateCommandOption(typeof(bool), "force", new [] { 'f' }, "", new CoconaDefaultValue(false)),
                    CreateCommandOption(typeof(string), "message", new [] { 'm' }, "", new CoconaDefaultValue(string.Empty)),
                },
                new CommandArgumentDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string[]), "src", 0, "", CoconaDefaultValue.None),
                    new CommandArgumentDescriptor(typeof(string), "dest", 0, "", CoconaDefaultValue.None),
                }
            );
            parsed.Should().NotBeNull();
            parsed.Options.Should().HaveCount(2);
            parsed.Arguments.Should().HaveCount(4);
            parsed.Arguments[0].Value.Should().Be("src1");
            parsed.Arguments[1].Value.Should().Be("src2");
            parsed.Arguments[2].Value.Should().Be("src3");
            parsed.Arguments[3].Value.Should().Be("dest");
            parsed.UnknownOptions.Should().BeEmpty();
        }

        [Fact]
        public void ParseCommand_EndOfOptions_AtFirst()
        {
            var args = new[] { "--", "src1", "src2", "src3", "dest" };
            var parsed = new CoconaCommandLineParser().ParseCommand(
                args,
                new CommandOptionDescriptor[] { },
                new CommandArgumentDescriptor[]
                {
                        new CommandArgumentDescriptor(typeof(string[]), "src", 0, "", CoconaDefaultValue.None),
                        new CommandArgumentDescriptor(typeof(string), "dest", 0, "", CoconaDefaultValue.None),
                }
            );
            parsed.Should().NotBeNull();
            parsed.Options.Should().BeEmpty();
            parsed.Arguments.Should().HaveCount(4);
            parsed.Arguments[0].Value.Should().Be("src1");
            parsed.Arguments[1].Value.Should().Be("src2");
            parsed.Arguments[2].Value.Should().Be("src3");
            parsed.Arguments[3].Value.Should().Be("dest");
            parsed.UnknownOptions.Should().BeEmpty();
        }


        [Fact]
        public void ParseCommand_EndOfOptions_AtLast()
        {
            var args = new[] { "src1", "src2", "src3", "dest", "--" };
            var parsed = new CoconaCommandLineParser().ParseCommand(
                args,
                new CommandOptionDescriptor[] { },
                new CommandArgumentDescriptor[]
                {
                        new CommandArgumentDescriptor(typeof(string[]), "src", 0, "", CoconaDefaultValue.None),
                        new CommandArgumentDescriptor(typeof(string), "dest", 0, "", CoconaDefaultValue.None),
                }
            );
            parsed.Should().NotBeNull();
            parsed.Options.Should().BeEmpty();
            parsed.Arguments.Should().HaveCount(5);
            parsed.Arguments[0].Value.Should().Be("src1");
            parsed.Arguments[1].Value.Should().Be("src2");
            parsed.Arguments[2].Value.Should().Be("src3");
            parsed.Arguments[3].Value.Should().Be("dest");
            parsed.Arguments[4].Value.Should().Be("--");
            parsed.UnknownOptions.Should().BeEmpty();
        }

        [Fact]
        public void ParseCommand_LongOption_ShortOption_Arguments()
        {
            var args = new[] { "--recursive", "-f", "src1", "src2", "src3", "dest" };
            var parsed = new CoconaCommandLineParser().ParseCommand(
                args,
                new CommandOptionDescriptor[]
                {
                        CreateCommandOption(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                        CreateCommandOption(typeof(bool), "force", new [] { 'f' }, "", new CoconaDefaultValue(false)),
                },
                new CommandArgumentDescriptor[]
                {
                        new CommandArgumentDescriptor(typeof(string[]), "src", 0, "", CoconaDefaultValue.None),
                        new CommandArgumentDescriptor(typeof(string), "dest", 0, "", CoconaDefaultValue.None),
                }
            );
            parsed.Should().NotBeNull();
            parsed.Options.Should().HaveCount(2);
            parsed.Arguments.Should().HaveCount(4);
            parsed.Arguments[0].Value.Should().Be("src1");
            parsed.Arguments[1].Value.Should().Be("src2");
            parsed.Arguments[2].Value.Should().Be("src3");
            parsed.Arguments[3].Value.Should().Be("dest");
            parsed.UnknownOptions.Should().BeEmpty();
        }
        
        [Fact]
        public void ParseCommand_LongOption_ShortOption_Arguments_2()
        {
            var args = new[] { "--recursive", "-rf", "src1", "src2", "src3", "dest" };
            var parsed = new CoconaCommandLineParser().ParseCommand(
                args,
                new CommandOptionDescriptor[]
                {
                        CreateCommandOption(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                        CreateCommandOption(typeof(bool), "force", new [] { 'f' }, "", new CoconaDefaultValue(false)),
                },
                new CommandArgumentDescriptor[]
                {
                        new CommandArgumentDescriptor(typeof(string[]), "src", 0, "", CoconaDefaultValue.None),
                        new CommandArgumentDescriptor(typeof(string), "dest", 0, "", CoconaDefaultValue.None),
                }
            );
            parsed.Should().NotBeNull();
            parsed.Options.Should().HaveCount(3);
            parsed.Arguments.Should().HaveCount(4);
            parsed.Arguments[0].Value.Should().Be("src1");
            parsed.Arguments[1].Value.Should().Be("src2");
            parsed.Arguments[2].Value.Should().Be("src3");
            parsed.Arguments[3].Value.Should().Be("dest");
            parsed.UnknownOptions.Should().BeEmpty();
        }

        [Fact]
        public void ParseCommand_LongOption_ShortOption_EndOfOptions_Arguments()
        {
            var args = new[] { "--recursive", "-rf", "--", "src1", "src2", "src3", "dest" };
            var parsed = new CoconaCommandLineParser().ParseCommand(
                args,
                new CommandOptionDescriptor[]
                {
                        CreateCommandOption(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                        CreateCommandOption(typeof(bool), "force", new [] { 'f' }, "", new CoconaDefaultValue(false)),
                },
                new CommandArgumentDescriptor[]
                {
                        new CommandArgumentDescriptor(typeof(string[]), "src", 0, "", CoconaDefaultValue.None),
                        new CommandArgumentDescriptor(typeof(string), "dest", 0, "", CoconaDefaultValue.None),
                }
            );
            parsed.Should().NotBeNull();
            parsed.Options.Should().HaveCount(3);
            parsed.Arguments.Should().HaveCount(4);
            parsed.Arguments[0].Value.Should().Be("src1");
            parsed.Arguments[1].Value.Should().Be("src2");
            parsed.Arguments[2].Value.Should().Be("src3");
            parsed.Arguments[3].Value.Should().Be("dest");
            parsed.UnknownOptions.Should().BeEmpty();
        }

        [Fact]
        public void ParseCommand_LongOption_ShortOptionWithValue_EndOfOptions_Arguments()
        {
            var args = new[] { "--recursive", "-m", "Message", "--", "src1", "src2", "src3", "dest" };
            var parsed = new CoconaCommandLineParser().ParseCommand(
                args,
                new CommandOptionDescriptor[]
                {
                        CreateCommandOption(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                        CreateCommandOption(typeof(bool), "force", new [] { 'f' }, "", new CoconaDefaultValue(false)),
                        CreateCommandOption(typeof(string), "message", new [] { 'm' }, "", new CoconaDefaultValue(string.Empty)),
                },
                new CommandArgumentDescriptor[]
                {
                        new CommandArgumentDescriptor(typeof(string[]), "src", 0, "", CoconaDefaultValue.None),
                        new CommandArgumentDescriptor(typeof(string), "dest", 0, "", CoconaDefaultValue.None),
                }
            );
            parsed.Should().NotBeNull();
            parsed.Options.Should().HaveCount(2);
            parsed.Options[0].Value.Should().Be("true");
            parsed.Options[1].Value.Should().Be("Message");
            parsed.Arguments.Should().HaveCount(4);
            parsed.Arguments[0].Value.Should().Be("src1");
            parsed.Arguments[1].Value.Should().Be("src2");
            parsed.Arguments[2].Value.Should().Be("src3");
            parsed.Arguments[3].Value.Should().Be("dest");
            parsed.UnknownOptions.Should().BeEmpty();
        }

        [Fact]
        public void ParseCommand_LongOptions_Insufficient_Value()
        {
            var args = new[] { "--message" };
            var parsed = new CoconaCommandLineParser().ParseCommand(
                args,
                new CommandOptionDescriptor[]
                {
                    CreateCommandOption(typeof(string), "message", new [] { 'm' }, "", new CoconaDefaultValue(string.Empty)),
                },
                new CommandArgumentDescriptor[]
                {
                }
            );
            parsed.Should().NotBeNull();
            parsed.Options.Should().HaveCount(1);
            parsed.Options[0].Value.Should().BeNull();
            parsed.Arguments.Should().BeEmpty();
            parsed.UnknownOptions.Should().BeEmpty();
        }

        [Fact]
        public void ParseCommand_ShortOptionWithValue_Arguments()
        {
            var args = new[] { "-rm", "Message", "src1", "src2", "src3", "dest" };
            var parsed = new CoconaCommandLineParser().ParseCommand(
                args,
                new CommandOptionDescriptor[]
                {
                        CreateCommandOption(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                        CreateCommandOption(typeof(bool), "force", new [] { 'f' }, "", new CoconaDefaultValue(false)),
                        CreateCommandOption(typeof(string), "message", new [] { 'm' }, "", new CoconaDefaultValue(string.Empty)),
                },
                new CommandArgumentDescriptor[]
                {
                        new CommandArgumentDescriptor(typeof(string[]), "src", 0, "", CoconaDefaultValue.None),
                        new CommandArgumentDescriptor(typeof(string), "dest", 0, "", CoconaDefaultValue.None),
                }
            );
            parsed.Should().NotBeNull();
            parsed.Options.Should().HaveCount(2);
            parsed.Options[0].Value.Should().Be("true");
            parsed.Options[1].Value.Should().Be("Message");
            parsed.Arguments.Should().HaveCount(4);
            parsed.Arguments[0].Value.Should().Be("src1");
            parsed.Arguments[1].Value.Should().Be("src2");
            parsed.Arguments[2].Value.Should().Be("src3");
            parsed.Arguments[3].Value.Should().Be("dest");
            parsed.UnknownOptions.Should().BeEmpty();
        }

        [Fact]
        public void ParseCommand_ShortOptionWithValue_EndOfOptions_Arguments()
        {
            var args = new[] { "-rm", "Message", "--", "src1", "src2", "src3", "dest" };
            var parsed = new CoconaCommandLineParser().ParseCommand(
                args,
                new CommandOptionDescriptor[]
                {
                        CreateCommandOption(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                        CreateCommandOption(typeof(bool), "force", new [] { 'f' }, "", new CoconaDefaultValue(false)),
                        CreateCommandOption(typeof(string), "message", new [] { 'm' }, "", new CoconaDefaultValue(string.Empty)),
                },
                new CommandArgumentDescriptor[]
                {
                        new CommandArgumentDescriptor(typeof(string[]), "src", 0, "", CoconaDefaultValue.None),
                        new CommandArgumentDescriptor(typeof(string), "dest", 0, "", CoconaDefaultValue.None),
                }
            );
            parsed.Should().NotBeNull();
            parsed.Options.Should().HaveCount(2);
            parsed.Options[0].Value.Should().Be("true");
            parsed.Options[1].Value.Should().Be("Message");
            parsed.Arguments.Should().HaveCount(4);
            parsed.Arguments[0].Value.Should().Be("src1");
            parsed.Arguments[1].Value.Should().Be("src2");
            parsed.Arguments[2].Value.Should().Be("src3");
            parsed.Arguments[3].Value.Should().Be("dest");
            parsed.UnknownOptions.Should().BeEmpty();
        }

        [Fact]
        public void ParseCommand_ShortOptionWithValue_EndOfOptions_Arguments_Hyphen()
        {
            var args = new[] { "-rm", "Message", "--", "--filename", "src1", "src2", "src3", "dest" };
            var parsed = new CoconaCommandLineParser().ParseCommand(
                args,
                new CommandOptionDescriptor[]
                {
                        CreateCommandOption(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                        CreateCommandOption(typeof(bool), "force", new [] { 'f' }, "", new CoconaDefaultValue(false)),
                        CreateCommandOption(typeof(string), "message", new [] { 'm' }, "", new CoconaDefaultValue(string.Empty)),
                },
                new CommandArgumentDescriptor[]
                {
                        new CommandArgumentDescriptor(typeof(string[]), "src", 0, "", CoconaDefaultValue.None),
                        new CommandArgumentDescriptor(typeof(string), "dest", 0, "", CoconaDefaultValue.None),
                }
            );
            parsed.Should().NotBeNull();
            parsed.Options.Should().HaveCount(2);
            parsed.Options[0].Value.Should().Be("true");
            parsed.Options[1].Value.Should().Be("Message");
            parsed.Arguments.Should().HaveCount(5);
            parsed.Arguments[0].Value.Should().Be("--filename");
            parsed.Arguments[1].Value.Should().Be("src1");
            parsed.Arguments[2].Value.Should().Be("src2");
            parsed.Arguments[3].Value.Should().Be("src3");
            parsed.Arguments[4].Value.Should().Be("dest");
            parsed.UnknownOptions.Should().BeEmpty();
        }

        [Fact]
        public void ParseCommand_ShortOptionWithContinuousValue()
        {
            var args = new[] { "-I../include/1", "-I../include/2" };
            var parsed = new CoconaCommandLineParser().ParseCommand(
                args,
                new CommandOptionDescriptor[]
                {
                    CreateCommandOption(typeof(string), "include", new [] { 'I' }, "", new CoconaDefaultValue(string.Empty)),
                    CreateCommandOption(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                    CreateCommandOption(typeof(bool), "force", new [] { 'f' }, "", new CoconaDefaultValue(false)),
                    CreateCommandOption(typeof(string), "message", new [] { 'm' }, "", new CoconaDefaultValue(string.Empty)),
                },
                new CommandArgumentDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string[]), "src", 0, "", CoconaDefaultValue.None),
                    new CommandArgumentDescriptor(typeof(string), "dest", 0, "", CoconaDefaultValue.None),
                }
            );
            parsed.Should().NotBeNull();
            parsed.Options.Should().HaveCount(2);
            parsed.Options[0].Value.Should().Be("../include/1");
            parsed.Options[1].Value.Should().Be("../include/2");
            parsed.Arguments.Should().BeEmpty();
            parsed.UnknownOptions.Should().BeEmpty();
        }

        [Fact]
        public void ParseCommand_ShortOptionWithContinuousValue_Arguments()
        {
            var args = new[] { "-I../include/1", "-I../include/2", "src1", "src2", "src3", "dest" };
            var parsed = new CoconaCommandLineParser().ParseCommand(
                args,
                new CommandOptionDescriptor[]
                {
                    CreateCommandOption(typeof(string), "include", new [] { 'I' }, "", new CoconaDefaultValue(string.Empty)),
                    CreateCommandOption(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                    CreateCommandOption(typeof(bool), "force", new [] { 'f' }, "", new CoconaDefaultValue(false)),
                    CreateCommandOption(typeof(string), "message", new [] { 'm' }, "", new CoconaDefaultValue(string.Empty)),
                },
                new CommandArgumentDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string[]), "src", 0, "", CoconaDefaultValue.None),
                    new CommandArgumentDescriptor(typeof(string), "dest", 0, "", CoconaDefaultValue.None),
                }
            );
            parsed.Should().NotBeNull();
            parsed.Options.Should().HaveCount(2);
            parsed.Options[0].Value.Should().Be("../include/1");
            parsed.Options[1].Value.Should().Be("../include/2");
            parsed.Arguments.Should().HaveCount(4);
            parsed.Arguments[0].Value.Should().Be("src1");
            parsed.Arguments[1].Value.Should().Be("src2");
            parsed.Arguments[2].Value.Should().Be("src3");
            parsed.Arguments[3].Value.Should().Be("dest");
            parsed.UnknownOptions.Should().BeEmpty();
        }

        [Fact]
        public void ParseCommand_ShortOptionWithContinuousValue_EndOfOptions_Arguments()
        {
            var args = new[] { "-I../include/1", "-I../include/2", "--", "src1", "src2", "src3", "dest" };
            var parsed = new CoconaCommandLineParser().ParseCommand(
                args,
                new CommandOptionDescriptor[]
                {
                    CreateCommandOption(typeof(string), "include", new [] { 'I' }, "", new CoconaDefaultValue(string.Empty)),
                    CreateCommandOption(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                    CreateCommandOption(typeof(bool), "force", new [] { 'f' }, "", new CoconaDefaultValue(false)),
                    CreateCommandOption(typeof(string), "message", new [] { 'm' }, "", new CoconaDefaultValue(string.Empty)),
                },
                new CommandArgumentDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string[]), "src", 0, "", CoconaDefaultValue.None),
                    new CommandArgumentDescriptor(typeof(string), "dest", 0, "", CoconaDefaultValue.None),
                }
            );
            parsed.Should().NotBeNull();
            parsed.Options.Should().HaveCount(2);
            parsed.Options[0].Value.Should().Be("../include/1");
            parsed.Options[1].Value.Should().Be("../include/2");
            parsed.Arguments.Should().HaveCount(4);
            parsed.Arguments[0].Value.Should().Be("src1");
            parsed.Arguments[1].Value.Should().Be("src2");
            parsed.Arguments[2].Value.Should().Be("src3");
            parsed.Arguments[3].Value.Should().Be("dest");
            parsed.UnknownOptions.Should().BeEmpty();
        }

        [Fact]
        public void ParseCommand_ShortOptionWithContinuousValue_EndOfOptions_Arguments_Hyphen()
        {
            var args = new[] { "-I../include/1", "-I../include/2", "--", "--filename", "src1", "src2", "src3", "dest" };
            var parsed = new CoconaCommandLineParser().ParseCommand(
                args,
                new CommandOptionDescriptor[]
                {
                    CreateCommandOption(typeof(string), "include", new [] { 'I' }, "", new CoconaDefaultValue(string.Empty)),
                    CreateCommandOption(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                    CreateCommandOption(typeof(bool), "force", new [] { 'f' }, "", new CoconaDefaultValue(false)),
                    CreateCommandOption(typeof(string), "message", new [] { 'm' }, "", new CoconaDefaultValue(string.Empty)),
                },
                new CommandArgumentDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string[]), "src", 0, "", CoconaDefaultValue.None),
                    new CommandArgumentDescriptor(typeof(string), "dest", 0, "", CoconaDefaultValue.None),
                }
            );
            parsed.Should().NotBeNull();
            parsed.Options.Should().HaveCount(2);
            parsed.Options[0].Value.Should().Be("../include/1");
            parsed.Options[1].Value.Should().Be("../include/2");
            parsed.Arguments.Should().HaveCount(5);
            parsed.Arguments[0].Value.Should().Be("--filename");
            parsed.Arguments[1].Value.Should().Be("src1");
            parsed.Arguments[2].Value.Should().Be("src2");
            parsed.Arguments[3].Value.Should().Be("src3");
            parsed.Arguments[4].Value.Should().Be("dest");
            parsed.UnknownOptions.Should().BeEmpty();
        }


        [Fact]
        public void ParseCommand_ShortOptionWithEqualValue()
        {
            var args = new[] { "-I=../include/1", "-I=../include/2" };
            var parsed = new CoconaCommandLineParser().ParseCommand(
                args,
                new CommandOptionDescriptor[]
                {
                    CreateCommandOption(typeof(string), "include", new [] { 'I' }, "", new CoconaDefaultValue(string.Empty)),
                    CreateCommandOption(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                    CreateCommandOption(typeof(bool), "force", new [] { 'f' }, "", new CoconaDefaultValue(false)),
                    CreateCommandOption(typeof(string), "message", new [] { 'm' }, "", new CoconaDefaultValue(string.Empty)),
                },
                new CommandArgumentDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string[]), "src", 0, "", CoconaDefaultValue.None),
                    new CommandArgumentDescriptor(typeof(string), "dest", 0, "", CoconaDefaultValue.None),
                }
            );
            parsed.Should().NotBeNull();
            parsed.Options.Should().HaveCount(2);
            parsed.Options[0].Value.Should().Be("../include/1");
            parsed.Options[1].Value.Should().Be("../include/2");
            parsed.Arguments.Should().BeEmpty();
            parsed.UnknownOptions.Should().BeEmpty();
        }

        [Fact]
        public void ParseCommand_ShortOptionWithEqualValue_Arguments()
        {
            var args = new[] { "-I=../include/1", "-I=../include/2", "src1", "src2", "src3", "dest" };
            var parsed = new CoconaCommandLineParser().ParseCommand(
                args,
                new CommandOptionDescriptor[]
                {
                    CreateCommandOption(typeof(string), "include", new [] { 'I' }, "", new CoconaDefaultValue(string.Empty)),
                    CreateCommandOption(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                    CreateCommandOption(typeof(bool), "force", new [] { 'f' }, "", new CoconaDefaultValue(false)),
                    CreateCommandOption(typeof(string), "message", new [] { 'm' }, "", new CoconaDefaultValue(string.Empty)),
                },
                new CommandArgumentDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string[]), "src", 0, "", CoconaDefaultValue.None),
                    new CommandArgumentDescriptor(typeof(string), "dest", 0, "", CoconaDefaultValue.None),
                }
            );
            parsed.Should().NotBeNull();
            parsed.Options.Should().HaveCount(2);
            parsed.Options[0].Value.Should().Be("../include/1");
            parsed.Options[1].Value.Should().Be("../include/2");
            parsed.Arguments.Should().HaveCount(4);
            parsed.Arguments[0].Value.Should().Be("src1");
            parsed.Arguments[1].Value.Should().Be("src2");
            parsed.Arguments[2].Value.Should().Be("src3");
            parsed.Arguments[3].Value.Should().Be("dest");
            parsed.UnknownOptions.Should().BeEmpty();
        }

        [Fact]
        public void ParseCommand_ShortOptionWithEqualValue_EndOfOptions_Arguments()
        {
            var args = new[] { "-I=../include/1", "-I=../include/2", "--", "src1", "src2", "src3", "dest" };
            var parsed = new CoconaCommandLineParser().ParseCommand(
                args,
                new CommandOptionDescriptor[]
                {
                    CreateCommandOption(typeof(string), "include", new [] { 'I' }, "", new CoconaDefaultValue(string.Empty)),
                    CreateCommandOption(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                    CreateCommandOption(typeof(bool), "force", new [] { 'f' }, "", new CoconaDefaultValue(false)),
                    CreateCommandOption(typeof(string), "message", new [] { 'm' }, "", new CoconaDefaultValue(string.Empty)),
                },
                new CommandArgumentDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string[]), "src", 0, "", CoconaDefaultValue.None),
                    new CommandArgumentDescriptor(typeof(string), "dest", 0, "", CoconaDefaultValue.None),
                }
            );
            parsed.Should().NotBeNull();
            parsed.Options.Should().HaveCount(2);
            parsed.Options[0].Value.Should().Be("../include/1");
            parsed.Options[1].Value.Should().Be("../include/2");
            parsed.Arguments.Should().HaveCount(4);
            parsed.Arguments[0].Value.Should().Be("src1");
            parsed.Arguments[1].Value.Should().Be("src2");
            parsed.Arguments[2].Value.Should().Be("src3");
            parsed.Arguments[3].Value.Should().Be("dest");
            parsed.UnknownOptions.Should().BeEmpty();
        }

        [Fact]
        public void ParseCommand_ShortOptionWithEqualValue_EndOfOptions_Arguments_Hyphen()
        {
            var args = new[] { "-I=../include/1", "-I=../include/2", "--", "--filename", "src1", "src2", "src3", "dest" };
            var parsed = new CoconaCommandLineParser().ParseCommand(
                args,
                new CommandOptionDescriptor[]
                {
                    CreateCommandOption(typeof(string), "include", new [] { 'I' }, "", new CoconaDefaultValue(string.Empty)),
                    CreateCommandOption(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                    CreateCommandOption(typeof(bool), "force", new [] { 'f' }, "", new CoconaDefaultValue(false)),
                    CreateCommandOption(typeof(string), "message", new [] { 'm' }, "", new CoconaDefaultValue(string.Empty)),
                },
                new CommandArgumentDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string[]), "src", 0, "", CoconaDefaultValue.None),
                    new CommandArgumentDescriptor(typeof(string), "dest", 0, "", CoconaDefaultValue.None),
                }
            );
            parsed.Should().NotBeNull();
            parsed.Options.Should().HaveCount(2);
            parsed.Options[0].Value.Should().Be("../include/1");
            parsed.Options[1].Value.Should().Be("../include/2");
            parsed.Arguments.Should().HaveCount(5);
            parsed.Arguments[0].Value.Should().Be("--filename");
            parsed.Arguments[1].Value.Should().Be("src1");
            parsed.Arguments[2].Value.Should().Be("src2");
            parsed.Arguments[3].Value.Should().Be("src3");
            parsed.Arguments[4].Value.Should().Be("dest");
            parsed.UnknownOptions.Should().BeEmpty();
        }


        [Fact]
        public void ParseCommand_UnknownOptions_1()
        {
            var args = new[] { "-I=../include/1", "-I=../include/2", "-y", "--", "--filename", "src1", "src2", "src3", "dest" };
            var parsed = new CoconaCommandLineParser().ParseCommand(
                args,
                new CommandOptionDescriptor[]
                {
                    CreateCommandOption(typeof(string), "include", new [] { 'I' }, "", new CoconaDefaultValue(string.Empty)),
                    CreateCommandOption(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                    CreateCommandOption(typeof(bool), "force", new [] { 'f' }, "", new CoconaDefaultValue(false)),
                    CreateCommandOption(typeof(string), "message", new [] { 'm' }, "", new CoconaDefaultValue(string.Empty)),
                },
                new CommandArgumentDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string[]), "src", 0, "", CoconaDefaultValue.None),
                    new CommandArgumentDescriptor(typeof(string), "dest", 0, "", CoconaDefaultValue.None),
                }
            );
            parsed.Should().NotBeNull();
            parsed.Options.Should().HaveCount(2);
            parsed.Arguments.Should().HaveCount(5);
            parsed.UnknownOptions.Should().HaveCount(1);
            parsed.UnknownOptions[0].Should().Be("y");
        }

        [Fact]
        public void ParseCommand_UnknownOptions_2()
        {
            var args = new[] { "-I=../include/1", "-X=../include/2", "-y", "--", "--filename", "src1", "src2", "src3", "dest" };
            var parsed = new CoconaCommandLineParser().ParseCommand(
                args,
                new CommandOptionDescriptor[]
                {
                    CreateCommandOption(typeof(string), "include", new [] { 'I' }, "", new CoconaDefaultValue(string.Empty)),
                    CreateCommandOption(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                    CreateCommandOption(typeof(bool), "force", new [] { 'f' }, "", new CoconaDefaultValue(false)),
                    CreateCommandOption(typeof(string), "message", new [] { 'm' }, "", new CoconaDefaultValue(string.Empty)),
                },
                new CommandArgumentDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string[]), "src", 0, "", CoconaDefaultValue.None),
                    new CommandArgumentDescriptor(typeof(string), "dest", 0, "", CoconaDefaultValue.None),
                }
            );
            parsed.Should().NotBeNull();
            parsed.Options.Should().HaveCount(1); // 'I'
            parsed.Arguments.Should().HaveCount(5);
            parsed.UnknownOptions.Should().Contain("X"); // 'X'
            parsed.UnknownOptions.Should().Contain("y");
        }

        [Fact]
        public void ParseCommand_UnknownOptions_3()
        {
            var args = new[] { "-=" };
            var parsed = new CoconaCommandLineParser().ParseCommand(
                args,
                new CommandOptionDescriptor[]
                {
                    CreateCommandOption(typeof(string), "include", new [] { 'I' }, "", new CoconaDefaultValue(string.Empty)),
                    CreateCommandOption(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                    CreateCommandOption(typeof(bool), "force", new [] { 'f' }, "", new CoconaDefaultValue(false)),
                    CreateCommandOption(typeof(string), "message", new [] { 'm' }, "", new CoconaDefaultValue(string.Empty)),
                },
                new CommandArgumentDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string[]), "src", 0, "", CoconaDefaultValue.None),
                    new CommandArgumentDescriptor(typeof(string), "dest", 0, "", CoconaDefaultValue.None),
                }
            );
            parsed.Should().NotBeNull();
            parsed.Options.Should().BeEmpty();
            parsed.Arguments.Should().BeEmpty();
            parsed.UnknownOptions.Should().HaveCount(1); // '='
        }

        [Fact]
        public void ParseCommand_UnknownOptions_LongName_1()
        {
            var args = new[] { "--help" };
            var parsed = new CoconaCommandLineParser().ParseCommand(
                args,
                new CommandOptionDescriptor[]
                {
                },
                new CommandArgumentDescriptor[]
                {
                }
            );
            parsed.Should().NotBeNull();
            parsed.Options.Should().BeEmpty();
            parsed.Arguments.Should().BeEmpty();
            parsed.UnknownOptions.Should().HaveCount(1);
            parsed.UnknownOptions.Should().Equal(new[] { "help" });
        }

        [Fact]
        public void ParseCommand_UnknownOptions_LongName_2()
        {
            var args = new[] { "--help", "--foo" };
            var parsed = new CoconaCommandLineParser().ParseCommand(
                args,
                new CommandOptionDescriptor[]
                {
                },
                new CommandArgumentDescriptor[]
                {
                }
            );
            parsed.Should().NotBeNull();
            parsed.Options.Should().BeEmpty();
            parsed.Arguments.Should().BeEmpty();
            parsed.UnknownOptions.Should().HaveCount(2);
            parsed.UnknownOptions.Should().Equal(new[] { "help", "foo" });
        }

        [Fact]
        public void ParseCommand_UnknownOptions_LongName_Value()
        {
            var args = new[] { "--foo=bar" };
            var parsed = new CoconaCommandLineParser().ParseCommand(
                args,
                new CommandOptionDescriptor[]
                {
                },
                new CommandArgumentDescriptor[]
                {
                }
            );
            parsed.Should().NotBeNull();
            parsed.Options.Should().BeEmpty();
            parsed.Arguments.Should().BeEmpty();
            parsed.UnknownOptions.Should().HaveCount(1);
            parsed.UnknownOptions.Should().Equal(new[] { "foo=bar" });
        }

        [Fact]
        public void ParseCommand_UnknownOptions_ShortName_1()
        {
            var args = new[] { "-h" };
            var parsed = new CoconaCommandLineParser().ParseCommand(
                args,
                new CommandOptionDescriptor[]
                {
                },
                new CommandArgumentDescriptor[]
                {
                }
            );
            parsed.Should().NotBeNull();
            parsed.Options.Should().BeEmpty();
            parsed.Arguments.Should().BeEmpty();
            parsed.UnknownOptions.Should().HaveCount(1);
            parsed.UnknownOptions.Should().Equal(new[] { "h" });
        }

        [Fact]
        public void ParseCommand_UnknownOptions_ShortName_2()
        {
            var args = new[] { "-h", "-f" };
            var parsed = new CoconaCommandLineParser().ParseCommand(
                args,
                new CommandOptionDescriptor[]
                {
                },
                new CommandArgumentDescriptor[]
                {
                }
            );
            parsed.Should().NotBeNull();
            parsed.Options.Should().BeEmpty();
            parsed.Arguments.Should().BeEmpty();
            parsed.UnknownOptions.Should().HaveCount(2);
            parsed.UnknownOptions.Should().Equal(new[] { "h", "f" });
        }

        [Fact]
        public void ParseCommand_ShortOptions_Value_Pattern_1()
        {
            var args = new[] { "-mX=hogehoge" };
            var parsed = new CoconaCommandLineParser().ParseCommand(
                args,
                new CommandOptionDescriptor[]
                {
                    CreateCommandOption(typeof(string), "message", new [] { 'm' }, "", new CoconaDefaultValue(string.Empty)),
                },
                new CommandArgumentDescriptor[]
                {
                }
            );
            parsed.Should().NotBeNull();
            parsed.Options.Should().HaveCount(1);
            parsed.Options[0].Value.Should().Be("X=hogehoge");
            parsed.Arguments.Should().BeEmpty();
            parsed.UnknownOptions.Should().BeEmpty();
        }

        [Fact]
        public void ParseCommand_ShortOptions_Value_Pattern_2()
        {
            var args = new[] { "-Xm=hogehoge" };
            var parsed = new CoconaCommandLineParser().ParseCommand(
                args,
                new CommandOptionDescriptor[]
                {
                    CreateCommandOption(typeof(string), "message", new [] { 'm' }, "", new CoconaDefaultValue(string.Empty)),
                },
                new CommandArgumentDescriptor[]
                {
                }
            );
            parsed.Should().NotBeNull();
            parsed.Options.Should().HaveCount(1);
            parsed.Options[0].Value.Should().Be("hogehoge");
            parsed.Arguments.Should().BeEmpty();
            parsed.UnknownOptions.Should().HaveCount(1);
        }

        [Fact]
        public void ParseCommand_ShortOptions_Insufficient_Value()
        {
            var args = new[] { "-m" };
            var parsed = new CoconaCommandLineParser().ParseCommand(
                args,
                new CommandOptionDescriptor[]
                {
                    CreateCommandOption(typeof(string), "message", new [] { 'm' }, "", new CoconaDefaultValue(string.Empty)),
                },
                new CommandArgumentDescriptor[]
                {
                }
            );
            parsed.Should().NotBeNull();
            parsed.Options.Should().HaveCount(1);
            parsed.Options[0].Value.Should().BeNull();
            parsed.Arguments.Should().BeEmpty();
            parsed.UnknownOptions.Should().BeEmpty();
        }
    }
}
