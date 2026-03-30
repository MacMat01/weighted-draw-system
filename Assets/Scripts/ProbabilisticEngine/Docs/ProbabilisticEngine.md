# ProbabilisticEngine Documentation

## Overview

The **ProbabilisticEngine** is a powerful choice-driven system that enables dynamic decision-making based on conditions, weighted probabilities, and modifiers. It's designed for narrative games, RPGs, and interactive storytelling where outcomes should be influenced by game state, player actions, and conditional logic.

## Core Concept

The engine evaluates choices by:
1. **Filtering** available options based on conditions
2. **Computing weights** for each valid option, adjusted by modifiers
3. **Selecting** an outcome using weighted random selection
4. **Applying** effects to the game state

## Architecture

### Core Components

#### 1. **ProbabilityEngine**
The main orchestrator that evaluates choices.

```csharp
public class ProbabilityEngine
{
    public ProbabilityResult Evaluate(string choiceId, GameState state)
}
```

- Takes a choice ID and current game state
- Returns a `ProbabilityResult` with the selected option and its final weight
- Manages a dictionary of all available choices

#### 2. **ProbabilityChoice**
Represents a single choice with multiple options.

```csharp
public class ProbabilityChoice
{
    public string Id;
    public List<ProbabilityOption> Options = new();
    public ProbabilityResult Evaluate(GameState state)
}
```

- Contains multiple `ProbabilityOption` instances
- Filters valid options based on their conditions
- Computes weights and performs weighted random selection

#### 3. **ProbabilityOption**
Represents a single outcome or response within a choice.

```csharp
public class ProbabilityOption
{
    public string Id;
    public float BaseWeight;
    public List<IEffect> Effects = new();
    public List<ICondition> Conditions = new();
    public List<IModifier> Modifiers = new();
    
    public bool AreConditionsMet(GameState state)
    public float ComputeWeight(GameState state)
}
```

- **BaseWeight**: Default probability weight (higher = more likely)
- **Conditions**: Must all be met for option to be valid
- **Modifiers**: Adjust the weight based on game state
- **Effects**: Applied when this option is selected

#### 4. **ProbabilityResult**
The outcome of an evaluation.

```csharp
public class ProbabilityResult
{
    public ProbabilityOption Option { get; }
    public float FinalWeight { get; }
}
```

- Contains the selected option and its computed weight
- Used to determine which effects should be applied

### Game State

#### **GameState**
Maintains all runtime data that influences choice evaluation.

```csharp
public class GameState
{
    private Dictionary<string, int> _resources;
    private HashSet<string> _flags;
    private Dictionary<string, string> _context;
    
    public int GetResource(string key)
    public void SetResource(string key, int value)
    public bool HasFlag(string flag)
    public void SetFlag(string flag)
    public string GetContext(string key)
    public void SetContext(string key, string value)
    public bool IsOnCooldown(string optionId, int turns)
}
```

- **Resources**: Named numeric values (e.g., health, gold, reputation)
- **Flags**: Boolean state markers (e.g., quest_completed, has_key)
- **Context**: String key-value pairs for dynamic state
- **Cooldowns**: Track option availability over turns

## Extensibility

### Conditions

**ICondition Interface**
```csharp
public interface ICondition
{
    bool Evaluate(GameState state);
}
```

Conditions determine if an option is available. Only options with all conditions met are considered.

#### Built-in Conditions

- **FlagCondition**: Checks if a game state flag is set
  ```csharp
  Flag: "has_sword"  // evaluates state.HasFlag("has_sword")
  ```

- **ResourceCondition**: Checks if a resource meets a threshold
  ```csharp
  Resource: "mana"
  MinValue: 50  // evaluates state.GetResource("mana") >= 50
  ```

- **ContextCondition**: Checks context values
- **CustomConditionBase**: Extend for custom logic

### Modifiers

**IModifier Interface**
```csharp
public interface IModifier
{
    float Apply(float currentWeight, GameState state);
}
```

Modifiers transform option weights based on game state, allowing dynamic probability adjustments.

#### Built-in Modifiers

- **BiasModifier**: Multiplies weight by a bias factor
  ```csharp
  Bias: 1.5  // weight *= 1.5
  ```

- **CooldownModifier**: Reduces weight if option is on cooldown
  ```csharp
  OptionId: "attack"
  CooldownTurns: 3
  ```

- **FirstTimeBonusModifier**: Increases weight for first-time selections
- **StoryFlagModifier**: Adjusts weight based on narrative flags
- **CustomModifierBase**: Extend for custom logic

### Effects

**IEffect Interface**
```csharp
public interface IEffect
{
    void Apply(GameState state);
}
```

Effects are applied to the game state when an option is selected.

#### Built-in Effects

- **ResourceEffect**: Modifies a resource value
  ```csharp
  Resource: "health"
  Amount: -10  // reduces health by 10
  ```

- **CustomEffectBase**: Extend for custom logic

## Data Definitions

### Choice Definition

```csharp
public class ChoiceDefinition
{
    public string Id;
    public List<OptionDefinition> Options;
}
```

Defines a choice and its available options.

### Option Definition

```csharp
public class OptionDefinition
{
    public string Id;
    public float BaseWeight;
    public List<ConditionDefinition> Conditions;
    public List<ModifierDefinition> Modifiers;
}
```

Defines an individual option with its base weight, conditions, and modifiers.

### Condition Definition

```csharp
public class ConditionDefinition
{
    public string Type;      // "Flag", "Resource", "Context", etc.
    public string ParamA;    // varies by type
    public string ParamB;    // varies by type
}
```

Generic definition for conditions that can be serialized and deserialized.

### Modifier Definition

```csharp
public class ModifierDefinition
{
    public string Type;      // "Bias", "Cooldown", etc.
    public string ParamA;    // varies by type
    public string ParamB;    // varies by type
}
```

Generic definition for modifiers that can be serialized and deserialized.

## Usage Flow

### 1. Building Choices

Use the **ChoiceBuilder** to construct choices from definitions:

```csharp
var choiceDefinition = database.GetChoice("tavern_encounter");
var choice = ChoiceBuilder.Build(choiceDefinition);
```

The builder instantiates the correct condition and modifier types based on their `Type` field.

### 2. Creating the Engine

```csharp
var choices = new[] { choice1, choice2, choice3 };
var engine = new ProbabilityEngine(choices);
```

### 3. Evaluating Choices

```csharp
var gameState = new GameState();
gameState.SetResource("gold", 100);
gameState.SetFlag("has_sword", true);

var result = engine.Evaluate("tavern_encounter", gameState);

if (result != null)
{
    // Apply the selected option's effects
    foreach (var effect in result.Option.Effects)
    {
        effect.Apply(gameState);
    }
}
```

## Example Scenario

**Choice**: "Combat Encounter"

**Option 1**: Aggressive Attack
- BaseWeight: 1.0
- Condition: Has sword equipped
- Modifier: BiasModifier (1.2x) if player level > 5
- Effects: Reduce enemy health by 20

**Option 2**: Defensive Stance
- BaseWeight: 0.8
- Condition: Health below 50%
- Modifier: BiasModifier (2.0x) if defensive_skill flag is set
- Effects: Restore 15 health, add defensive_active flag

**Option 3**: Flee
- BaseWeight: 0.5
- Condition: None
- Modifier: BiasModifier (3.0x) if has_escape_route flag set
- Effects: Move to safe location, add combat_fled flag

**Evaluation Process**:
1. Filter: Only options with met conditions are valid
2. Compute: Calculate final weights with modifiers applied
3. Select: Use weighted random selection
4. Apply: Execute effects from the selected option

## Weighted Random Selection

The engine uses `WeightedRandom.PickIndex()` to select options based on their computed weights:

```csharp
var weights = new[] { 1.0f, 1.5f, 0.5f };
int selectedIndex = WeightedRandom.PickIndex(weights);
// Selection probability: Option 0: 40%, Option 1: 60%, Option 2: 20%
// (normalized: 1.0/(1.0+1.5+0.5) = 40%, etc.)
```

## Extension Points

### Creating Custom Conditions

```csharp
public class TimeOfDayCondition : ICondition
{
    public string TimeOfDay;  // "morning", "night", etc.
    
    public bool Evaluate(GameState state)
    {
        return state.GetContext("time_of_day") == TimeOfDay;
    }
}
```

### Creating Custom Modifiers

```csharp
public class LuckModifier : IModifier
{
    public float LuckFactor;
    
    public float Apply(float weight, GameState state)
    {
        int luck = state.GetResource("luck");
        return weight * (1f + (luck * 0.01f * LuckFactor));
    }
}
```

### Creating Custom Effects

```csharp
public class DialogueEffect : CustomEffectBase
{
    public string DialogueKey;
    
    public override void Apply(GameState state)
    {
        state.SetContext("last_dialogue", DialogueKey);
        // Trigger dialogue system
    }
}
```

## Database

The **ChoiceDatabase** stores all choice definitions and manages their serialization/deserialization:

```csharp
public class ChoiceDatabase
{
    public ChoiceDefinition GetChoice(string id)
    public void SaveChoice(ChoiceDefinition choice)
    public List<ChoiceDefinition> GetAllChoices()
}
```

## Performance Considerations

- **Lazy Evaluation**: Options are only evaluated when `Evaluate()` is called
- **Condition Short-circuiting**: All conditions must pass; evaluation stops at first failure
- **Weight Caching**: Consider caching weights if the same choice is evaluated multiple times
- **Efficient Random Selection**: Weighted random uses index-based selection (O(n) complexity)

## Future Enhancements

- Resource system generalization (currently TODO)
- Cooldown system implementation (currently placeholder)
- First-time option tracking (HasSeenOption)
- Option chaining/consequences
- Conditional effects based on selection results
- Serialization to JSON/YAML for easy definition management

