using System.Collections.Generic;

namespace yield;

public static class ExpSmoothingTask
{
    public static IEnumerable<DataPoint> SmoothExponentialy(this IEnumerable<DataPoint> dataPoints, 
        double smoothingFactor)
    {
        var smoothedPreviousPoint = (DataPoint)null;
        foreach (var point in dataPoints)
        {
            smoothedPreviousPoint ??= point.WithExpSmoothedY(point.OriginalY);
            var smoothedAverageValue = smoothingFactor * point.OriginalY 
                + (1 - smoothingFactor) * smoothedPreviousPoint.ExpSmoothedY;
            smoothedPreviousPoint = point.WithExpSmoothedY(smoothedAverageValue);
            yield return smoothedPreviousPoint;
        }
    }
}
