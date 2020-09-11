using System;
using System.IO;
using System.Text;
using Xunit;

namespace HackAssembler.Tests
{
    public class ParserTest
    {
        [Fact]
        public void HasMoreCommands_WhenEmptyAssemblyIsGiven_ReturnsFalse()
        {
            var assembly = "";
            var encoding = new UTF8Encoding();

            using (var stream = new MemoryStream(encoding.GetBytes(assembly)))
            {
                using (var parser = new Parser(stream))
                {
                    Assert.False(parser.HasMoreCommands);
                }
            }
        }

        [Fact]
        public void HasMoreCommands_WhenNonEmptyAssemblyIsGiven_ReturnsTrue()
        {
            var assembly = "@R0";
            var encoding = new UTF8Encoding();

            using (var stream = new MemoryStream(encoding.GetBytes(assembly)))
            {
                using (var parser = new Parser(stream))
                {
                    Assert.True(parser.HasMoreCommands);
                }
            }
        }

        [Fact]
        public void Advance_AdvancesToNextLine()
        {
            var assembly =
                "@20\n" +
                "D=A";
            var encoding = new UTF8Encoding();

            using (var stream = new MemoryStream(encoding.GetBytes(assembly)))
            {
                using (var parser = new Parser(stream))
                {
                    parser.Advance();
                    Assert.True(parser.HasMoreCommands);

                    parser.Advance();
                    Assert.False(parser.HasMoreCommands);
                }
            }
        }

        [Fact]
        public void Advance_IgnoresEmptyLines()
        {
            var assembly =
                "@20\n" +
                "   \n" +
                "D=A";
            var encoding = new UTF8Encoding();

            using (var stream = new MemoryStream(encoding.GetBytes(assembly)))
            {
                using (var parser = new Parser(stream))
                {
                    parser.Advance();
                    Assert.True(parser.HasMoreCommands);

                    parser.Advance();
                    Assert.False(parser.HasMoreCommands);
                }
            }
        }

        [Fact]
        public void Advance_IgnoresComments()
        {
            var assembly =
                "@20\n" +
                "//Comment" +
                "// Comment beginning with space\n" +
                "D=A";
            var encoding = new UTF8Encoding();

            using (var stream = new MemoryStream(encoding.GetBytes(assembly)))
            {
                using (var parser = new Parser(stream))
                {
                    parser.Advance();
                    Assert.True(parser.HasMoreCommands);

                    parser.Advance();
                    Assert.False(parser.HasMoreCommands);
                }
            }
        }

        [Fact]
        public void Advance_DoesNothingWhenReachTheEnd()
        {
            var assembly =
                "@20\n" +
                "D=A";
            var encoding = new UTF8Encoding();

            using (var stream = new MemoryStream(encoding.GetBytes(assembly)))
            {
                using (var parser = new Parser(stream))
                {
                    parser.Advance();
                    parser.Advance();
                    parser.Advance();
                    parser.Advance();
                    parser.Advance();
                }
            }
        }

        [Fact]
        public void IsInstructionA_ReturnsTrueIfCurrentInstructionIsOfTypeA()
        {
            var assembly =
                "@20\n" +
                "D=A";
            var encoding = new UTF8Encoding();

            using (var stream = new MemoryStream(encoding.GetBytes(assembly)))
            {
                using (var parser = new Parser(stream))
                {
                    parser.Advance();
                    Assert.True(parser.IsInstructionA);

                    parser.Advance();
                    Assert.False(parser.IsInstructionA);
                }
            }
        }

        [Fact]
        public void Address_ReturnsTheValueAfterTheAtSymbol()
        {
            var assembly =
                "@sum\n" +
                "@20";
            var encoding = new UTF8Encoding();

            using (var stream = new MemoryStream(encoding.GetBytes(assembly)))
            {
                using (var parser = new Parser(stream))
                {
                    parser.Advance();
                    Assert.Equal("sum", parser.Address);

                    parser.Advance();
                    Assert.Equal("20", parser.Address);
                }
            }
        }

        [Fact]
        public void Dest_ReturnsTheDestinationRegister()
        {
            var assembly = "D=M";
            var encoding = new UTF8Encoding();

            using (var stream = new MemoryStream(encoding.GetBytes(assembly)))
            {
                using (var parser = new Parser(stream))
                {
                    parser.Advance();
                    Assert.Equal("D", parser.Dest);
                }
            }
        }

        [Fact]
        public void Comp_ReturnsTheComputation()
        {
            var assembly = "D=M+1";
            var encoding = new UTF8Encoding();

            using (var stream = new MemoryStream(encoding.GetBytes(assembly)))
            {
                using (var parser = new Parser(stream))
                {
                    parser.Advance();
                    Assert.Equal("M+1", parser.Comp);
                }
            }
        }

        [Fact]
        public void Jump_ReturnsTheComputation()
        {
            var assembly = "0;JMP";
            var encoding = new UTF8Encoding();

            using (var stream = new MemoryStream(encoding.GetBytes(assembly)))
            {
                using (var parser = new Parser(stream))
                {
                    parser.Advance();
                    Assert.Equal("JMP", parser.Jump);
                }
            }
        }

        [Fact]
        public void Label_ReturnsLabelWithinParenthesis()
        {
            var assembly = "(LOOP)";
            var encoding = new UTF8Encoding();

            using (var stream = new MemoryStream(encoding.GetBytes(assembly)))
            {
                using (var parser = new Parser(stream))
                {
                    parser.Advance();
                    Assert.Equal("LOOP", parser.Label);
                }
            }
        }

        [Fact]
        public void IsLabel_ReturnsTrueWhenEncountersLabel()
        {
            var assembly =
                "(START)\n" +
                "  @START\n" +
                "  0;JMP";
            var encoding = new UTF8Encoding();

            using (var stream = new MemoryStream(encoding.GetBytes(assembly)))
            {
                using (var parser = new Parser(stream))
                {
                    parser.Advance();
                    Assert.True(parser.IsLabel);

                    parser.Advance();
                    Assert.False(parser.IsLabel);

                    parser.Advance();
                    Assert.False(parser.IsLabel);
                }
            }
        }

        [Fact]
        public void InstructionNumber_ReturnsTheNumberOfTheCurrentInstruction()
        {
            var assembly =
                "(START)\n" +
                "  @20\n" +
                "  //Comment" +
                "  // Comment beginning with space\n" +
                "  D=A";
            var encoding = new UTF8Encoding();

            using (var stream = new MemoryStream(encoding.GetBytes(assembly)))
            {
                using (var parser = new Parser(stream))
                {
                    parser.Advance();
                    Assert.Equal(-1, parser.InstructionNumber);

                    parser.Advance();
                    Assert.Equal(0, parser.InstructionNumber);

                    parser.Advance();
                    Assert.Equal(1, parser.InstructionNumber);
                }
            }
        }

        [Fact]
        public void SkipsSpaces()
        {
            var assembly =
                "  @ R0  \n" +
                " D = M + 1 ;  JGT ";
            var encoding = new UTF8Encoding();

            using (var stream = new MemoryStream(encoding.GetBytes(assembly)))
            {
                using (var parser = new Parser(stream))
                {
                    parser.Advance();
                    Assert.Equal("R0", parser.Address);

                    parser.Advance();
                    Assert.Equal("D", parser.Dest);
                    Assert.Equal("M+1", parser.Comp);
                    Assert.Equal("JGT", parser.Jump);
                }
            }
        }
    }
}
