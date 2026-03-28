namespace ProbabilisticEngine.Core
{
    public class ProbabilityResult
    {
        public string OptionId { get; }
        public float FinalWeight { get; }

        public ProbabilityResult(string optionId, float finalWeight)
        {
            OptionId = optionId;
            FinalWeight = finalWeight;
        }
    }
}

