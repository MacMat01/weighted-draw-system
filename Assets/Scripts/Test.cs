using UnityEngine;

public class Test : MonoBehaviour
{
    void Start()
    {
        Debug.Log("TestRunner START chiamato");
        ProbabilityEngineTest2.Run();
        Debug.Log("FINISH");
    }
}