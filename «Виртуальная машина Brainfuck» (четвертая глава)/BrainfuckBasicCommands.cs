using System;
using System.Collections.Generic;

namespace func.brainfuck
{
    public class BrainfuckBasicCommands
    {
        public static void RegisterTo(IVirtualMachine virtualMachine,
            Func<int> readInput, Action<char> outputCharacter)
        {
            virtualMachine.RegisterCommand('.', b
                => { outputCharacter((char)virtualMachine.Memory[virtualMachine.MemoryPointer]); });
            virtualMachine.RegisterCommand(',', b
                => { virtualMachine.Memory[virtualMachine.MemoryPointer] = (byte)readInput(); });
            virtualMachine.RegisterCommand('+', b
                => IncrementMemory(virtualMachine));
            virtualMachine.RegisterCommand('-', b
                => DecrementMemory(virtualMachine));
            RegisterPointerCommands(virtualMachine);
            RegisterSymbols(virtualMachine, 'a', 'z');
            RegisterSymbols(virtualMachine, 'A', 'Z');
            RegisterSymbols(virtualMachine, '0', '9');
        }
        /// <summary>
        /// ����������� �������� �������� ��������� ������ �� 1, ���������� ��� �� 0 ��� ���������� 256.
        /// </summary>
        /// <param name="virtualMachine"> ����������� ������.</param>
        private static void IncrementMemory(IVirtualMachine virtualMachine)
        {
            virtualMachine.Memory[virtualMachine.MemoryPointer] =
                (byte)((virtualMachine.Memory[virtualMachine.MemoryPointer] + 1) % 256);
        }
        /// <summary>
        /// ��������� �������� �������� ��������� ������ �� 1, ���������� ��� �� 255 ��� ���������� 0
        /// </summary>
        /// <param name="virtualMachine"> ����������� ������.</param>
        private static void DecrementMemory(IVirtualMachine virtualMachine)
        {
            var newMemoryValue = virtualMachine.Memory[virtualMachine.MemoryPointer] - 1;
            if (newMemoryValue >= 0)
                virtualMachine.Memory[virtualMachine.MemoryPointer] = (byte)newMemoryValue;
            else
                virtualMachine.Memory[virtualMachine.MemoryPointer] = (byte)(newMemoryValue + 256);
        }
        /// <summary>
        /// ������������ �������� �������� �������� ��� �������, ��������������� �������� � ������� ��������� ������.
        /// </summary>
        /// <param name="virtualMachine"> ����������� ������.</param>
        /// <param name="startChar"> ��������� ������ ���������.</param>
        /// <param name="endChar"> �������� ������ ���������.</param>
        private static void RegisterSymbols(IVirtualMachine virtualMachine, char startChar, char endChar)
        {
            for (var currentChar = startChar; currentChar <= endChar; currentChar++)
            {
                RegisterSymbolCommand(virtualMachine, currentChar);
            }
        }
        /// <summary>
        /// ������������ ������ ��� �������, ������� ������������� �������� � ������� ��������� ������.
        /// </summary>
        /// <param name="virtualMachine"> ����������� ������..</param>
        /// <param name="symbol"> �������������� ������..</param>
        private static void RegisterSymbolCommand(IVirtualMachine virtualMachine, char symbol)
        {
            var charByte = (byte)symbol;
            virtualMachine.RegisterCommand(symbol, b
                => { virtualMachine.Memory[virtualMachine.MemoryPointer] = charByte; });
        }
        /// <summary>
        /// ������������ ������� ���������� � ���������� ���������.
        /// </summary>
        /// <param name="virtualMachine"> ����������� ������.</param>
        private static void RegisterPointerCommands(IVirtualMachine virtualMachine)
        {
            virtualMachine.RegisterCommand('>', b => IncrementPointer(virtualMachine));
            virtualMachine.RegisterCommand('<', b => DecrementPointer(virtualMachine));
        }
        /// <summary>
        /// ����������� ��������� ������ �� 1, ������������ � 0 ��� ���������� ����� ������� ������.
        /// </summary>
        /// <param name="virtualMachine"> ����������� ������.</param>
        private static void IncrementPointer(IVirtualMachine virtualMachine)
        {
            if (virtualMachine.MemoryPointer < virtualMachine.Memory.Length - 1)
                virtualMachine.MemoryPointer++;
            else
                virtualMachine.MemoryPointer = 0;
        }
        /// <summary>
        /// ��������� ��������� ������ �� 1, ������������ � ����� ������� ������ ��� ���������� 0.
        /// </summary>
        /// <param name="virtualMachine"> ����������� ������.</param>
        private static void DecrementPointer(IVirtualMachine virtualMachine)
        {
            if (virtualMachine.MemoryPointer > 0)
                virtualMachine.MemoryPointer--;
            else
                virtualMachine.MemoryPointer = virtualMachine.Memory.Length - 1;
        }
    }
}