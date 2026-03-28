using System.Collections.Generic;
using System.Linq;

namespace ProbabilisticEngine.Data
{
    public class ChoiceDatabase
    {
        public List<ChoiceDefinition> Choices = new();

        public ChoiceDefinition GetChoice(string id)
            => Choices.FirstOrDefault(c => c.Id == id);
    }
}