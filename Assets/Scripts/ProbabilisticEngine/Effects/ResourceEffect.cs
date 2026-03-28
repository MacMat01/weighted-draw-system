using ProbabilisticEngine.Runtime;

namespace ProbabilisticEngine.Effects
{
    public class ResourceEffect : CustomEffectBase
    {
        public string Resource;
        public int Amount;

        public override void Apply(GameState state)
        {
            int current = state.GetResource(Resource);
            state.SetResource(Resource, current + Amount);
        }
    }
}