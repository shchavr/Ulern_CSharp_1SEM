using System.Collections.Generic;
using System.Linq;

namespace rocket_bot;

public class Channel<T> where T : class
{
    /// <summary>
    /// Возвращает элемент по индексу или null, если такого элемента нет.
    /// При присвоении удаляет все элементы после.
    /// Если индекс в точности равен размеру коллекции, работает как Append.
    /// </summary>
    private readonly object blockingObjects = new();
    private List<T> threadSafeElementsList = new();

    public T this[int index]
    {
        get
        {
            lock (blockingObjects)
            {
                return (index > threadSafeElementsList.Count - 1 
                    || threadSafeElementsList.Count == 0) ? null : threadSafeElementsList[index];
            }
        }

        set
        {
            lock (blockingObjects)
            {
                int safeCount = threadSafeElementsList.Count;
                if (index == safeCount)
                    threadSafeElementsList.Add(value);
                else if (index < safeCount)
                {
                    threadSafeElementsList.RemoveRange(index, safeCount - index);
                    threadSafeElementsList.Add(value);
                }
            }
        }
    }


    /// <summary>
    /// Возвращает последний элемент или null, если такого элемента нет
    /// </summary>
    public T LastItem()
    {
        lock (blockingObjects)
        {
            if (threadSafeElementsList.Count == 0)
                return null;
            return threadSafeElementsList.Last();
        }
    }

    /// <summary>
    /// Добавляет item в конец только если lastItem является последним элементом
    /// </summary>
    public void AppendIfLastItemIsUnchanged(T item, T knownLastItem)
    {
        lock (blockingObjects)
        {
            if (threadSafeElementsList.Count != 0 
                && knownLastItem == threadSafeElementsList.Last() 
                || threadSafeElementsList.Count == 0)
                threadSafeElementsList.Add(item);
        }
    }

    /// <summary>
    /// Возвращает количество элементов в коллекции
    /// </summary>
    public int Count
    {
        get
        {
            lock (blockingObjects)
            {
                return threadSafeElementsList.Count;
            }
        }
    }
}