using UnityEngine;

public class Test : MonoBehaviour
{
    void Start()
    {
        Debug.Log("TestRunner START chiamato");
        ProbabilityEngineTest.Run();
        Debug.Log("FINISH");
    }
}