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
        /// Увеличивает значение текущего указателя памяти на 1, оборачивая его до 0 при достижении 256.
        /// </summary>
        /// <param name="virtualMachine"> Виртуальная машина.</param>
        private static void IncrementMemory(IVirtualMachine virtualMachine)
        {
            virtualMachine.Memory[virtualMachine.MemoryPointer] =
                (byte)((virtualMachine.Memory[virtualMachine.MemoryPointer] + 1) % 256);
        }
        /// <summary>
        /// Уменьшает значение текущего указателя памяти на 1, оборачивая его до 255 при достижении 0
        /// </summary>
        /// <param name="virtualMachine"> Виртуальная машина.</param>
        private static void DecrementMemory(IVirtualMachine virtualMachine)
        {
            var newMemoryValue = virtualMachine.Memory[virtualMachine.MemoryPointer] - 1;
            if (newMemoryValue >= 0)
                virtualMachine.Memory[virtualMachine.MemoryPointer] = (byte)newMemoryValue;
            else
                virtualMachine.Memory[virtualMachine.MemoryPointer] = (byte)(newMemoryValue + 256);
        }
        /// <summary>
        /// Регистрирует заданный диапазон символов как команды, устанавливающие значение в текущем указателе памяти.
        /// </summary>
        /// <param name="virtualMachine"> Виртуальная машина.</param>
        /// <param name="startChar"> Начальный символ диапазона.</param>
        /// <param name="endChar"> Конечный символ диапазона.</param>
        private static void RegisterSymbols(IVirtualMachine virtualMachine, char startChar, char endChar)
        {
            for (var currentChar = startChar; currentChar <= endChar; currentChar++)
            {
                RegisterSymbolCommand(virtualMachine, currentChar);
            }
        }
        /// <summary>
        /// Регистрирует символ как команду, которая устанавливает значение в текущем указателе памяти.
        /// </summary>
        /// <param name="virtualMachine"> Виртуальная машина..</param>
        /// <param name="symbol"> Регистрируемый символ..</param>
        private static void RegisterSymbolCommand(IVirtualMachine virtualMachine, char symbol)
        {
            var charByte = (byte)symbol;
            virtualMachine.RegisterCommand(symbol, b
                => { virtualMachine.Memory[virtualMachine.MemoryPointer] = charByte; });
        }
        /// <summary>
        /// Регистрирует команды инкремента и декремента указателя.
        /// </summary>
        /// <param name="virtualMachine"> Виртуальная машина.</param>
        private static void RegisterPointerCommands(IVirtualMachine virtualMachine)
        {
            virtualMachine.RegisterCommand('>', b => IncrementPointer(virtualMachine));
            virtualMachine.RegisterCommand('<', b => DecrementPointer(virtualMachine));
        }
        /// <summary>
        /// Увеличивает указатель памяти на 1, оборачиваясь к 0 при достижении конца массива памяти.
        /// </summary>
        /// <param name="virtualMachine"> Виртуальная машина.</param>
        private static void IncrementPointer(IVirtualMachine virtualMachine)
        {
            if (virtualMachine.MemoryPointer < virtualMachine.Memory.Length - 1)
                virtualMachine.MemoryPointer++;
            else
                virtualMachine.MemoryPointer = 0;
        }
        /// <summary>
        /// Уменьшает указатель памяти на 1, оборачиваясь к концу массива памяти при достижении 0.
        /// </summary>
        /// <param name="virtualMachine"> Виртуальная машина.</param>
        private static void DecrementPointer(IVirtualMachine virtualMachine)
        {
            if (virtualMachine.MemoryPointer > 0)
                virtualMachine.MemoryPointer--;
            else
                virtualMachine.MemoryPointer = virtualMachine.Memory.Length - 1;
        }
    }
}