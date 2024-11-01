using System;
using System.Collections.Generic;
using System.Linq;
using Greedy.Architecture;

namespace Greedy
{
    public class DijkstraData
    {
        public Point Previous { get; set; }
        public int Price { get; set; }
        public DijkstraData()
        {
            Previous = new Point(0, 0);
            Price = 0;
        }
    }

    public class DijkstraPathFinder
    {
        private HashSet<Point> targetPointsCoordinates;
        private Dictionary<Point, DijkstraData> pathTrackk;
        private List<Point> visited;
        private Point pointOutsidePath;

        public IEnumerable<PathWithCost> GetPathsByDijkstra(State state, Point start, IEnumerable<Point> targets)
        {
            Initialize(state, start, targets);

            while (targetPointsCoordinates.Count > 0)
            {
                var nextNodeToOpen = UnpackNode();

                if (nextNodeToOpen == pointOutsidePath)
                    break;

                if (targetPointsCoordinates.Contains(nextNodeToOpen))
                {
                    yield return MakePath(nextNodeToOpen, start);
                    targetPointsCoordinates.Remove(nextNodeToOpen);
                }

                var adjacentNodes = GetIncidentNodes(nextNodeToOpen, state);
                AddIncidentNodes(adjacentNodes, nextNodeToOpen, state);
            }
        }
        /// <summary>
        /// �������������� ��������� ��������� ���������.
        /// </summary>
        /// <param name="state">������� ��������� ����.</param>
        /// <param name="start">��������� �����.</param>
        /// <param name="targets">��������� ������� �����.</param>
        private void Initialize(State state, Point start, IEnumerable<Point> targets)
        {
            targetPointsCoordinates = new HashSet<Point>(targets);
            pathTrackk = new Dictionary<Point, DijkstraData>();
            pathTrackk[start] = new DijkstraData ();

            visited = new List<Point>();
            pointOutsidePath = new Point(state.MapWidth + 1, state.MapHeight + 1);
        }
        /// <summary>
        /// ������������� ��������� ���� ��� ���������.
        /// </summary>
        /// <returns>��������� ���� ��� ���������.</returns>
        private Point UnpackNode()
        {
            var bestPrice = double.PositiveInfinity;
            var nextNodeToOpen = pointOutsidePath;

            foreach (var point in pathTrackk.Where(pointDataPair => !visited.Contains(pointDataPair.Key) 
                && pointDataPair.Value.Price < bestPrice))
            {
                bestPrice = point.Value.Price;
                nextNodeToOpen = point.Key;
            }

            return nextNodeToOpen;
        }
        /// <summary>
        /// ��������� �������� ���� � �������� ���� ��� ���������� ���������.
        /// </summary>
        /// <param name="adjacentNodes">��������� �������� �����.</param>
        /// <param name="currentNode">������� ����.</param>
        /// <param name="state">������� ��������� ����.</param>
        private void AddIncidentNodes(IEnumerable<Point> adjacentNodes, Point currentNode, State state)
        {
            foreach (var adjacentNode in adjacentNodes)
            {
                var newPrice = pathTrackk[currentNode].Price + state.CellCost[adjacentNode.X, adjacentNode.Y];
                if (!pathTrackk.ContainsKey(adjacentNode) || pathTrackk[adjacentNode].Price > newPrice)
                    pathTrackk[adjacentNode] = new DijkstraData { Previous = currentNode, Price = newPrice };
            }
            visited.Add(currentNode);
        }
        /// <summary>
        /// ������� ���� � ����� ����� ���������� �������.
        /// </summary>
        /// <param name="endPoint">�������� �����.</param>
        /// <param name="startPoint">��������� �����.</param>
        /// <returns>���� � ����� ����� ���������� �������.</returns>
        private PathWithCost MakePath(Point endPoint, Point startPoint)
        {
            var pathPoints = new List<Point>();
            var currentPoint = endPoint;
            while (currentPoint != startPoint)
            {
                pathPoints.Add(currentPoint);
                currentPoint = pathTrackk[currentPoint].Previous;
            }

            pathPoints.Add(startPoint);
            pathPoints.Reverse();
            var constructedPath = new PathWithCost(pathTrackk[endPoint].Price, pathPoints.ToArray());
            return constructedPath;
        }
        /// <summary>
        /// ���������� �������� ���� ��� ���������� ����.
        /// </summary>
        /// <param name="currentNode">������� ����.</param>
        /// <param name="gameState">������� ��������� ����.</param>
        /// <returns>��������� �������� �����.</returns>
        private IEnumerable<Point> GetIncidentNodes(Point currentNode, State gameState)
        {
            return new[]
            {
                new Point(currentNode.X, currentNode.Y + 1),
                new Point(currentNode.X, currentNode.Y - 1),
                new Point(currentNode.X + 1, currentNode.Y),
                new Point(currentNode.X - 1, currentNode.Y)
            }
            .Where(point => gameState.InsideMap(point) && !gameState.IsWallAt(point));
        }
    }
}