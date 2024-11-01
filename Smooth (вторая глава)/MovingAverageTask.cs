using System.Collections.Generic;

namespace yield;

public static class MovingAverageTask
{
    public static IEnumerable<DataPoint> MovingAverage(this IEnumerable<DataPoint> dataPoints, 
        int movingWindowSize)
    {
        var movingQueue = new Queue<DataPoint>();
        var sumValues = 0.0;
        foreach (var currentDataPoint in dataPoints)
        {
            var currentPoint = new DataPoint(currentDataPoint);
            movingQueue.Enqueue(currentPoint);
            sumValues += currentDataPoint.OriginalY;
            yield return currentDataPoint.WithAvgSmoothedY(sumValues / movingQueue.Count);

            if (movingQueue.Count == movingWindowSize) 
                sumValues -= movingQueue.Dequeue().OriginalY;
        }
    }
}