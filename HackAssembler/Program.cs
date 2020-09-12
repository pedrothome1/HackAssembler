using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HackAssembler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var fileName = args[0];
            using var file = File.OpenRead(fileName);

            var outputFileName = Path.GetFileNameWithoutExtension(fileName) + ".hack";
            using var outputFile = File.Create(outputFileName);
            using var streamWriter = new StreamWriter(outputFile);

            var symbolTable = InitialSymbolTable();

            using var labelParser = new Parser(file);

            while (labelParser.HasMoreCommands)
            {
                labelParser.Advance();

                if (labelParser.IsLabel)
                    symbolTable.TryAdd(labelParser.Label, (labelParser.InstructionNumber + 1).ToString());
            }

            file.Position = 0;

            using var parser = new Parser(file);
            var code = new Code();
            var nextVariableAddress = 16;

            while (parser.HasMoreCommands)
            {
                parser.Advance();

                var binaryInstruction = string.Empty;

                if (parser.IsInstructionA)
                {
                    if (int.TryParse(parser.Address, out var addressInDecimal))
                    {
                        var addressInBinary = Convert.ToString(addressInDecimal, 2).PadLeft(15, '0');
                        binaryInstruction = $"{Code.AInstructionConstant}{addressInBinary}";
                    }
                    else if (symbolTable.ContainsKey(parser.Address))
                    {
                        addressInDecimal = int.Parse(symbolTable[parser.Address]);
                        var addressInBinary = Convert.ToString(addressInDecimal, 2).PadLeft(15, '0');
                        binaryInstruction = $"{Code.AInstructionConstant}{addressInBinary}";
                    }
                    else
                    {
                        symbolTable.Add(parser.Address, nextVariableAddress.ToString());
                        var addressInBinary = Convert.ToString(nextVariableAddress, 2).PadLeft(15, '0');
                        binaryInstruction = $"{Code.AInstructionConstant}{addressInBinary}";

                        if (nextVariableAddress >= 32767)
                            nextVariableAddress = 16;
                        else
                            ++nextVariableAddress;
                    }
                }
                else if (!parser.IsLabel)
                {
                    var dest = code.Dest(parser.Dest);
                    var comp = code.Comp(parser.Comp);
                    var jump = code.Jump(parser.Jump);

                    binaryInstruction = $"{Code.CInstructionConstants}{comp}{dest}{jump}";
                }

                if (!string.IsNullOrWhiteSpace(binaryInstruction))
                    streamWriter.WriteLine(binaryInstruction);
            }
        }

        private static IDictionary<string, string> InitialSymbolTable()
        {
            var table = Enumerable
                .Range(0, 16)
                .Select(x => new [] { $"R{x}", x.ToString() })
                .ToDictionary(k => k[0], v => v[1]);

            table["SCREEN"] = "16384";
            table["KBD"] = "24576";
            table["SP"] = "0";
            table["LCL"] = "1";
            table["ARG"] = "2";
            table["THIS"] = "3";
            table["THAT"] = "4";

            return table;
        }
    }
}
