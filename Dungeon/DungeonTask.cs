using System;
using System.Collections.Generic;
using System.Linq;

namespace Dungeon
{
    public class DungeonTask
    {
        public static MoveDirection[] FindShortestPath(Map map)
        {
            var startingPosition = map.InitialPosition;
            var exitLocation = map.Exit;
            var treasureLocations = map.Chests;

            var pathsToExit = BfsTask.FindPaths(map, startingPosition, new Point[] { exitLocation }).FirstOrDefault();
            if (pathsToExit == null)
                return Array.Empty<MoveDirection>();
            var moveToExit = pathsToExit.ToList();
            moveToExit.Reverse();
            if (HasChestsInPath(treasureLocations, moveToExit))
                return ConvertPointsToDirection(moveToExit);

            var pathFromStartToChests = BfsTask.FindPaths(map, startingPosition, treasureLocations);
            var pathFromStartToExit = BfsTask.FindPaths(map, exitLocation, treasureLocations).DefaultIfEmpty();
            if (pathFromStartToChests.FirstOrDefault() == null)
                return ConvertPointsToDirection(moveToExit);

            var optimalPath = FindShortestPathFromStartToExit(pathFromStartToChests, pathFromStartToExit);
            return ConvertPointsToDirection(optimalPath.ToList());
        }
        /// <summary>
        /// Проверяет, содержатся ли сундуки в пути.
        /// </summary>
        /// <param name="chests">Массив координат сундуков.</param>
        /// <param name="path">Путь.</param>
        /// <returns><c>true</c>, если путь содержит сундуки, иначе <c>false</c>.</returns>
        private static bool HasChestsInPath(Point[] chests, 
            List<Point> path) => chests.Any(chest => path.Contains(chest));

        /// <summary>
        /// Находит кратчайший путь от начальной точки до выхода из подземелья, проходящий через сундуки.
        /// </summary>
        /// <param name="pathFromStartToTreasures">Пути от начальной точки до сундуков.</param>
        /// <param name="pathFromExitToTreasures">Пути от выхода до сундуков.</param>
        /// <returns>Коллекция точек, представляющих кратчайший путь.</returns>
        private static IEnumerable<Point> FindShortestPathFromStartToExit(
            IEnumerable<SinglyLinkedList<Point>> pathFromStartToTreasures,
            IEnumerable<SinglyLinkedList<Point>> pathFromExitToTreasures)
        {
            var routeStartToExit = pathFromStartToTreasures.Join(
                pathFromExitToTreasures,
                fromStartToTreasures => fromStartToTreasures.Value,
                fromExitToTreasures => fromExitToTreasures.Value,
                (fromStartToTreasures, fromExitToTreasures) => new
                {
                    Length = fromStartToTreasures.Length + fromExitToTreasures.Length,
                    ListFinish = fromStartToTreasures.ToList(),
                    ListStart = fromExitToTreasures.ToList()
                });

            var shortestRoute = routeStartToExit.OrderBy(path => path.Length).First();
            shortestRoute.ListFinish.Reverse();
            shortestRoute.ListFinish.AddRange(shortestRoute.ListStart.Skip(1));
            return shortestRoute.ListFinish;
        }
        /// <summary>
        /// Преобразует список точек в массив направлений.
        /// </summary>
        /// <param name="points">Список точек.</param>
        /// <returns>Массив направлений.</returns>
        private static MoveDirection[] ConvertPointsToDirection(List<Point> points)
        {
            return points
                .Take(points.Count - 1)
                .Select((currentPoint, index) =>
                {
                    var nextPoint = points[index + 1];
                    return CalculateDirection(currentPoint, nextPoint);
                })
                .ToArray();
        }

        /// <summary>
        /// Вычисляет направление движения между двумя точками.
        /// </summary>
        /// <param name="currentPoint">Текущая точка.</param>
        /// <param name="nextPoint">Следующая точка.</param>
        /// <returns>Направление движения.</returns>
        private static MoveDirection CalculateDirection(Point currentPoint, Point nextPoint)
        {
            var offset = new Point(currentPoint.X - nextPoint.X, currentPoint.Y - nextPoint.Y);
            if (offset.X == 1)
                return MoveDirection.Left;
            if (offset.X == -1)
                return MoveDirection.Right;
            if (offset.Y == 1)
                return MoveDirection.Up;
            if (offset.Y == -1)
                return MoveDirection.Down;
            throw new ArgumentException();
        }
    }
}