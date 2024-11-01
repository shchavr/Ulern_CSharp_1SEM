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
    /// ������������ ���������� ����, �������� ������� ����� � 
    /// ���������� ���� � �������� ������ ������������� ����������.
    /// </summary>
    /// <param name="slidingWindow">������� ����������� ����..</param>
    /// <param name="potentialMaximums">������ ������������� ����������..</param>
    /// <param name="currentPoint">������� ����� ������.</param>
    /// <param name="maxWindowSize">������������ ������ ����������� ����.</param>
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
    /// ������� �� ������ ������������� ���������, ������� ������ ������� �����.
    /// </summary>
    /// <param name="potentialMaximums">������ ������������� ����������.</param>
    /// <param name="currentPoint">������� ����� ������.</param>
    private static void RemovePotentialMaxValues(LinkedList<DataPoint> potentialMaximums,
        DataPoint currentPoint)
    {
        while (potentialMaximums.Count != 0 
            && currentPoint.OriginalY > potentialMaximums.Last.Value.OriginalY)
            potentialMaximums.RemoveLast();
    }
}