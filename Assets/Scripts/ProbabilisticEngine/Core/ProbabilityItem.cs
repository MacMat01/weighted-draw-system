using System.Collections.Generic;
using JetBrains.Annotations;
using ProbabilisticEngine.Interfaces;
namespace ProbabilisticEngine.Core
{
    /// <summary>
    ///     Versione completamente generica di ProbabilityItem.
    ///     Funziona con qualsiasi tipo di stato e qualsiasi tipo di opzione.
    /// </summary>
    public class ProbabilityItem<TState, TOption>
        where TState : IGameState
        where TOption : IProbabilityOption<TState>
    {
        public float BaseWeight;

        [CanBeNull] public List<ICondition<TState>> Conditions;
        public string Id;
        [CanBeNull] public List<TOption> Options;

        /// <summary>
        ///     Verifica se tutte le condizioni sono soddisfatte per lo stato dato.
        /// </summary>
        public bool AreConditionsMet(TState state)
        {
            if (Conditions == null)
            {
                return true;
            }

            foreach (ICondition<TState> c in Conditions)
                if (!c.Evaluate(state))
                {
                    return false;
                }

            return true;
        }
    }
}
