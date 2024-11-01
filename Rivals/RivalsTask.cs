using System.Collections.Generic;
using System.Linq;

namespace Rivals
{
    public class RivalsTask
    {
        public static IEnumerable<OwnedLocation> AssignOwners(Map map)
        {
            var locationQueue = new Queue<OwnedLocation>();
            var visitedPoints = new HashSet<Point>();
            for (var playerIndex = 0; playerIndex < map.Players.Length; playerIndex++)
            {
                visitedPoints.Add(map.Players[playerIndex]);
                locationQueue.Enqueue(new OwnedLocation(playerIndex, map.Players[playerIndex], 0));
            }

            while (locationQueue.Count != 0)
            {
                var currentLocation = locationQueue.Dequeue();
                yield return currentLocation;

                var adjacentLocations = GetIncidentNodes(currentLocation, map);

                EnqueueUnvisitedNodes(locationQueue, visitedPoints, currentLocation, adjacentLocations);
            }
        }
        /// <summary>
        /// Возвращает коллекцию точек, смежных с указанным местоположением.
        /// </summary>
        /// <param name="currentLocation">Текущее местоположение.</param>
        /// <param name="map">Карта, на которой расположены точки.</param>
        /// <returns>Коллекция точек, смежных с указанным местоположением.</returns>
        private static IEnumerable<Point> GetIncidentNodes(OwnedLocation currentLocation, Map map)
        {
            return new[]
            {
                new Point(currentLocation.Location.X, currentLocation.Location.Y + 1),
                new Point(currentLocation.Location.X, currentLocation.Location.Y - 1),
                new Point(currentLocation.Location.X + 1, currentLocation.Location.Y),
                new Point(currentLocation.Location.X - 1, currentLocation.Location.Y)
            }.Where(point => map.InBounds(point) && map.Maze[point.X, point.Y] != MapCell.Wall);
        }
        /// <summary>
        /// Добавляет в очередь непосещенные точки смежные с указанным местоположением.
        /// </summary>
        /// <param name="queue">Очередь местоположений.</param>
        /// <param name="visitedPoints">Множество посещенных точек.</param>
        /// <param name="currentLocation">Текущее местоположение.</param>
        /// <param name="pointsToCheck">Коллекция точек для проверки.</param>
        private static void EnqueueUnvisitedNodes(Queue<OwnedLocation> queue, HashSet<Point> visitedPoints,
            OwnedLocation currentLocation, IEnumerable<Point> pointsToCheck)
        {
            foreach (var point in pointsToCheck)
            {
                if (visitedPoints.Contains(point))
                    continue;
                visitedPoints.Add(point);
                queue.Enqueue(new OwnedLocation(currentLocation.Owner, point, currentLocation.Distance + 1));
            }
        }
    }
}