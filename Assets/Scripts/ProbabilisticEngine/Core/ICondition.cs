using ProbabilisticEngine.Runtime;

namespace ProbabilisticEngine.Core
{
    public interface ICondition
    {
        bool Evaluate(GameState state);
    }
}