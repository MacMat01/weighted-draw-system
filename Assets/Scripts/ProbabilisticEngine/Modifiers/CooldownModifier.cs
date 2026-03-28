using ProbabilisticEngine.Core;
using ProbabilisticEngine.Runtime;

namespace ProbabilisticEngine.Modifiers
{
    public class CooldownModifier : IModifier
    {
        public string OptionId;
        public int CooldownTurns;

        public float Apply(float weight, GameState state)
        {
            if (state.IsOnCooldown(OptionId, CooldownTurns))
                return 0f;

            return weight;
        }
    }
}