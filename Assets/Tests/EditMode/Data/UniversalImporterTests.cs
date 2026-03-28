using System.Collections.Generic;
using Data;
using NUnit.Framework;
namespace Tests.EditMode.Data
{
    public class UniversalImporterTests
    {
        [Test]
        public void ImportRawText_Csv_MapsRowsToExampleCardData()
        {
            string csv = "Id,Name,Cost,IsLegendary,Attack\n" +
                "c001,\"Fire, Mage\",3,true,4.5\n" +
                "c002,Guardian,5,false,6";

            List<ExampleCardData> cards = UniversalImporter.ImportRawText<ExampleCardData>(csv, ".csv");

            Assert.AreEqual(2, cards.Count);
            Assert.AreEqual("c001", cards[0].Id);
            Assert.AreEqual("Fire, Mage", cards[0].Name);
            Assert.AreEqual(3, cards[0].Cost);
            Assert.IsTrue(cards[0].IsLegendary);
            Assert.AreEqual(4.5f, cards[0].Attack, 0.001f);

            Assert.AreEqual("c002", cards[1].Id);
            Assert.AreEqual("Guardian", cards[1].Name);
            Assert.AreEqual(5, cards[1].Cost);
            Assert.IsFalse(cards[1].IsLegendary);
            Assert.AreEqual(6f, cards[1].Attack, 0.001f);
        }

        [Test]
        public void ImportRawText_JsonArray_MapsItemsToExampleCardData()
        {
            string json = "[" +
                "{\"Id\":\"c001\",\"Name\":\"Arcane Bolt\",\"Cost\":2,\"IsLegendary\":false,\"Attack\":3.25}," +
                "{\"Id\":\"c002\",\"Name\":\"Titan\",\"Cost\":8,\"IsLegendary\":true,\"Attack\":9}" +
                "]";

            List<ExampleCardData> cards = UniversalImporter.ImportRawText<ExampleCardData>(json, ".json");

            Assert.AreEqual(2, cards.Count);
            Assert.AreEqual("Arcane Bolt", cards[0].Name);
            Assert.IsTrue(cards[1].IsLegendary);
            Assert.AreEqual(9f, cards[1].Attack, 0.001f);
        }
    }
}
