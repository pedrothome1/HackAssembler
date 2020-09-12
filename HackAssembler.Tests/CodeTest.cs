using Xunit;

namespace HackAssembler.Tests
{
    public class CodeTest
    {
        [Fact]
        public void Dest_ReturnsCorrectBinarySequenceForGivenDestination()
        {
            var code = new Code();

            Assert.Equal("000", code.Dest(""));
            Assert.Equal("000", code.Dest("null"));
            Assert.Equal("000", code.Dest(null));

            Assert.Equal("001", code.Dest("M"));
            Assert.Equal("010", code.Dest("D"));
            Assert.Equal("011", code.Dest("MD"));
            Assert.Equal("100", code.Dest("A"));
            Assert.Equal("101", code.Dest("AM"));
            Assert.Equal("110", code.Dest("AD"));
            Assert.Equal("111", code.Dest("AMD"));
        }

        [Fact]
        public void Comp_ReturnsCorrectBinarySequenceForGivenComputation()
        {
            var code = new Code();

            Assert.Equal("0101010", code.Comp("0"));
            Assert.Equal("0111111", code.Comp("1"));
            Assert.Equal("0111010", code.Comp("-1"));
            Assert.Equal("0001100", code.Comp("D"));

            Assert.Equal("0110000", code.Comp("A"));
            Assert.Equal("1110000", code.Comp("M"));

            Assert.Equal("0001101", code.Comp("!D"));

            Assert.Equal("0110001", code.Comp("!A"));
            Assert.Equal("1110001", code.Comp("!M"));

            Assert.Equal("0001111", code.Comp("-D"));

            Assert.Equal("0110011", code.Comp("-A"));
            Assert.Equal("1110011", code.Comp("-M"));

            Assert.Equal("0011111", code.Comp("D+1"));

            Assert.Equal("0110111", code.Comp("A+1"));
            Assert.Equal("1110111", code.Comp("M+1"));

            Assert.Equal("0001110", code.Comp("D-1"));

            Assert.Equal("0110010", code.Comp("A-1"));
            Assert.Equal("1110010", code.Comp("M-1"));

            Assert.Equal("0000010", code.Comp("D+A"));
            Assert.Equal("1000010", code.Comp("D+M"));

            Assert.Equal("0010011", code.Comp("D-A"));
            Assert.Equal("1010011", code.Comp("D-M"));

            Assert.Equal("0000111", code.Comp("A-D"));
            Assert.Equal("1000111", code.Comp("M-D"));

            Assert.Equal("0000000", code.Comp("D&A"));
            Assert.Equal("1000000", code.Comp("D&M"));

            Assert.Equal("0010101", code.Comp("D|A"));
            Assert.Equal("1010101", code.Comp("D|M"));
        }

        [Fact]
        public void Dest_ReturnsCorrectBinarySequenceForGivenJump()
        {
            var code = new Code();

            Assert.Equal("000", code.Jump(""));
            Assert.Equal("000", code.Jump("null"));
            Assert.Equal("000", code.Jump(null));

            Assert.Equal("001", code.Jump("JGT"));
            Assert.Equal("010", code.Jump("JEQ"));
            Assert.Equal("011", code.Jump("JGE"));
            Assert.Equal("100", code.Jump("JLT"));
            Assert.Equal("101", code.Jump("JNE"));
            Assert.Equal("110", code.Jump("JLE"));
            Assert.Equal("111", code.Jump("JMP"));
        }
    }
}
