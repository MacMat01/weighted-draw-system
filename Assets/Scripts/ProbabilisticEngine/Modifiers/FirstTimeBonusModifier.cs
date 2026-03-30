using ProbabilisticEngine.Core;
using ProbabilisticEngine.Runtime;

namespace ProbabilisticEngine.Modifiers
{
    public class FirstTimeBonusModifier : IModifier
    {
        public float BonusMultiplier = 2f;

        public float Apply(float weight, GameState state)
        {
            return state.HasSeenOption ? weight : weight * BonusMultiplier;
        }
    }
}