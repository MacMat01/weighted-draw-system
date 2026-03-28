using ProbabilisticEngine.Core;

namespace ProbabilisticEngine.Modifiers
{
    public abstract class CustomModifierBase : IModifier
    {
        public abstract float Apply(float currentWeight, GameState state);
    }
}