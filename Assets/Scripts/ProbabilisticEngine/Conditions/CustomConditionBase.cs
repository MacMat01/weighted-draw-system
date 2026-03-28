using ProbabilisticEngine.Core;
using ProbabilisticEngine.Runtime;

namespace ProbabilisticEngine.Conditions
{
    public abstract class CustomConditionBase : ICondition
    {
        public abstract bool Evaluate(GameState state);
    }
}