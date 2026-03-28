using ProbabilisticEngine.Core;

namespace ProbabilisticEngine.Modifiers
{
    public class StoryFlagModifier : IModifier
    {
        public string Flag;
        public float MultiplierIfTrue = 1f;
        public float MultiplierIfFalse = 1f;

        public float Apply(float weight, GameState state)
        {
            return state.HasFlag(Flag) ? weight * MultiplierIfTrue : weight * MultiplierIfFalse;
        }
    }
}