using System.Collections.Generic;

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
            var choice = _database.GetChoice(choiceId);
            return choice?.Evaluate(state);
        }
    }
}