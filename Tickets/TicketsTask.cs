using System.Numerics;

namespace Tickets
{
    internal class TicketsTask
    {
        public static BigInteger Solve(int halfLength, int totalSum)
        {
            if (totalSum % 2 != 0)
                return 0;
            var optimizationTable = FillOptTable(halfLength, totalSum / 2);
            var halfResult = CountHappyTickets(optimizationTable, halfLength, totalSum / 2);
            return halfResult * halfResult;
        }
        /// <summary>
        /// Заполняет таблицу оптимизации для заданных параметров половины длины и половины суммы.
        /// </summary>
        /// <param name="halfLen">Половина длины.</param>
        /// <param name="halfSum">Половина суммы.</param>
        /// <returns>Таблица оптимизации в виде двумерного массива BigInteger.</returns>
        private static BigInteger[,] FillOptTable(int halfLen, int halfSum)
        {
            var optimizationTable = new BigInteger[halfLen + 1, halfSum + 1];
            for (var lengthIndex = 0; lengthIndex <= halfLen; lengthIndex++)
                for (var sumIndex = 0; sumIndex <= halfSum; sumIndex++)
                    optimizationTable[lengthIndex, sumIndex] = -1;
            return optimizationTable;
        }
        /// <summary>
        /// Вычисляет количество "счастливых" билетов для заданных параметров длины, 
        /// половины суммы и таблицы оптимизации.
        /// </summary>
        /// <param name="optimizationTable">Таблица оптимизации.</param>
        /// <param name="length">Длина билета.</param>
        /// <param name="halfSum">Половина суммы.</param>
        /// <returns>Количество "счастливых" билетов в виде BigInteger.</returns>
        private static BigInteger CountHappyTickets(BigInteger[,] optimizationTable, int length, int halfSum)
        {
            if (optimizationTable[length, halfSum] >= 0)
                return optimizationTable[length, halfSum];

            if (halfSum == 0)
                return 1;

            if (length == 0)
                return 0;

            optimizationTable[length, halfSum] = 0;

            optimizationTable[length, halfSum] = CountHappyTicketsRecursive(optimizationTable, length, halfSum);

            return optimizationTable[length, halfSum];
        }
        /// <summary>
        /// Рекурсивно вычисляет количество "счастливых" билетов для заданных параметров длины, 
        /// половины суммы и таблицы оптимизации.
        /// </summary>
        /// <param name="optimizationTable">Таблица оптимизации.</param>
        /// <param name="length">Длина билета.</param>
        /// <param name="halfSum">Половина суммы.</param>
        /// <returns>Количество "счастливых" билетов в виде BigInteger.</returns>
        private static BigInteger CountHappyTicketsRecursive(BigInteger[,] optimizationTable, int length, int halfSum)
        {
            var count = new BigInteger();
            for (var ticketValue = 0; ticketValue < 10; ticketValue++)
            {
                if (halfSum - ticketValue >= 0)
                    count += CountHappyTickets(optimizationTable, length - 1, halfSum - ticketValue);    
            }
            return count;
        }
    }
}
