using System;
using System.Collections.Generic;

namespace func.brainfuck
{
    public class BrainfuckLoopCommands
    {
        public static void RegisterTo(IVirtualMachine virtualMachine)
        {
            var loopMappings = GetLoopMappings(virtualMachine.Instructions);

            virtualMachine.RegisterCommand('[', b =>
            {
                if (virtualMachine.Memory[virtualMachine.MemoryPointer] == 0)
                    virtualMachine.InstructionPointer = loopMappings[virtualMachine.InstructionPointer];   
            });

            virtualMachine.RegisterCommand(']', b =>
            {
                if (virtualMachine.Memory[virtualMachine.MemoryPointer] != 0)
                    virtualMachine.InstructionPointer = loopMappings[virtualMachine.InstructionPointer]; 
            });
        }
        /// <summary>
        /// ���������� ������������� ����� ������������ � ������������ �������� � ���� ���������.
        /// </summary>
        /// <param name="programCode"> ��� ���������..</param>
        /// <returns> �������, ���������� ������������� ����� ������������ � ������������ ��������.</returns>
        private static Dictionary<int, int> GetLoopMappings(string programCode)
        {
            var openBracketIndices = new Stack<int>();
            var loopMappings = new Dictionary<int, int>();

            for (var currentIndex = 0; currentIndex < programCode.Length; currentIndex++)
            {
                if (programCode[currentIndex] == '[') openBracketIndices.Push(currentIndex);               
                else if (programCode[currentIndex] == ']')
                {
                    if (openBracketIndices.Count == 0) throw new ArgumentException();               
                    var openBracketIndex = openBracketIndices.Pop();
                    loopMappings[openBracketIndex] = currentIndex;
                    loopMappings[currentIndex] = openBracketIndex;
                }
            }

            if (openBracketIndices.Count > 0) throw new ArgumentException();
            
            return loopMappings;
        }
    }
}
