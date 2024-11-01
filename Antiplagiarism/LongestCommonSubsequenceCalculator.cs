using System;
using System.Collections.Generic;
using System.Linq;

namespace Antiplagiarism;

public static class LongestCommonSubsequenceCalculator
{
    public static List<string> Calculate(List<string> firstList, List<string> secondList)
    {
        var optimizationTable = CreateOptimizationTable(firstList, secondList);
        return RestoreAnswer(optimizationTable, firstList, secondList);
    }
    /// <summary>
    /// Создает таблицу оптимизации для заданных списков строк.
    /// </summary>
    /// <param name="firstList">Первый список строк.</param>
    /// <param name="secondList">Второй список строк.</param>
    /// <returns>Таблица оптимизации в виде двумерного массива.</returns>
    private static int[,] CreateOptimizationTable(List<string> firstList, List<string> secondList)
    {
        var optimizationTable = InitializeOptimizationArray(firstList.Count, secondList.Count);

        for (var firstIndex = 1; firstIndex <= firstList.Count; ++firstIndex)
        {
            for (var secondIndex = 1; secondIndex <= secondList.Count; ++secondIndex)
            {
                if (firstList[firstIndex - 1] == secondList[secondIndex - 1])
                    optimizationTable[firstIndex, secondIndex] = 1
                        + optimizationTable[firstIndex - 1, secondIndex - 1];
                else
                    optimizationTable[firstIndex, secondIndex] = Math.Max(optimizationTable[firstIndex - 1,
                        secondIndex], optimizationTable[firstIndex, secondIndex - 1]);
            }
        }

        return optimizationTable;
    }
    /// <summary>
    /// Инициализирует двумерный массив оптимизации заданного размера.
    /// </summary>
    /// <param name="rowCount">Количество строк массива.</param>
    /// <param name="colCount">Количество столбцов массива.</param>
    /// <returns>Инициализированный двумерный массив оптимизации.</returns>
    private static int[,] InitializeOptimizationArray(int rowCount, int colCount)
    {
        var optimizationArray = new int[rowCount + 1, colCount + 1];
        return optimizationArray;
    }
    /// <summary>
    /// Восстанавливает ответ на основе таблицы оптимизации и заданных списков строк.
    /// </summary>
    /// <param name="optimizationTable">Таблица оптимизации.</param>
    /// <param name="firstList">Первый список строк.</param>
    /// <param name="secondList">Второй список строк.</param>
    /// <returns>Список строк - восстановленный ответ.</returns>
    private static List<string> RestoreAnswer(int[,] optimizationTable, List<string> firstList, List<string> secondList)
    {
        var firstListCount = firstList.Count;
        var secondListCount = secondList.Count;
        if (firstListCount == 0 || secondListCount == 0)
            return new List<string>();
        if (firstList[firstListCount - 1] == secondList[secondListCount - 1])
        {
            var result = RestoreAnswer(optimizationTable, firstList.GetRange(0, firstListCount - 1),
                secondList.GetRange(0, secondListCount - 1));
            result.Add(firstList[firstListCount - 1]);
            return result;
        }
        return optimizationTable[firstListCount, secondListCount - 1]
            > optimizationTable[firstListCount - 1, secondListCount]
                ? RestoreAnswer(optimizationTable, firstList, secondList.GetRange(0, secondListCount - 1))
                : RestoreAnswer(optimizationTable, firstList.GetRange(0, firstListCount - 1), secondList);
    }
}

