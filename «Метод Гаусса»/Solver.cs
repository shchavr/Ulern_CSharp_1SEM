using System;
using System.Linq;

namespace GaussAlgorithm
{
    public class Solver
    {
        public double[] Solve(double[][] matrix, double[] freeMembers)
        {
            if (matrix == null || freeMembers == null)
                throw new ArgumentNullException("Matrix and free members cannot be null.");

            if (matrix.Length != freeMembers.Length)
                throw new ArgumentException("Matrix and free members must have the same number of rows.");

            matrix = AddColumn(matrix, freeMembers);

            return GaussSolve(matrix, matrix, freeMembers);
        }
        /// <summary>
        /// Решает систему линейных уравнений методом Гаусса.
        /// </summary>
        /// <param name="extendedMatrix"> Расширенная матрица.</param>
        /// <param name="sourceMatrix"> Исходная матрица перед увеличением.</param>
        /// <param name="sourceFreeMembers"> Вектор свободных членов системы.</param>
        /// <returns> Вектор, содержащий решение системы уравнений.</returns>
        /// <exception cref="NoSolutionException"> Возникает, когда система уравнений неразрешима.</exception>
        private static double[] GaussSolve(double[][] extendedMatrix, 
            double[][] sourceMatrix, double[] sourceFreeMembers)
        {
            var rowCount = extendedMatrix.Length;
            var columnCount = extendedMatrix[0].Length;

            var solutionVector = new double[columnCount - 1];
            var isSolutionColumn = new bool[columnCount - 1];
            PerformElementaryTransforms(extendedMatrix, rowCount, columnCount, solutionVector, isSolutionColumn);

            if (!IsMatrixResolvable(extendedMatrix, rowCount, columnCount))
                throw new NoSolutionException("The system of equations is not solvable.");

            ExtractSolution(extendedMatrix, columnCount, isSolutionColumn, rowCount, solutionVector);

            return solutionVector;
        }
        /// <summary>
        /// Выполняет элементарные операции над строками для преобразования матрицы в форму эшелона строк.
        /// </summary>
        /// <param name="matrix"> Преобразованная матрица.</param>
        /// <param name="numberColumns"> Количество столбцов в матрице.</param>
        /// <param name="rows"> Количество строк в матрице.</param>
        /// <param name="solutionVector"> Вектор для хранения решения.</param>
        /// <param name="solutionColumn"> Логический массив, указывающий, какие столбцы были решены</param>
        private static void PerformElementaryTransforms(double[][] matrix, int rows, 
            int numberColumns, double[] solutionVector, bool[] solutionColumn)
        {
            var isRowUsed = new bool[rows];

            for (var columnIndex = 0; columnIndex < numberColumns - 1; columnIndex++)
            {
                var pivotRowIndex = FindPivotRowIndex(matrix, rows, columnIndex, isRowUsed);

                if (pivotRowIndex == -1)
                {
                    solutionVector[columnIndex] = 0;
                    solutionColumn[columnIndex] = true;
                    continue;
                }

                NormalizePivotRow(matrix, rows, numberColumns, pivotRowIndex, columnIndex);
            }
        }
        /// <summary>
        /// Находит индекс сводной строки для данного столбца.
        /// </summary>
        /// <param name="searchMatrix"> Матрица для поиска внутри нее.</param>
        /// <param name="numberColumns"> Количество столбцов в матрице.</param>
        /// <param name="rows"> Количество строк в матрице.</param>
        /// <param name="rowUsed"> Логический массив, указывающий, какие строки были использованы.</param>
        /// <returns> Индекс сводной строки или -1, если сводная строка не найдена.</returns>
        private static int FindPivotRowIndex(double[][] searchMatrix, int rows, int numberColumns, bool[] rowUsed)
        {
            for (var rowIndex = 0; rowIndex < rows; rowIndex++)
            {
                if (searchMatrix[rowIndex][numberColumns] != 0 && !rowUsed[rowIndex])
                {
                    rowUsed[rowIndex] = true;
                    return rowIndex;
                }
            }
            return -1;
        }
        /// <summary>
        /// Нормализует сводную строку так, чтобы ее ведущий коэффициент был равен 1.
        /// </summary>
        /// <param name="matrixNormalization"> Матрица для нормализации.</param>
        /// <param name="numberColumns"> Количество столбцов в матрице.</param>
        /// <param name="rows"> Количество строк в матрице.</param>
        /// <param name="pivotRowIndex"> Индекс сводной строки.</param>
        /// <param name="column"> Индекс столбца сводного элемента.</param>
        private static void NormalizePivotRow(double[][] matrixNormalization, int rows, 
            int numberColumns, int pivotRowIndex, int column)
        {
            for (var currentRowIndex = 0; currentRowIndex < rows; currentRowIndex++)
            {
                if (currentRowIndex == pivotRowIndex) continue;

                var coefficient = matrixNormalization[currentRowIndex][column] 
                    / matrixNormalization[pivotRowIndex][column];

                for (var columnIndex = 0; columnIndex < numberColumns; columnIndex++)
                    matrixNormalization[currentRowIndex][columnIndex] 
                        -= matrixNormalization[pivotRowIndex][columnIndex] * coefficient;
            }
        }
        /// <summary>
        /// Проверяет, разрешима ли матрица, т. е. существует ли решение системы уравнений.
        /// </summary>
        /// <param name="matrixTested"> Проверяемая матрица.</param>
        /// <param name="numberColumns"> Количество столбцов в матрице.</param>
        /// <param name="rows"> Количество строк в матрице.</param>
        /// <returns> True, если матрица разрешима, и false в противном случае.</returns>
        private static bool IsMatrixResolvable(double[][] matrixTested, int rows, int numberColumns)
        {
            for (var currentRowIndex = 0; currentRowIndex < rows; currentRowIndex++)
            {
                if (matrixTested[currentRowIndex][numberColumns - 1] == 0) continue;
                var hasNonZeroElement = false;
                for (var columnIndex = 0; columnIndex < numberColumns - 1; columnIndex++)
                {
                    if (matrixTested[currentRowIndex][columnIndex] != 0)
                    {
                        hasNonZeroElement = true;
                        break;
                    }
                }
                if (!hasNonZeroElement) return false;
            }
            return true;
        }
        /// <summary>
        /// Извлекает вектор решения из преобразованной матрицы.
        /// </summary>
        /// <param name="matrix"> Преобразованная матрица.</param>
        /// <param name="numberColumns"> Количество столбцов в матрице.</param>
        /// <param name="isSolutionColumn"> Логический массив, указывающий, какие столбцы были решены.</param>
        /// <param name="rows"> Количество строк в матрице.</param>
        /// <param name="solutionVector"> Вектор для хранения решения.</param>
        private static void ExtractSolution(double[][] matrix, int numberColumns, 
            bool[] isSolutionColumn, int rows, double[] solutionVector)
        {
            for (var columnIndex = 0; columnIndex < numberColumns - 1; columnIndex++)
            {
                if (isSolutionColumn[columnIndex]) continue;
                var nonZeroRowIndex = FindNonZeroRowIndex(matrix, rows, columnIndex);
                solutionVector[columnIndex] = nonZeroRowIndex == -1 ? 0 : matrix[nonZeroRowIndex][numberColumns - 1] 
                    / matrix[nonZeroRowIndex][columnIndex];
            }
        }
        /// <summary>
        /// Находит индекс первой строки с ненулевым значением в данном столбце.
        /// </summary>
        /// <param name="searchMatrix"> Матрица для поиска внутри нее.</param>
        /// <param name="rows"> Количество строк в матрице.</param>
        /// <param name="column"> Индекс столбца для поиска.</param>
        /// <returns> Индекс первой ненулевой строки или -1, если такой строки не существует.</returns>
        private static int FindNonZeroRowIndex(double[][] searchMatrix, int rows, int column)
        {
            for (var currentRowIndex = 0; currentRowIndex < rows; currentRowIndex++)
            {
                if (Math.Abs(searchMatrix[currentRowIndex][column]) > 1e-5) 
                    return currentRowIndex;
            }
            return -1;
        }
        /// <summary>
        /// Добавляет новый столбец в матрицу.
        /// </summary>
        /// <param name="matrixArray"> Исходная матрица.</param>
        /// <param name="newColumn"> Новый добавляемый столбец.</param>
        /// <returns> Матрицу с добавленным новым столбцом.</returns>
        /// <exception cref="ArgumentException"> Возникает, когда матрица и новый столбец не имеют 
        /// одинакового количества строк.</exception>
        private static double[][] AddColumn(double[][] matrixArray, double[] newColumn)
        {
            if (matrixArray.Length != newColumn.Length)
                throw new ArgumentException("Matrix and new numberColumns must have the same number of rows.");

            return matrixArray.Select((row, i) => row.Concat(new[] { newColumn[i] }).ToArray()).ToArray();
        }
    }
}