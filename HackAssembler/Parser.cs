using System;
using System.IO;
using System.Text.RegularExpressions;

namespace HackAssembler
{
    public class Parser : IDisposable
    {
        private readonly StreamReader _streamReader;

        public Parser(Stream stream)
        {
            _streamReader = new StreamReader(stream);
            InstructionNumber = -1;
        }

        public string Address { get; private set; }
        public string Dest { get; private set; }
        public string Comp { get; private set; }
        public string Jump { get; private set; }
        public string Label { get; private set; }
        public int InstructionNumber { get; private set; }
        public bool IsInstructionA { get; private set; }
        public bool IsLabel => Label != string.Empty;


        public bool HasMoreCommands()
        {
            return !_streamReader.EndOfStream;
        }

        public void Advance()
        {
            Address = Dest = Comp = Jump = Label = string.Empty;

            var instruction = _streamReader.ReadLine().RemoveSpaces();

            while (!_streamReader.EndOfStream && (instruction == string.Empty || instruction.StartsWith("//")))
                instruction = _streamReader.ReadLine().RemoveSpaces();

            IsInstructionA = instruction[0] == '@';

            if (IsInstructionA)
            {
                Address = instruction.Substring(1);
                ++InstructionNumber;
            }
            else if (CheckLabel(instruction))
            {
                Label = GetLabel(instruction);
            }
            else
            {
                var hasDestination = instruction.Contains("=");
                var hasJump = instruction.Contains(";");
                var fields = Regex.Split(instruction, "[=;]");

                if (hasDestination && hasJump)
                {
                    Dest = fields[0];
                    Comp = fields[1];
                    Jump = fields[2];
                }
                else if (hasJump)
                {
                    Comp = fields[0];
                    Jump = fields[1];
                }
                else if (hasDestination)
                {
                    Dest = fields[0];
                    Comp = fields[1];
                }

                ++InstructionNumber;
            }
        }

        public void Dispose()
        {
            _streamReader.Dispose();
        }

        private bool CheckLabel(string instruction)
        {
            return Regex.IsMatch(instruction, @"\(\w+\)");
        }

        private string GetLabel(string instruction)
        {
            return Regex.Replace(instruction, @"\W", "");
        }
    }
}
