using ProbabilisticEngine.Core;
using ProbabilisticEngine.Runtime;

namespace ProbabilisticEngine.Conditions
{
    public class FlagCondition : ICondition
    {
        public string Flag;

        public bool Evaluate(GameState state)
        {
            return state.HasFlag(Flag);
        }
    }
}