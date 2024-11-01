using System;
using System.Collections;
using System.Collections.Generic;

namespace hashes
{
    public class ReadonlyBytes : IEnumerable<byte>
    {
        private readonly byte[] values;
        private readonly int hash;

        public int Length => values.Length;

        public byte this[int arrayIndex]
        {
            get
            {
                if (arrayIndex >= values.Length)
                    throw new IndexOutOfRangeException();
                
                return values[arrayIndex];
            }
        }

        public ReadonlyBytes(params byte[] inputValues)
        {
            if (inputValues == null)
                throw new ArgumentNullException();
            
            values = inputValues;
            hash = CalculateFNVHash();
        }

        public IEnumerator<byte> GetEnumerator() => ((IEnumerable<byte>)values).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public override int GetHashCode() => hash;

        public override bool Equals(object otherObject)
        {
            if (otherObject == null || GetType() != otherObject.GetType())
                return false;
            
            return Equals((ReadonlyBytes)otherObject);
        }

        public override string ToString() => "[" + string.Join(", ", values) + "]";

        /// <summary>
        /// Определяет, равен ли текущий объект "ReadonlyBytes" другому объекту "ReadonlyBytes".
        /// </summary>
        /// <param name="otherReadonlyBytes">Другой объект "ReadonlyBytes" для сравнения.</param>
        /// <returns>True, если объекты равны; в противном случае - false.</returns>
        private bool Equals(ReadonlyBytes otherReadonlyBytes)
        {
            if (otherReadonlyBytes.Length != Length)
                return false;
            

            for (var currentIndex = 0; currentIndex < Length; currentIndex++)
            {
                if (otherReadonlyBytes[currentIndex] != values[currentIndex])
                    return false;
                
            }

            return true;
        }

        /// <summary>
        /// Вычисляет хэш FNV (Fowler-Noll-Vo) для объекта "ReadonlyBytes".
        /// </summary>
        /// <returns>Вычисленное значение хэша FNV.</returns>
        private int CalculateFNVHash()
        {
            const int fnvPrimeNumber = 16777619;
            var calculatedHash = 2166136261;

            foreach (var byteValue in values)
            {
                unchecked
                {
                    calculatedHash = (calculatedHash ^ byteValue) * fnvPrimeNumber;
                }
            }
            return (int)calculatedHash;
        }
    }
}