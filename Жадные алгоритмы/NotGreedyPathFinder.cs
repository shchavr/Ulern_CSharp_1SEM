using Greedy.Architecture;
using System.Collections.Generic;
using System.Linq;

namespace Greedy
{
    public class NotGreedyPathFinder : IPathFinder
    {
        public List<Point> FindPathToCompleteGoal(State currentState)
        {
            var pathFinder = new DijkstraPathFinder();
            var allPaths = currentState.Chests.Append(currentState.Position).SelectMany(start 
                => pathFinder.GetPathsByDijkstra(currentState, 
                start, currentState.Chests.Append(currentState.Position).Where(end => start != end)));
            var allEdges = allPaths.ToDictionary(path => (path.Start, path.End), path => path);
            var optimalPath = FindBestPath(currentState, allEdges);
            return GetPathFromBestPath(optimalPath, allEdges);
        }
        /// <summary>
        /// Находит наилучший путь для достижения цели в указанном состоянии.
        /// </summary>
        /// <param name="state">Состояние</param>
        /// <returns>Список точек для достижения цели</returns>
        private LinkedPath FindBestPath(State state, Dictionary<(Point, Point), PathWithCost> pathEdges)
        {
            var pathStack = new Stack<LinkedPath>();
            pathStack.Push(new LinkedPath(null, state.Position, 0, state.Chests));
            var bestPath = pathStack.Peek();
            while (pathStack.Count > 0)
            {
                var currentPath = pathStack.Pop();
                if (currentPath.PathCost > state.Energy)
                    continue;
                if (currentPath.PathScore > bestPath.PathScore)
                    bestPath = currentPath;
                foreach (var point in currentPath.UnvisitedPoints)
                    pathStack.Push(currentPath.MoveTo(pathEdges[(currentPath.CurrentPosition, point)]));
            }

            return bestPath;
        }
        /// <summary>
        /// Получает путь из наилучшего пути и списка ребер между точками.
        /// </summary>
        /// <param name="bestPath">Наилучший путь</param>
        /// <param name="pathEdges">Словарь ребер между точками</param>
        /// <returns>Список точек, образующих путь</returns>
        private List<Point> GetPathFromBestPath(LinkedPath bestPath, Dictionary<(Point, Point), 
            PathWithCost> pathEdges)
        {
            var pathSequence = bestPath.GetPath();
            var extractedPath = new List<Point>();
            for (var index = 0; index < pathSequence.Count - 1; index++)
            {
                var startPoint = pathSequence[index];
                var endPoint = pathSequence[index + 1];
                var edgeDetails = pathEdges[(startPoint, endPoint)];
                extractedPath.AddRange(edgeDetails.Path.Skip(1));
            }

            return extractedPath;
        }

        public class LinkedPath
        {
            public LinkedPath(LinkedPath previousPath, Point currentPosition, 
                int pathCost, HashSet<Point> unvisitedPoints, int pathScore = 0)
            {
                this.previousPath = previousPath;
                CurrentPosition = currentPosition;
                PathCost = pathCost;
                PathScore = pathScore;
                UnvisitedPoints = unvisitedPoints;
            }

            private readonly LinkedPath previousPath;
            public readonly Point CurrentPosition;
            public readonly HashSet<Point> UnvisitedPoints;
            public readonly int PathCost;
            public readonly int PathScore;

            public List<Point> GetPath()
            {
                var pathSequence = new List<Point>();
                var currentPath = this;
                while (currentPath != null)
                {
                    pathSequence.Add(currentPath.CurrentPosition);
                    currentPath = currentPath.previousPath;
                }

                pathSequence.Reverse();
                return pathSequence;
            }
            public LinkedPath MoveTo(PathWithCost pathEdge)
            {
                var updatedVisitedPoints = new HashSet<Point>(UnvisitedPoints);
                updatedVisitedPoints.Remove(pathEdge.End);
                var updatedPath = new LinkedPath(this, pathEdge.End, PathCost + pathEdge.Cost, 
                    updatedVisitedPoints, PathScore + 1);
                return updatedPath;
            }
        }
    }
}
