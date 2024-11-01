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
        /// ������ ������� �������� ��������� ������� ������.
        /// </summary>
        /// <param name="extendedMatrix"> ����������� �������.</param>
        /// <param name="sourceMatrix"> �������� ������� ����� �����������.</param>
        /// <param name="sourceFreeMembers"> ������ ��������� ������ �������.</param>
        /// <returns> ������, ���������� ������� ������� ���������.</returns>
        /// <exception cref="NoSolutionException"> ���������, ����� ������� ��������� �����������.</exception>
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
        /// ��������� ������������ �������� ��� �������� ��� �������������� ������� � ����� ������� �����.
        /// </summary>
        /// <param name="matrix"> ��������������� �������.</param>
        /// <param name="numberColumns"> ���������� �������� � �������.</param>
        /// <param name="rows"> ���������� ����� � �������.</param>
        /// <param name="solutionVector"> ������ ��� �������� �������.</param>
        /// <param name="solutionColumn"> ���������� ������, �����������, ����� ������� ���� ������</param>
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
        /// ������� ������ ������� ������ ��� ������� �������.
        /// </summary>
        /// <param name="searchMatrix"> ������� ��� ������ ������ ���.</param>
        /// <param name="numberColumns"> ���������� �������� � �������.</param>
        /// <param name="rows"> ���������� ����� � �������.</param>
        /// <param name="rowUsed"> ���������� ������, �����������, ����� ������ ���� ������������.</param>
        /// <returns> ������ ������� ������ ��� -1, ���� ������� ������ �� �������.</returns>
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
        /// ����������� ������� ������ ���, ����� �� ������� ����������� ��� ����� 1.
        /// </summary>
        /// <param name="matrixNormalization"> ������� ��� ������������.</param>
        /// <param name="numberColumns"> ���������� �������� � �������.</param>
        /// <param name="rows"> ���������� ����� � �������.</param>
        /// <param name="pivotRowIndex"> ������ ������� ������.</param>
        /// <param name="column"> ������ ������� �������� ��������.</param>
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
        /// ���������, ��������� �� �������, �. �. ���������� �� ������� ������� ���������.
        /// </summary>
        /// <param name="matrixTested"> ����������� �������.</param>
        /// <param name="numberColumns"> ���������� �������� � �������.</param>
        /// <param name="rows"> ���������� ����� � �������.</param>
        /// <returns> True, ���� ������� ���������, � false � ��������� ������.</returns>
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
        /// ��������� ������ ������� �� ��������������� �������.
        /// </summary>
        /// <param name="matrix"> ��������������� �������.</param>
        /// <param name="numberColumns"> ���������� �������� � �������.</param>
        /// <param name="isSolutionColumn"> ���������� ������, �����������, ����� ������� ���� ������.</param>
        /// <param name="rows"> ���������� ����� � �������.</param>
        /// <param name="solutionVector"> ������ ��� �������� �������.</param>
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
        /// ������� ������ ������ ������ � ��������� ��������� � ������ �������.
        /// </summary>
        /// <param name="searchMatrix"> ������� ��� ������ ������ ���.</param>
        /// <param name="rows"> ���������� ����� � �������.</param>
        /// <param name="column"> ������ ������� ��� ������.</param>
        /// <returns> ������ ������ ��������� ������ ��� -1, ���� ����� ������ �� ����������.</returns>
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
        /// ��������� ����� ������� � �������.
        /// </summary>
        /// <param name="matrixArray"> �������� �������.</param>
        /// <param name="newColumn"> ����� ����������� �������.</param>
        /// <returns> ������� � ����������� ����� ��������.</returns>
        /// <exception cref="ArgumentException"> ���������, ����� ������� � ����� ������� �� ����� 
        /// ����������� ���������� �����.</exception>
        private static double[][] AddColumn(double[][] matrixArray, double[] newColumn)
        {
            if (matrixArray.Length != newColumn.Length)
                throw new ArgumentException("Matrix and new numberColumns must have the same number of rows.");

            return matrixArray.Select((row, i) => row.Concat(new[] { newColumn[i] }).ToArray()).ToArray();
        }
    }
}