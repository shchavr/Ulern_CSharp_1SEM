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
        /// ��������� ��������� ����� � �������.
        /// </summary>
        /// <param name="queue">������� �����.</param>
        /// <param name="start">��������� �����.</param>
        private static void EnqueueStartPoint(Queue<Point> queue, Point start) => queue.Enqueue(start);
        // <summary>
        /// ������� � ���������� ��������� ����� �� �������.
        /// </summary>
        /// <param name="queue">������� �����.</param>
        /// <returns>��������� ����� �� �������.</returns>
        private static Point DequeuePoint(Queue<Point> queue) => queue.Dequeue();
        /// <summary>
        /// ���������, ��������� �� ����� � �������� �����.
        /// </summary>
        /// <param name="map">����� ����������.</param>
        /// <param name="point">����� ��� ��������.</param>
        /// <returns><c>true</c>, ���� ����� ��������� � �������� �����, ����� <c>false</c>.</returns>
        private static bool IsInBounds(Map map, Point point) => map.InBounds(point);
        /// <summary>
        /// ���������, �������� �� ����� ������ �� �����.
        /// </summary>
        /// <param name="map">����� ����������.</param>
        /// <param name="point">����� ��� ��������.</param>
        /// <returns><c>true</c>, ���� ����� �������� ������, ����� <c>false</c>.</returns>
        private static bool IsWall(Map map, Point point) => map.Dungeon[point.X, point.Y] == MapCell.Wall;
        /// <summary>
        /// ���������� ��������� �������� ����� ��� ��������� �����.
        /// </summary>
        /// <param name="currentPoint">������� �����.</param>
        /// <returns>��������� �������� ����� ��� ��������� �����.</returns>
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
        /// ��������� � ������� ������������ ����� �������� �����.
        /// </summary>
        /// <param name="pointQueue">������� �����.</param>
        /// <param name="visitedPoints">��������� ���������� �����.</param>
        /// <param name="pointsToCheck">��������� ����� ��� ��������.</param>
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
        /// ��������� ���� �� �������� �����.
        /// </summary>
        /// <param name="pathMap">������� ��� �������� �����.</param>
        /// <param name="currentPoint">������� �����.</param>
        /// <param name="adjacentPoints">��������� �������� ����� ��� ������� �����.</param>
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
        /// ���������, �������� �� ��������� ������ � ��������� ������.
        /// </summary>
        /// <param name="chestsSet">��������� ��������.</param>
        /// <param name="point">����� ��� ��������.</param>
        /// <returns><c>true</c>, ���� ��������� �������� ��������� �����, ����� <c>false</c>.</returns>
        private static bool ContainChest(HashSet<Point> chestsSet, Point point)
            => chestsSet.Contains(point);
        /// <summary>
        /// ���������, �������� �� ������� ���� ��� ��������� �����.
        /// </summary>
        /// <param name="road">������� ��� �������� �����.</param>
        /// <param name="point">����� ��� ��������.</param>
        /// <returns><c>true</c>, ���� ������� �������� ���� ��� ��������� �����, ����� <c>false</c>.</returns>
        private static bool ContainRoad(Dictionary<Point, SinglyLinkedList<Point>> road, Point point) 
            => road.ContainsKey(point);
        /// <summary>
        /// ������� ��������� ����� �� ��������� ��������.
        /// </summary>
        /// <param name="chestsSet">��������� ��������.</param>
        /// <param name="point">����� ��� ��������.</param>
        private static void RemoveChest(HashSet<Point> chestsSet, Point point)
            => chestsSet.Remove(point);
        /// <summary>
        /// ���������� ���� ��� ��������� ����� �� ������� �����.
        /// </summary>
        /// <param name="road">������� ��� �������� �����.</param>
        /// <param name="point">�����, ��� ������� ����� ������� ����.</param>
        /// <returns>���� ��� ��������� �����.</returns>
        private static SinglyLinkedList<Point> GetRoad(Dictionary<Point,
            SinglyLinkedList<Point>> road, Point point) => road[point];
    }
}