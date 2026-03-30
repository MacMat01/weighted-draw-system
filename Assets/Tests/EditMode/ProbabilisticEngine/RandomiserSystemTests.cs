using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Importer;
using NUnit.Framework;
using ProbabilisticEngine.Core;
using UnityEditor;
using UnityEngine;
namespace Tests.EditMode.ProbabilisticEngine
{
    public class RandomiserSystemTests
    {
        private const string csvAssetPath = "Assets/Scripts/Importer/DataSchema/DataSchema_CSV_Example1.asset";
        private const string jsonAssetPath = "Assets/Scripts/Importer/DataSchema/DataSchema_JSON_Example1.asset";

        [SetUp]
        public void SetUp()
        {
            Random.InitState(1337);
        }

        [Test]
        public void EvaluateRandom_UsesWeightColumnFlag_WithCsvAssetPath()
        {
            AssertWeightedSelectionFromSchema(csvAssetPath);
        }

        [Test]
        public void EvaluateRandom_UsesWeightColumnFlag_WithJsonAssetPath()
        {
            AssertWeightedSelectionFromSchema(jsonAssetPath);
        }

        private static void AssertWeightedSelectionFromSchema(string schemaAssetPath)
        {
            DataSchemaSO schema = AssetDatabase.LoadAssetAtPath<DataSchemaSO>(schemaAssetPath);
            Assert.IsNotNull(schema, $"Missing schema asset at '{schemaAssetPath}'.");
            Assert.IsTrue(schema.HasSourceDataFile(), "Schema is expected to have a source TextAsset assigned.");

            List<DataRecord> records = DynamicDataImporter.ImportFromSchema(schema);
            Assert.AreEqual(7, records.Count);

            RandomiserSystem system = new RandomiserSystem(records, schema);
            string weightColumnName = ResolveWeightColumnName(schema);
            Assert.IsNotNull(weightColumnName, "Schema must define exactly one WeightColumn.");

            Dictionary<string, object> notMetContext = new Dictionary<string, object>
            {
                { "Friendship", 0f },
                { "Finance", 0f },
                { "Accademic_Performance", 100f },
                { "Accademic Performance", 100f }
            };

            List<DataRecord> notMetChoices = system.GetValidChoices(notMetContext);
            Assert.Less(notMetChoices.Count, records.Count, "Expected at least one row to be filtered when conditions are not met.");

            // Draw with unmet conditions: selected rows must always come from the filtered subset.
            HashSet<int> notMetAllowedIds = notMetChoices
                .Select(static record => record.GetField("Card_ID"))
                .OfType<int>()
                .ToHashSet();
            Assert.Greater(notMetAllowedIds.Count, 0);

            const int notMetDraws = 5000;
            for (int i = 0; i < notMetDraws; i++)
            {
                DataRecord selected = system.EvaluateRandom(notMetContext);
                Assert.IsNotNull(selected);
                Assert.IsTrue(selected.GetField("Card_ID") is int selectedId && notMetAllowedIds.Contains(selectedId));
            }

            Dictionary<string, object> metContext = new Dictionary<string, object>
            {
                { "Friendship", 100f },
                { "Finance", 100f },
                { "Accademic_Performance", 0f },
                { "Accademic Performance", 0f }
            };

            List<DataRecord> validChoices = system.GetValidChoices(metContext);
            Assert.AreEqual(records.Count, validChoices.Count);

            Dictionary<int, float> weightsByCardId = validChoices
                .Where(static record => record.GetField("Card_ID") is int)
                .ToDictionary(
                    static record => (int)record.GetField("Card_ID"),
                    record => ReadWeightValue(record.GetField(weightColumnName)));

            float totalWeight = weightsByCardId.Values.Sum();
            Assert.Greater(totalWeight, 0f, "Expected positive total weight from imported rows.");

            float maxWeight = weightsByCardId.Values.Max();
            HashSet<int> topWeightIds = weightsByCardId
                .Where(pair => Mathf.Approximately(pair.Value, maxWeight))
                .Select(static pair => pair.Key)
                .ToHashSet();
            Assert.Greater(topWeightIds.Count, 0);

            int topWeightHits = 0;
            const int draws = 25000;
            Dictionary<int, int> drawCountByCardId = records
                .Select(static record => record.GetField("Card_ID"))
                .OfType<int>()
                .ToDictionary(static id => id, static _ => 0);

            for (int i = 0; i < draws; i++)
            {
                DataRecord selected = system.EvaluateRandom(metContext);
                Assert.IsNotNull(selected);

                if (selected.GetField("Card_ID") is int selectedCardId && drawCountByCardId.ContainsKey(selectedCardId))
                {
                    drawCountByCardId[selectedCardId]++;
                }

                if (selected.GetField("Card_ID") is int selectedId && topWeightIds.Contains(selectedId))
                {
                    topWeightHits++;
                }
            }

            foreach (KeyValuePair<int, int> pair in drawCountByCardId)
            {
                Assert.Greater(pair.Value, 0, $"Expected card {pair.Key} to be drawn at least once in {draws} draws.");
            }

            float expectedTopShare = topWeightIds.Sum(id => weightsByCardId[id]) / totalWeight;
            float actualTopShare = topWeightHits / (float)draws;
            Assert.That(actualTopShare, Is.EqualTo(expectedTopShare).Within(0.05f));
        }

        private static float ReadWeightValue(object rawWeight)
        {
            if (rawWeight == null)
            {
                return 0f;
            }

            return rawWeight switch
            {
                int intValue => intValue,
                long longValue => longValue,
                float floatValue => floatValue,
                double doubleValue => (float)doubleValue,
                string stringValue when float.TryParse(stringValue, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out float parsedValue) => parsedValue,
                _ => 0f
            };
        }

        private static string ResolveWeightColumnName(DataSchemaSO schema)
        {
            List<ColumnDefinition> weightColumns = schema.Columns
                .Where(static column => column is { DataType: ColumnDataType.WeightColumn } && !string.IsNullOrWhiteSpace(column.ColumnName))
                .ToList();

            return weightColumns.Count != 1 ? null : weightColumns[0].ColumnName;

        }
    }
}
