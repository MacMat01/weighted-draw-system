namespace ProbabilisticEngine.Interfaces
{
    public interface IEffect<TState>
    {
        void Apply(TState state);
    }
}