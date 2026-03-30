using ProbabilisticEngine.Core;
using ProbabilisticEngine.Runtime;

namespace ProbabilisticEngine.Effects
{
    public abstract class CustomEffectBase :  IEffect
    {
        public abstract void Apply(GameState state);
    }
}