using ProbabilisticEngine.Core;

namespace ProbabilisticEngine.Runtime
{
    public class GameStateController
    {
        public GameState State { get; private set; } = new();

        public void ApplyResult(ProbabilityResult result)
        {
            if (result == null)
                return;

            foreach (var effect in result.Option.Effects)
                effect.Apply(State);

        }
    }
}