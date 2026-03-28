using ProbabilisticEngine.Core;

namespace ProbabilisticEngine.Modifiers
{
    public class BiasModifier : IModifier
    {
        public float Bias;

        public float Apply(float weight, GameState state)
        {
            return weight * Bias;
        }
    }
}