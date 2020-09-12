using System;
using System.Collections.Generic;

namespace HackAssembler
{
    public class Code
    {
        const string AInstructionConstant = "0";
        const string CInstructionConstants = "111";

        private static IDictionary<string, string> Computations = new Dictionary<string, string>()
        {
            ["0"]     = "0101010",
            ["1"]     = "0111111",
            ["-1"]    = "0111010",
            ["D"]     = "0001100",
            ["A"]     = "0110000",
            ["!D"]    = "0001101",
            ["!A"]    = "0110001",
            ["-D"]    = "0001111",
            ["-A"]    = "0110011",
            ["D+1"]   = "0011111",
            ["A+1"]   = "0110111",
            ["D-1"]   = "0001110",
            ["A-1"]   = "0110010",
            ["D+A"]   = "0000010",
            ["D-A"]   = "0010011",
            ["A-D"]   = "0000111",
            ["D&A"]   = "0000000",
            ["D|A"]   = "0010101",

            ["M"]     = "1110000",
            ["!M"]    = "1110001",
            ["-M"]    = "1110011",
            ["M+1"]   = "1110111",
            ["M-1"]   = "1110010",
            ["D+M"]   = "1000010",
            ["D-M"]   = "1010011",
            ["M-D"]   = "1000111",
            ["D&M"]   = "1000000",
            ["D|M"]   = "1010101",
        };

        public string Dest(string mnemonic) =>
            GetIndexAsBinary(mnemonic, new [] { "null", "M", "D", "MD", "A", "AM", "AD", "AMD" });

        public string Comp(string mnemonic) => Computations[mnemonic];

        public string Jump(string mnemonic) =>
            GetIndexAsBinary(mnemonic, new [] { "null", "JGT", "JEQ", "JGE", "JLT", "JNE", "JLE", "JMP" });

        private string GetIndexAsBinary(string mnemonic, string[] mnemonicArray)
        {
            if (string.IsNullOrWhiteSpace(mnemonic))
                mnemonic = "null";

            var code = Array.FindIndex(mnemonicArray, x => x == mnemonic);

            return Convert.ToString(code, 2).PadLeft(3, '0');
        }
    }
}
