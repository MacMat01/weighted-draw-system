using System;
using System.Collections.Generic;
using System.Globalization;
using Importer;
using ProbabilisticEngine.Utils;
using UnityEngine;
using Random = UnityEngine.Random;
namespace ProbabilisticEngine.Core
{
    /// <summary>
    ///     Filters imported DataRecord rows by ParsedCondition lists and selects one valid row at random.
    /// </summary>
    public sealed class RandomiserSystem
    {
        private readonly string conditionColumnName;
        private readonly List<DataRecord> items;
        private readonly string weightColumnName;

        private RandomiserSystem(
            IEnumerable<DataRecord> items,
            string conditionColumnName,
            string weightColumnName = null)
        {
            this.items = items != null ? new List<DataRecord>(items) : new List<DataRecord>();
            this.conditionColumnName = conditionColumnName;
            this.weightColumnName = weightColumnName;
        }

        public RandomiserSystem(
            IEnumerable<DataRecord> items,
            DataSchemaSO schema,
            string conditionColumnName = null,
            string weightColumnName = null)
            : this(
                items,
                string.IsNullOrWhiteSpace(conditionColumnName)
                    ? ResolveColumnByType(schema, ColumnDataType.ConditionList)
                    : conditionColumnName,
                string.IsNullOrWhiteSpace(weightColumnName)
                    ? ResolveColumnByType(schema, ColumnDataType.WeightColumn)
                    : weightColumnName)
        {
        }

        /// <summary>
        ///     Returns only rows whose parsed conditions evaluate to true for the provided context.
        /// </summary>
        public List<DataRecord> GetValidChoices(IReadOnlyDictionary<string, object> gameStateContext)
        {
            List<DataRecord> valid = new List<DataRecord>();
            foreach (DataRecord item in items)
            {
                if (item != null && AreItemConditionsMet(item, gameStateContext))
                {
                    valid.Add(item);
                }
            }

            return valid;
        }

        /// <summary>
        ///     Selects one valid row based on weighted probability ratios.
        /// </summary>
        public DataRecord EvaluateRandom(IReadOnlyDictionary<string, object> gameStateContext)
        {
            List<DataRecord> validItems = GetValidChoices(gameStateContext);
            if (validItems.Count == 0)
            {
                return null;
            }

            List<float> weights = BuildWeights(validItems);
            float totalWeight = 0f;
            foreach (float weight in weights)
            {
                totalWeight += weight;
            }

            if (totalWeight <= 0f)
            {
                int uniformIndex = Random.Range(0, validItems.Count);
                return validItems[uniformIndex];
            }

            int weightedIndex = WeightedRandom.PickIndex(weights);
            return validItems[weightedIndex];
        }

        private bool AreItemConditionsMet(DataRecord item, IReadOnlyDictionary<string, object> gameStateContext)
        {
            if (string.IsNullOrWhiteSpace(conditionColumnName))
            {
                return true;
            }

            object rawConditionList = item.GetField(conditionColumnName);
            if (rawConditionList == null)
            {
                return true;
            }

            if (rawConditionList is not List<ParsedCondition> parsedConditions)
            {
                return false;
            }

            if (parsedConditions.Count == 0)
            {
                return true;
            }

            bool aggregate = EvaluateCondition(parsedConditions[0], gameStateContext);
            for (int i = 1; i < parsedConditions.Count; i++)
            {
                ParsedCondition condition = parsedConditions[i];
                bool current = EvaluateCondition(condition, gameStateContext);

                if (string.Equals(condition.ConnectorFromPrevious, "OR", StringComparison.OrdinalIgnoreCase))
                {
                    aggregate = aggregate || current;
                }
                else
                {
                    // Default to AND when connector metadata is missing or malformed.
                    aggregate = aggregate && current;
                }
            }

            return aggregate;
        }

        private List<float> BuildWeights(IReadOnlyList<DataRecord> validItems)
        {
            List<float> weights = new List<float>(validItems.Count);
            foreach (DataRecord item in validItems)
            {
                if (string.IsNullOrWhiteSpace(weightColumnName))
                {
                    weights.Add(1f);
                    continue;
                }

                object rawWeight = item.GetField(weightColumnName);
                if (TryConvertToFloat(rawWeight, out float parsedWeight) && parsedWeight > 0f)
                {
                    weights.Add(parsedWeight);
                }
                else
                {
                    weights.Add(0f);
                }
            }

            return weights;
        }

        private static bool EvaluateCondition(ParsedCondition condition, IReadOnlyDictionary<string, object> gameStateContext)
        {
            if (condition == null)
            {
                return false;
            }

            if (condition.IsBooleanLiteral)
            {
                return condition.BooleanLiteralValue;
            }

            if (!TryResolveContextValue(gameStateContext, condition.VariableName, out float leftValue))
            {
                return false;
            }

            float rightValue = condition.Value;
            return condition.Operator switch
            {
                "==" => Mathf.Approximately(leftValue, rightValue),
                "!=" => !Mathf.Approximately(leftValue, rightValue),
                ">" => leftValue > rightValue,
                "<" => leftValue < rightValue,
                ">=" => leftValue >= rightValue,
                "<=" => leftValue <= rightValue,
                _ => false
            };
        }

        private static bool TryResolveContextValue(IReadOnlyDictionary<string, object> gameStateContext, string variableName, out float value)
        {
            value = 0f;
            if (string.IsNullOrWhiteSpace(variableName) || gameStateContext == null || gameStateContext.Count == 0)
            {
                return false;
            }

            foreach (KeyValuePair<string, object> pair in gameStateContext)
            {
                if (!string.Equals(pair.Key, variableName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                return TryConvertToFloat(pair.Value, out value);
            }

            return false;
        }

        private static bool TryConvertToFloat(object rawValue, out float value)
        {
            value = 0f;
            switch (rawValue)
            {
                case null:
                    return false;
                case float floatValue:
                    value = floatValue;
                    return true;
                case double doubleValue:
                    value = (float)doubleValue;
                    return true;
                case int intValue:
                    value = intValue;
                    return true;
                case long longValue:
                    value = longValue;
                    return true;
                case bool boolValue:
                    value = boolValue ? 1f : 0f;
                    return true;
                case string stringValue:
                    if (float.TryParse(stringValue, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out float parsedStringFloat))
                    {
                        value = parsedStringFloat;
                        return true;
                    }

                    if (bool.TryParse(stringValue, out bool parsedStringBool))
                    {
                        value = parsedStringBool ? 1f : 0f;
                        return true;
                    }

                    return false;
                default:
                    try
                    {
                        value = Convert.ToSingle(rawValue, CultureInfo.InvariantCulture);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
            }
        }

        private static string ResolveColumnByType(DataSchemaSO schema, ColumnDataType type)
        {
            if (schema?.Columns == null)
            {
                return null;
            }

            foreach (ColumnDefinition column in schema.Columns)
            {
                if (column != null && column.DataType == type && !string.IsNullOrWhiteSpace(column.ColumnName))
                {
                    return column.ColumnName;
                }
            }

            return null;
        }
    }
}
