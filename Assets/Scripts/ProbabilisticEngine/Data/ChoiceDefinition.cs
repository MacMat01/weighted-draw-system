using System.Collections.Generic;

namespace ProbabilisticEngine.Data
{
    [System.Serializable]
    public class ChoiceDefinition
    {
        public string Id;
        public List<OptionDefinition> Options;
    }
}