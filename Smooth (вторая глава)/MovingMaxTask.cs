using System;
using System.Collections.Generic;
using System.Linq;

namespace yield;

public static class MovingMaxTask
{
    public static IEnumerable<DataPoint> MovingMax(this IEnumerable<DataPoint> dataPoints, int maxWindowSize)
    {
        var slidingWindow = new Queue<DataPoint>();
        var potentialMaximums = new LinkedList<DataPoint>();
        foreach (var currentDataPoint in dataPoints)
        {
            var currentPoint = new DataPoint(currentDataPoint);
            ProcessSlidingWindow(slidingWindow, potentialMaximums, currentPoint, maxWindowSize);
            yield return currentPoint.WithMaxY(potentialMaximums.First.Value.OriginalY);
        }
    }
    /// <summary>
    /// ќбрабатывает скольз€щее окно, добавл€€ текущую точку в 
    /// скольз€щее окно и обновл€€ список потенциальных максимумов.
    /// </summary>
    /// <param name="slidingWindow">ќчередь скольз€щего окна..</param>
    /// <param name="potentialMaximums">—писок потенциальных максимумов..</param>
    /// <param name="currentPoint">“екуща€ точка данных.</param>
    /// <param name="maxWindowSize">ћаксимальный размер скольз€щего окна.</param>
    private static void ProcessSlidingWindow(Queue<DataPoint> slidingWindow,
        LinkedList<DataPoint> potentialMaximums, DataPoint currentPoint, int maxWindowSize)
    {
        if (slidingWindow.Count == maxWindowSize)
        {
            var removedPoint = slidingWindow.Dequeue();
            if (potentialMaximums.Contains(removedPoint))
                potentialMaximums.Remove(removedPoint);
        }
        slidingWindow.Enqueue(currentPoint);
        RemovePotentialMaxValues(potentialMaximums, currentPoint);
        potentialMaximums.AddLast(currentPoint);
    }
    /// <summary>
    /// ”дал€ет из списка потенциальные максимумы, которые больше текущей точки.
    /// </summary>
    /// <param name="potentialMaximums">—писок потенциальных максимумов.</param>
    /// <param name="currentPoint">“екуща€ точка данных.</param>
    private static void RemovePotentialMaxValues(LinkedList<DataPoint> potentialMaximums,
        DataPoint currentPoint)
    {
        while (potentialMaximums.Count != 0 
            && currentPoint.OriginalY > potentialMaximums.Last.Value.OriginalY)
            potentialMaximums.RemoveLast();
    }
}