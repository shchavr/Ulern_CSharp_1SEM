using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rocket_bot
{
    public partial class Bot
    {
        public Rocket GetNextMove(Rocket rocket)
        {
            var moveEvaluationTasks = CreateTasks(rocket);
            Task.WaitAll(moveEvaluationTasks.ToArray());
            var optimalMove = moveEvaluationTasks.Select(task => task.Result).Max();
            return rocket.Move(optimalMove.Turn, level);
        }

        public List<Task<(Turn Turn, double Score)>> CreateTasks(Rocket rocket)
        {
            return Enumerable.Range(0, threadsCount)
                             .Select(taskIndex => Task.Run(() =>
                             {
                                 int seed;
                                 lock (random)
                                 {
                                     seed = random.Next();
                                 }
                                 var localRandom = new Random(seed);
                                 return SearchBestMove(rocket, localRandom, iterationsCount / threadsCount);
                             })).ToList();
        }
    }
}
