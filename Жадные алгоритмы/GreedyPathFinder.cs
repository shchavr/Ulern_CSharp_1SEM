using System.Collections.Generic;
using System.Linq;
using Greedy.Architecture;

namespace Greedy;

public class GreedyPathFinder : IPathFinder
{
    public List<Point> FindPathToCompleteGoal(State currentState)
    {
        var pathFinder = new DijkstraPathFinder();
        var chestLocations = new HashSet<Point>(currentState.Chests);
        var pathResult = new List<Point>();

        for (var goalIndex = 0; goalIndex < currentState.Goal; goalIndex++)
        {
            var pathToChest = GetPathToNextChest(currentState, pathFinder, chestLocations);

            if (pathToChest == null)
                return new List<Point>();

            currentState.Position = pathToChest.End;
            currentState.Energy -= pathToChest.Cost;

            if (currentState.Energy < 0)
                return new List<Point>();

            chestLocations.Remove(pathToChest.End);
            pathResult.AddRange(GetPathWithoutStartPoint(pathToChest));
        }

        return pathResult;
    }

    /// <summary>
    /// Получает путь до следующего сундука.
    /// </summary>
    /// <param name="currentState">Текущее состояние.</param>
    /// <param name="pathFinder">Объект для поиска пути.</param>
    /// <param name="chestLocations">Координаты сундуков.</param>
    /// <returns>Путь до следующего сундука.</returns>
    private PathWithCost GetPathToNextChest(State currentState, 
        DijkstraPathFinder pathFinder, HashSet<Point> chestLocations)
        => pathFinder.GetPathsByDijkstra(currentState, currentState.Position, chestLocations).FirstOrDefault();

    /// <summary>
    /// Получает путь без стартовой точки.
    /// </summary>
    /// <param name="pathFinder">Объект с путем.</param>
    /// <returns>Путь без стартовой точки.</returns>
    private IEnumerable<Point> GetPathWithoutStartPoint(PathWithCost pathFinder)
        => pathFinder.Path.Skip(1);
}