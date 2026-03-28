using System.Collections.Generic;
using System.Linq;

namespace ProbabilisticEngine.Core
{
    public class ProbabilityChoice
    {
        public string Id;
        public List<ProbabilityOption> Options = new();

        public ProbabilityResult Evaluate(GameState state)
        {
            var valid = Options
                .Where(o => o.AreConditionsMet(state))
                .ToList();

            if (valid.Count == 0)
                return null;

            var weights = valid.Select(o => o.ComputeWeight(state)).ToList();
            int index = WeightedRandom.PickIndex(weights);

            return new ProbabilityResult(valid[index].Id, weights[index]);
        }
    }
}