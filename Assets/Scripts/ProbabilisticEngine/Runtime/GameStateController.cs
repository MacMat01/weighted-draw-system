using ProbabilisticEngine.Core;

namespace ProbabilisticEngine.Runtime
{
    public class GameStateController
    {
        public GameState State { get; private set; } = new();

        public void ApplyResult(ProbabilityResult result)
        {
            // Qui applichi effetti, flag, risorse, ecc.
        }
    }
}