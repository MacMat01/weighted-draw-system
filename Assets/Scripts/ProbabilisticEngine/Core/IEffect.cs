using ProbabilisticEngine.Runtime;

namespace ProbabilisticEngine.Core
{
    public interface IEffect
    {
        void Apply(GameState state);
    }
}