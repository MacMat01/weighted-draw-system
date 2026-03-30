using System.Collections.Generic;
using System.Linq;
using ProbabilisticEngine.Data;
using ProbabilisticEngine.Runtime;

namespace ProbabilisticEngine.Core
{
    public class ProbabilityEngine
    {
        private readonly Dictionary<string, ProbabilityChoice> _choices;

        public ProbabilityEngine(IEnumerable<ProbabilityChoice> choices)
        {
            _choices = choices.ToDictionary(c => c.Id);
        }

        public ProbabilityResult Evaluate(string choiceId, GameState state)
        {
            if (!_choices.TryGetValue(choiceId, out var choice))
                return null;

            return choice.Evaluate(state);
        }
    }
}