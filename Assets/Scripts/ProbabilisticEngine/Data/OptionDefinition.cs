using System.Collections.Generic;

namespace ProbabilisticEngine.Data
{
    [System.Serializable]
    public class OptionDefinition
    {
        public string Id;
        public float BaseWeight;
        
        public List<ConditionDefinition> Conditions;
        public List<ModifierDefinition> Modifiers;

    }
}