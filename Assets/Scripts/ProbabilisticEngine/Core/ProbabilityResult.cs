    namespace ProbabilisticEngine.Core
{
    public class ProbabilityResult
    {
        public ProbabilityOption Option { get; }
        public float FinalWeight { get; }

        public ProbabilityResult(ProbabilityOption option, float finalWeight)
        {
            Option = option;
            FinalWeight = finalWeight;
        }
    }
}

