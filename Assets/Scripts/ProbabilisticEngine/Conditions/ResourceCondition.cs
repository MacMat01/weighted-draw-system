using ProbabilisticEngine.Core;
using ProbabilisticEngine.Runtime;

namespace ProbabilisticEngine.Conditions
{
    public class ResourceCondition : ICondition
    {
        public string Resource;
        public int MinValue;

        public bool Evaluate(GameState state)
        {
            return state.GetResource(Resource) >= MinValue;
        }
    }
}