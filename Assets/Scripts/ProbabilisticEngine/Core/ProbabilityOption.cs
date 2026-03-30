using System.Collections.Generic;
using ProbabilisticEngine.Interfaces;
namespace ProbabilisticEngine.Core
{
    /// <summary>
    ///     Versione generica di ProbabilityOption che implementa IProbabilityOption
    ///     <TState>
    ///         .
    ///         Include l'applicazione diretta di effetti allo stato di gioco.
    ///         Gli effetti sono COMPLETAMENTE OPZIONALI - un'opzione può non avere alcun effetto
    ///         e servire solo per rappresentare una scelta (narrativa, di direzione, etc.).
    /// </summary>
    public class ProbabilityOption<TState> : IProbabilityOption<TState>
        where TState : IGameState
    {

        private readonly List<IEffect<TState>> _effects = new List<IEffect<TState>>();
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public bool HasEffects => _effects.Count > 0;
        public int EffectsCount => _effects.Count;

        public void ApplyEffects(TState state)
        {
            foreach (IEffect<TState> effect in _effects)
            {
                effect.Apply(state);
            }
        }

        public void AddEffect(IEffect<TState> effect)
        {
            _effects.Add(effect);
        }
    }
}
