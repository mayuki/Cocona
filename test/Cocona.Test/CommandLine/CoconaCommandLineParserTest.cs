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
                    new CommandOptionDescriptor(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                    new CommandOptionDescriptor(typeof(bool), "force", new [] { 'f' }, "", new CoconaDefaultValue(false)),
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
        public void ParseCommand_LongOption_EndOfOptions()
        {
            var args = new[] { "--recursive","src1", "src2", "src3", "dest" };
            var parsed = new CoconaCommandLineParser().ParseCommand(
                args,
                new CommandOptionDescriptor[]
                {
                    new CommandOptionDescriptor(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
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
                        new CommandOptionDescriptor(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
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
                    new CommandOptionDescriptor(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                    new CommandOptionDescriptor(typeof(bool), "force", new [] { 'f' }, "", new CoconaDefaultValue(false)),
                    new CommandOptionDescriptor(typeof(string), "message", new [] { 'm' }, "", new CoconaDefaultValue(string.Empty)),
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
                    new CommandOptionDescriptor(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                    new CommandOptionDescriptor(typeof(bool), "force", new [] { 'f' }, "", new CoconaDefaultValue(false)),
                    new CommandOptionDescriptor(typeof(string), "message", new [] { 'm' }, "", new CoconaDefaultValue(string.Empty)),
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
                        new CommandOptionDescriptor(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                        new CommandOptionDescriptor(typeof(bool), "force", new [] { 'f' }, "", new CoconaDefaultValue(false)),
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
                        new CommandOptionDescriptor(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                        new CommandOptionDescriptor(typeof(bool), "force", new [] { 'f' }, "", new CoconaDefaultValue(false)),
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
                        new CommandOptionDescriptor(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                        new CommandOptionDescriptor(typeof(bool), "force", new [] { 'f' }, "", new CoconaDefaultValue(false)),
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
                        new CommandOptionDescriptor(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                        new CommandOptionDescriptor(typeof(bool), "force", new [] { 'f' }, "", new CoconaDefaultValue(false)),
                        new CommandOptionDescriptor(typeof(string), "message", new [] { 'm' }, "", new CoconaDefaultValue(string.Empty)),
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
                        new CommandOptionDescriptor(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                        new CommandOptionDescriptor(typeof(bool), "force", new [] { 'f' }, "", new CoconaDefaultValue(false)),
                        new CommandOptionDescriptor(typeof(string), "message", new [] { 'm' }, "", new CoconaDefaultValue(string.Empty)),
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
                        new CommandOptionDescriptor(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                        new CommandOptionDescriptor(typeof(bool), "force", new [] { 'f' }, "", new CoconaDefaultValue(false)),
                        new CommandOptionDescriptor(typeof(string), "message", new [] { 'm' }, "", new CoconaDefaultValue(string.Empty)),
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
                        new CommandOptionDescriptor(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                        new CommandOptionDescriptor(typeof(bool), "force", new [] { 'f' }, "", new CoconaDefaultValue(false)),
                        new CommandOptionDescriptor(typeof(string), "message", new [] { 'm' }, "", new CoconaDefaultValue(string.Empty)),
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
                    new CommandOptionDescriptor(typeof(string), "include", new [] { 'I' }, "", new CoconaDefaultValue(string.Empty)),
                    new CommandOptionDescriptor(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                    new CommandOptionDescriptor(typeof(bool), "force", new [] { 'f' }, "", new CoconaDefaultValue(false)),
                    new CommandOptionDescriptor(typeof(string), "message", new [] { 'm' }, "", new CoconaDefaultValue(string.Empty)),
                },
                new CommandArgumentDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string[]), "src", 0, "", CoconaDefaultValue.None),
                    new CommandArgumentDescriptor(typeof(string), "dest", 0, "", CoconaDefaultValue.None),
                }
            );
            parsed.Should().NotBeNull();
            parsed.Options.Should().HaveCount(2);
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
                    new CommandOptionDescriptor(typeof(string), "include", new [] { 'I' }, "", new CoconaDefaultValue(string.Empty)),
                    new CommandOptionDescriptor(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                    new CommandOptionDescriptor(typeof(bool), "force", new [] { 'f' }, "", new CoconaDefaultValue(false)),
                    new CommandOptionDescriptor(typeof(string), "message", new [] { 'm' }, "", new CoconaDefaultValue(string.Empty)),
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
                    new CommandOptionDescriptor(typeof(string), "include", new [] { 'I' }, "", new CoconaDefaultValue(string.Empty)),
                    new CommandOptionDescriptor(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                    new CommandOptionDescriptor(typeof(bool), "force", new [] { 'f' }, "", new CoconaDefaultValue(false)),
                    new CommandOptionDescriptor(typeof(string), "message", new [] { 'm' }, "", new CoconaDefaultValue(string.Empty)),
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
                    new CommandOptionDescriptor(typeof(string), "include", new [] { 'I' }, "", new CoconaDefaultValue(string.Empty)),
                    new CommandOptionDescriptor(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                    new CommandOptionDescriptor(typeof(bool), "force", new [] { 'f' }, "", new CoconaDefaultValue(false)),
                    new CommandOptionDescriptor(typeof(string), "message", new [] { 'm' }, "", new CoconaDefaultValue(string.Empty)),
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
                    new CommandOptionDescriptor(typeof(string), "include", new [] { 'I' }, "", new CoconaDefaultValue(string.Empty)),
                    new CommandOptionDescriptor(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                    new CommandOptionDescriptor(typeof(bool), "force", new [] { 'f' }, "", new CoconaDefaultValue(false)),
                    new CommandOptionDescriptor(typeof(string), "message", new [] { 'm' }, "", new CoconaDefaultValue(string.Empty)),
                },
                new CommandArgumentDescriptor[]
                {
                    new CommandArgumentDescriptor(typeof(string[]), "src", 0, "", CoconaDefaultValue.None),
                    new CommandArgumentDescriptor(typeof(string), "dest", 0, "", CoconaDefaultValue.None),
                }
            );
            parsed.Should().NotBeNull();
            parsed.Options.Should().HaveCount(2);
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
                    new CommandOptionDescriptor(typeof(string), "include", new [] { 'I' }, "", new CoconaDefaultValue(string.Empty)),
                    new CommandOptionDescriptor(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                    new CommandOptionDescriptor(typeof(bool), "force", new [] { 'f' }, "", new CoconaDefaultValue(false)),
                    new CommandOptionDescriptor(typeof(string), "message", new [] { 'm' }, "", new CoconaDefaultValue(string.Empty)),
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
                    new CommandOptionDescriptor(typeof(string), "include", new [] { 'I' }, "", new CoconaDefaultValue(string.Empty)),
                    new CommandOptionDescriptor(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                    new CommandOptionDescriptor(typeof(bool), "force", new [] { 'f' }, "", new CoconaDefaultValue(false)),
                    new CommandOptionDescriptor(typeof(string), "message", new [] { 'm' }, "", new CoconaDefaultValue(string.Empty)),
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
                    new CommandOptionDescriptor(typeof(string), "include", new [] { 'I' }, "", new CoconaDefaultValue(string.Empty)),
                    new CommandOptionDescriptor(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                    new CommandOptionDescriptor(typeof(bool), "force", new [] { 'f' }, "", new CoconaDefaultValue(false)),
                    new CommandOptionDescriptor(typeof(string), "message", new [] { 'm' }, "", new CoconaDefaultValue(string.Empty)),
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
                    new CommandOptionDescriptor(typeof(string), "include", new [] { 'I' }, "", new CoconaDefaultValue(string.Empty)),
                    new CommandOptionDescriptor(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                    new CommandOptionDescriptor(typeof(bool), "force", new [] { 'f' }, "", new CoconaDefaultValue(false)),
                    new CommandOptionDescriptor(typeof(string), "message", new [] { 'm' }, "", new CoconaDefaultValue(string.Empty)),
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
        }

        [Fact]
        public void ParseCommand_UnknownOptions_2()
        {
            var args = new[] { "-I=../include/1", "-X=../include/2", "-y", "--", "--filename", "src1", "src2", "src3", "dest" };
            var parsed = new CoconaCommandLineParser().ParseCommand(
                args,
                new CommandOptionDescriptor[]
                {
                    new CommandOptionDescriptor(typeof(string), "include", new [] { 'I' }, "", new CoconaDefaultValue(string.Empty)),
                    new CommandOptionDescriptor(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                    new CommandOptionDescriptor(typeof(bool), "force", new [] { 'f' }, "", new CoconaDefaultValue(false)),
                    new CommandOptionDescriptor(typeof(string), "message", new [] { 'm' }, "", new CoconaDefaultValue(string.Empty)),
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
        }

        [Fact]
        public void ParseCommand_UnknownOptions_3()
        {
            var args = new[] { "-=" };
            var parsed = new CoconaCommandLineParser().ParseCommand(
                args,
                new CommandOptionDescriptor[]
                {
                    new CommandOptionDescriptor(typeof(string), "include", new [] { 'I' }, "", new CoconaDefaultValue(string.Empty)),
                    new CommandOptionDescriptor(typeof(bool), "recursive", new [] { 'r', 'R' }, "", new CoconaDefaultValue(false)),
                    new CommandOptionDescriptor(typeof(bool), "force", new [] { 'f' }, "", new CoconaDefaultValue(false)),
                    new CommandOptionDescriptor(typeof(string), "message", new [] { 'm' }, "", new CoconaDefaultValue(string.Empty)),
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
        public void ParseCommand_ShortOptions_Value_Pattern_1()
        {
            var args = new[] { "-mX=hogehoge" };
            var parsed = new CoconaCommandLineParser().ParseCommand(
                args,
                new CommandOptionDescriptor[]
                {
                    new CommandOptionDescriptor(typeof(string), "message", new [] { 'm' }, "", new CoconaDefaultValue(string.Empty)),
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
                    new CommandOptionDescriptor(typeof(string), "message", new [] { 'm' }, "", new CoconaDefaultValue(string.Empty)),
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
    }
}
