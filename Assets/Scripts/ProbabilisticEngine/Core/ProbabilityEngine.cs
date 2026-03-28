using System.Collections.Generic;
using ProbabilisticEngine.Data;
using ProbabilisticEngine.Runtime;

namespace ProbabilisticEngine.Core
{
    public class ProbabilityEngine
    {
        private readonly ChoiceDatabase _database;

        public ProbabilityEngine(ChoiceDatabase database)
        {
            _database = database;
        }

        public ProbabilityResult Evaluate(string choiceId, GameState state)
        {
            var def = _database.GetChoice(choiceId);
            if (def == null)
                return null;

            var choice = ChoiceBuilder.Build(def);
            return choice.Evaluate(state);
        }
    }
}