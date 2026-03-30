using System;
using System.Collections.Generic;
using JetBrains.Annotations;
namespace ProbabilisticEngine.Data
{
    [Serializable]
    public class OptionDefinition
    {
        public string Id;
        [CanBeNull] public List<ModifierDefinition> Modifiers;
        [CanBeNull] public List<EffectDefinition> Effects;
    }
}
