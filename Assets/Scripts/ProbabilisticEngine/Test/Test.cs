using UnityEngine;
using ProbabilisticEngine.Examples;

namespace ProbabilisticEngine.Test
{
    /// <summary>
    /// Test runner per Unity del sistema di carte probabilistico.
    /// Attacca questo script a un GameObject per eseguire i test.
    /// </summary>
    public class Test : MonoBehaviour
    {
        /// <summary>
        /// Test del pool di carte semplice.
        /// </summary>
        [ContextMenu("Run Card Pool Test")]
        public void RunCardPoolTest()
        {
            try
            {
                CardPoolExample.RunCardPoolTest();
                Debug.Log("✅ Test pool carte semplice completato");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"❌ Errore nel test pool carte semplice: {ex.Message}");
            }
        }

        /// <summary>
        /// Test del pool di carte grande.
        /// </summary>
        [ContextMenu("Run Large Card Pool Test")]
        public void RunLargeCardPoolTest()
        {
            try
            {
                CardPoolExample.RunLargePoolTest();
                Debug.Log("✅ Test pool carte grande completato");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"❌ Errore nel test pool carte grande: {ex.Message}");
            }
        }

        /// <summary>
        /// Esegue tutti i test delle carte.
        /// </summary>
        [ContextMenu("Run All Card Tests")]
        public void RunAllCardTests()
        {
            Debug.Log("🚀 AVVIO TEST SISTEMA CARTE PROBABILISTICO\n");

            RunCardPoolTest();
            RunLargeCardPoolTest();

            Debug.Log("\n🎉 TUTTI I TEST DELLE CARTE COMPLETATI!");
        }

        /// <summary>
        /// Test automatico all'avvio (opzionale).
        /// </summary>
        private void Start()
        {
            RunAllCardTests();
        }
    }
}
