using ProbabilisticEngine.Conditions;
using ProbabilisticEngine.Core;
using ProbabilisticEngine.Data;
using ProbabilisticEngine.Modifiers;

public static class ChoiceBuilder
{
    public static ProbabilityChoice Build(ChoiceDefinition def)
    {
        var choice = new ProbabilityChoice
        {
            Id = def.Id
        };

        foreach (var optDef in def.Options)
        {
            var option = new ProbabilityOption
            {
                Id = optDef.Id,
                BaseWeight = optDef.BaseWeight
            };

            // Conditions
            foreach (var condDef in optDef.Conditions)
                option.Conditions.Add(BuildCondition(condDef));

            // Modifiers
            foreach (var modDef in optDef.Modifiers)
                option.Modifiers.Add(BuildModifier(modDef));

            choice.Options.Add(option);
        }

        return choice;
    }

    private static ICondition BuildCondition(ConditionDefinition def)
    {
        return def.Type switch
        {
            "Resource" => new ResourceCondition
            {
                Resource = def.ParamA,
                MinValue = int.Parse(def.ParamB)
            },

            "Flag" => new FlagCondition
            {
                Flag = def.ParamA
            },

            _ => throw new System.Exception($"Unknown condition type: {def.Type}")
        };
    }

    private static IModifier BuildModifier(ModifierDefinition def)
    {
        return def.Type switch
        {
            "Bias" => new BiasModifier
            {
                Bias = float.Parse(def.ParamA)
            },

            "Cooldown" => new CooldownModifier
            {
                OptionId = def.ParamA,
                CooldownTurns = int.Parse(def.ParamB)
            },

            _ => throw new System.Exception($"Unknown modifier type: {def.Type}")
        };
    }
}