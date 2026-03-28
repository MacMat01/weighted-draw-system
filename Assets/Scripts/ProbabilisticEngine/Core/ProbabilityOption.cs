using System.Collections.Generic;
using ProbabilisticEngine.Runtime;

namespace ProbabilisticEngine.Core
{
    public class ProbabilityOption
    {
        public string Id;
        public float BaseWeight;

        public List<ICondition> Conditions = new();
        public List<IModifier> Modifiers = new();

        public bool AreConditionsMet(GameState state)
        {
            foreach (var c in Conditions)
                if (!c.Evaluate(state))
                    return false;

            return true;
        }

        public float ComputeWeight(GameState state)
        {
            float weight = BaseWeight;

            foreach (var m in Modifiers)
                weight = m.Apply(weight, state);

            return weight;
        }
    }
}