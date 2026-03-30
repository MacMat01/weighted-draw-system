using System.Collections.Generic;
using ProbabilisticEngine.Interfaces;
namespace ProbabilisticEngine.Core
{
    /// <summary>
    ///     Generic ProbabilityOption implementation of IProbabilityOption
    ///     <TState>
    ///         .
    ///         Includes direct application of effects to game state.
    ///         Effects are completely optional: an option can have no effects
    ///         and still represent a valid narrative or gameplay choice.
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
