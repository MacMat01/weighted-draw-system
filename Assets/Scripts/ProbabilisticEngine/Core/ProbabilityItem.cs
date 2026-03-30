using System.Collections.Generic;
using JetBrains.Annotations;
using ProbabilisticEngine.Interfaces;
namespace ProbabilisticEngine.Core
{
    /// <summary>
    ///     Fully generic ProbabilityItem.
    ///     Works with any game state type and any option type.
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
        ///     Checks whether all conditions are satisfied for the given state.
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
