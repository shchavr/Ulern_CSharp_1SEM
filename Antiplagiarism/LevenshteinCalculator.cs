using System;
using System.Collections.Generic;
using System.Linq;

// Каждый документ — это список токенов. То есть List<string>.
// Вместо этого будем использовать псевдоним DocumentTokens.
// Это поможет избежать сложных конструкций:
// вместо List<List<string>> будет List<DocumentTokens>
using DocumentTokens = System.Collections.Generic.List<string>;

namespace Antiplagiarism;

public class LevenshteinCalculator
{
    public List<ComparisonResult> CompareDocumentsPairwise(List<DocumentTokens> inputDocuments)
    {
        var comparisonResults = new List<ComparisonResult>();
        for (var currentIndex = 0; currentIndex < inputDocuments.Count; currentIndex++)
        for (var nextIndex = currentIndex + 1; nextIndex < inputDocuments.Count; nextIndex++)
            comparisonResults.Add(CompareDocuments(inputDocuments[currentIndex], inputDocuments[nextIndex]));
        return comparisonResults;
    }
    /// <summary>
    /// Метод для инициализации массива с заданным размером.
    /// </summary>
    /// <param name="size">Размер массива.</param>
    /// <returns>Инициализированный массив с числами от 0 до указанного размера.</returns>
    private double[] InitializeArray(int size)
    {
        var initializedArray = new double[size + 1];
        for (var index = 0; index <= size; ++index)
            initializedArray[index] = index;
        return initializedArray;
    }
    /// <summary>
    /// Метод для вычисления расстояния между двумя токенами.
    /// </summary>
    /// <param name="firstToken">Первый токен.</param>
    /// <param name="secondToken">Второй токен.</param>
    /// <returns>Расстояние между указанными токенами.</returns>
    private double CalculateTokenDistance(string firstToken, string secondToken)
    {
        return TokenDistanceCalculator.GetTokenDistance(firstToken, secondToken);
    }
    /// <summary>
    /// Метод для вычисления сравнения между двумя документами с помощью алгоритма Дамерау-Левенштейна.
    /// </summary>
    /// <param name="firstDocument">Первый документ.</param>
    /// <param name="secondDocument">Второй документ.</param>
    /// <returns>Массив значений, представляющих результаты сравнения между документами.</returns>
    private double[] ComputeComparison(DocumentTokens firstDocument, DocumentTokens secondDocument)
    {
        var previousRow = InitializeArray(secondDocument.Count);
        var currentRow = new double[secondDocument.Count + 1];

        for (var firstIndex = 1; firstIndex <= firstDocument.Count; firstIndex++)
        {
            currentRow[0] = firstIndex;

            for (var secondIndex = 1; secondIndex <= secondDocument.Count; secondIndex++)
            {
                if (firstDocument[firstIndex - 1] == secondDocument[secondIndex - 1])
                    currentRow[secondIndex] = previousRow[secondIndex - 1];
                else
                {
                    var tokenDistance = CalculateTokenDistance(firstDocument[firstIndex - 1], 
                        secondDocument[secondIndex - 1]);
                    currentRow[secondIndex] = new List<double> 
                    { 
                        1 + currentRow[secondIndex - 1], tokenDistance + previousRow[secondIndex - 1], 
                        1 + previousRow[secondIndex] 
                    }.Min();
                }
            }

            Array.Copy(currentRow, previousRow, secondDocument.Count + 1);
        }

        return previousRow;
    }
    /// <summary>
    /// Метод для сравнения двух документов и возврата результата сравнения.
    /// </summary>
    /// <param name="firstDocument">Первый документ.</param>
    /// <param name="secondDocument">Второй документ.</param>
    /// <returns>Результат сравнения двух документов.</returns>
    private ComparisonResult CompareDocuments(DocumentTokens firstDocument, DocumentTokens secondDocument)
    {
        var comparisonResultArray = ComputeComparison(firstDocument, secondDocument);
        var finalComparisonResult = comparisonResultArray[secondDocument.Count];
        return new ComparisonResult(firstDocument, secondDocument, finalComparisonResult);
    }
}