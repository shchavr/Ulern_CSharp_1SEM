using System;
using System.Collections.Generic;

namespace LimitedSizeStack
{
    public class LimitedSizeStack<T>
    {
        private LinkedList<T> stackItems = new();
        private int maxStackSize;

        public LimitedSizeStack(int newSizeLimit)
        {
            maxStackSize = newSizeLimit;
        }

        public int Count => stackItems.Count;

        public void Push(T item)
        {
            stackItems.AddLast(item);
            if (stackItems.Count > maxStackSize)
                stackItems.RemoveFirst();
        }

        public T Pop()
        {
            if (stackItems.Count == 0)
                throw new InvalidOperationException();

            var lastItem = stackItems.Last;
            stackItems.RemoveLast();
            return lastItem.Value;
        }
    }
}