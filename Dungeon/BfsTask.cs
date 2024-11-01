using System.Collections.Generic;
using System.Linq;

namespace Dungeon
{
    public class BfsTask
    {
        public static IEnumerable<SinglyLinkedList<Point>> FindPaths(Map map, 
            Point startingPoint, Point[] treasureLocations)
        {
            var pathMap = new Dictionary<Point, SinglyLinkedList<Point>>();
            var visitedPoints = new HashSet<Point>();
            var pointQueue = new Queue<Point>();
            var treasureSet = new HashSet<Point>(treasureLocations);
            EnqueueStartPoint(pointQueue, startingPoint);
            visitedPoints.Add(startingPoint);
            pathMap.Add(startingPoint, new SinglyLinkedList<Point>(startingPoint));
            while (pointQueue.Count != 0)
            {
                var currentPoint = DequeuePoint(pointQueue);
                if (!IsInBounds(map, currentPoint) || IsWall(map, currentPoint))
                    continue;
                var adjacentPoints = GetPossibleDirections(currentPoint);
                EnqueueUnvisitedNodes(pointQueue, visitedPoints, adjacentPoints);
                UpdateRoad(pathMap, currentPoint, adjacentPoints);
                if (!ContainChest(treasureSet, currentPoint) || !ContainRoad(pathMap, currentPoint)) 
                    continue;
                RemoveChest(treasureSet, currentPoint);
                yield return GetRoad(pathMap, currentPoint);
            }
        }
        /// <summary>
        /// Добавляет начальную точку в очередь.
        /// </summary>
        /// <param name="queue">Очередь точек.</param>
        /// <param name="start">Начальная точка.</param>
        private static void EnqueueStartPoint(Queue<Point> queue, Point start) => queue.Enqueue(start);
        // <summary>
        /// Удаляет и возвращает следующую точку из очереди.
        /// </summary>
        /// <param name="queue">Очередь точек.</param>
        /// <returns>Следующая точка из очереди.</returns>
        private static Point DequeuePoint(Queue<Point> queue) => queue.Dequeue();
        /// <summary>
        /// Проверяет, находится ли точка в границах карты.
        /// </summary>
        /// <param name="map">Карта подземелья.</param>
        /// <param name="point">Точка для проверки.</param>
        /// <returns><c>true</c>, если точка находится в границах карты, иначе <c>false</c>.</returns>
        private static bool IsInBounds(Map map, Point point) => map.InBounds(point);
        /// <summary>
        /// Проверяет, является ли точка стеной на карте.
        /// </summary>
        /// <param name="map">Карта подземелья.</param>
        /// <param name="point">Точка для проверки.</param>
        /// <returns><c>true</c>, если точка является стеной, иначе <c>false</c>.</returns>
        private static bool IsWall(Map map, Point point) => map.Dungeon[point.X, point.Y] == MapCell.Wall;
        /// <summary>
        /// Возвращает коллекцию соседних точек для указанной точки.
        /// </summary>
        /// <param name="currentPoint">Текущая точка.</param>
        /// <returns>Коллекция соседних точек для указанной точки.</returns>
        private static IEnumerable<Point> GetPossibleDirections(Point currentPoint)
        {
            for (var offsetX = -1; offsetX <= 1; offsetX++)
            {
                for (var offsetY = -1; offsetY <= 1; offsetY++)
                {
                    if ((offsetX + offsetY) % 2 == 0)
                        continue;

                    var adjacentPoint = new Point
                    {
                        X = currentPoint.X + offsetX,
                        Y = currentPoint.Y + offsetY
                    };
                    yield return adjacentPoint;
                }
            }
        }
        /// <summary>
        /// Добавляет в очередь непосещенные точки соседних точек.
        /// </summary>
        /// <param name="pointQueue">Очередь точек.</param>
        /// <param name="visitedPoints">Множество посещенных точек.</param>
        /// <param name="pointsToCheck">Коллекция точек для проверки.</param>
        private static void EnqueueUnvisitedNodes(Queue<Point> pointQueue, 
            HashSet<Point> visitedPoints, IEnumerable<Point> pointsToCheck)
        {
            foreach (var node in pointsToCheck)
            {
                if (visitedPoints.Contains(node))
                    continue;
                pointQueue.Enqueue(node);
                visitedPoints.Add(node);
            }
        }
        /// <summary>
        /// Обновляет путь до соседних точек.
        /// </summary>
        /// <param name="pathMap">Словарь для хранения путей.</param>
        /// <param name="currentPoint">Текущая точка.</param>
        /// <param name="adjacentPoints">Коллекция соседних точек для текущей точки.</param>
        private static void UpdateRoad(Dictionary<Point, SinglyLinkedList<Point>> pathMap, 
            Point currentPoint, IEnumerable<Point> adjacentPoints)
        {
            foreach (var node in adjacentPoints)
            {
                if (!pathMap.ContainsKey(node))
                    pathMap.Add(node, new SinglyLinkedList<Point>(node, pathMap[currentPoint]));
            }
        }
        /// <summary>
        /// Проверяет, содержит ли множество сундук с указанной точкой.
        /// </summary>
        /// <param name="chestsSet">Множество сундуков.</param>
        /// <param name="point">Точка для проверки.</param>
        /// <returns><c>true</c>, если множество содержит указанную точку, иначе <c>false</c>.</returns>
        private static bool ContainChest(HashSet<Point> chestsSet, Point point)
            => chestsSet.Contains(point);
        /// <summary>
        /// Проверяет, содержит ли словарь путь для указанной точки.
        /// </summary>
        /// <param name="road">Словарь для хранения путей.</param>
        /// <param name="point">Точка для проверки.</param>
        /// <returns><c>true</c>, если словарь содержит путь для указанной точки, иначе <c>false</c>.</returns>
        private static bool ContainRoad(Dictionary<Point, SinglyLinkedList<Point>> road, Point point) 
            => road.ContainsKey(point);
        /// <summary>
        /// Удаляет указанную точку из множества сундуков.
        /// </summary>
        /// <param name="chestsSet">Множество сундуков.</param>
        /// <param name="point">Точка для удаления.</param>
        private static void RemoveChest(HashSet<Point> chestsSet, Point point)
            => chestsSet.Remove(point);
        /// <summary>
        /// Возвращает путь для указанной точки из словаря путей.
        /// </summary>
        /// <param name="road">Словарь для хранения путей.</param>
        /// <param name="point">Точка, для которой нужно вернуть путь.</param>
        /// <returns>Путь для указанной точки.</returns>
        private static SinglyLinkedList<Point> GetRoad(Dictionary<Point,
            SinglyLinkedList<Point>> road, Point point) => road[point];
    }
}