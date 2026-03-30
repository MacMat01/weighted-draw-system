using System.Collections.Generic;
using System.Linq;
using ProbabilisticEngine.Interfaces;
using ProbabilisticEngine.Utils;
namespace ProbabilisticEngine.Core
{
    /// <summary>
    ///     Generic version of the ProbabilityEngine.
    ///     Manages a pool of ProbabilityItem instances and selects valid entries.
    /// </summary>
    public class ProbabilityEngine<TState, TOption>
        where TState : IGameState
        where TOption : IProbabilityOption<TState>
    {
        private readonly List<ProbabilityItem<TState, TOption>> _items;

        public ProbabilityEngine(IEnumerable<ProbabilityItem<TState, TOption>> items)
        {
            _items = items.ToList();
        }

        /// <summary>
        ///     Filters ProbabilityItem entries whose conditions are satisfied
        ///     and returns the list of valid items.
        /// </summary>
        public List<ProbabilityItem<TState, TOption>> GetValidChoices(TState state)
        {
            return _items.Where(item => item.AreConditionsMet(state)).ToList();
        }

        public ProbabilityItem<TState, TOption> EvaluateRandom(TState state)
        {
            List<ProbabilityItem<TState, TOption>> validItems = GetValidChoices(state);

            if (validItems.Count == 0)
            {
                return null;
            }

            // Compute weights for each valid item.
            List<float> weights = validItems.Select(c => c.BaseWeight).ToList();

            // Select an item based on weighted randomness.
            int index = WeightedRandom.PickIndex(weights);
            ProbabilityItem<TState, TOption> selectedItem = validItems[index];

            // Return the selected ProbabilityItem (option effects are not applied here).
            return selectedItem;
        }
    }
}
