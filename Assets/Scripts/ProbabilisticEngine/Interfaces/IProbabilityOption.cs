namespace ProbabilisticEngine.Interfaces
{
    /// <summary>
    ///     Interfaccia generica per le opzioni di probabilità.
    ///     Permette di rappresentare qualsiasi tipo di scelta (direzioni, azioni, dialoghi, ecc.)
    ///     Ora include l'applicazione generica di effetti allo stato di gioco.
    /// </summary>
    public interface IProbabilityOption<TState> where TState : IGameState
    {
        string Id { get; }
        string Name { get; }
        string Description { get; }

        bool HasEffects { get; }

        int EffectsCount { get; }

        /// <summary>
        ///     Applica gli effetti dell'opzione allo stato di gioco.
        ///     Questo metodo viene chiamato quando l'opzione viene selezionata.
        /// </summary>
        void ApplyEffects(TState state);
    }
}
