namespace ProbabilisticEngine.Core
{
    public interface IModifier
    {
        float Apply(float currentWeight, GameState state);
    }
}