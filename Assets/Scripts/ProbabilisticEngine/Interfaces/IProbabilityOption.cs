namespace ProbabilisticEngine.Interfaces
{
    /// <summary>
    ///     Generic interface for probability options.
    ///     Allows representing any kind of choice (directions, actions, dialogue, etc.).
    ///     Includes generic effect application to game state.
    /// </summary>
    public interface IProbabilityOption<TState> where TState : IGameState
    {
        string Id { get; }
        string Name { get; }
        string Description { get; }

        bool HasEffects { get; }

        int EffectsCount { get; }

        /// <summary>
        ///     Applies this option's effects to the game state.
        ///     This method is called when the option is selected.
        /// </summary>
        void ApplyEffects(TState state);
    }
}
