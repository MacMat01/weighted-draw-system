using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Deck/Evento")]
public class Evento : ScriptableObject
{
    public string id;
    public float baseWeight = 1f;

    public List<string> tags = new();
    public List<Param> paramsList = new();

    // helper runtime (non serializzato)
    public Dictionary<string, float> Params {
        get {
            var dict = new Dictionary<string, float>();
            foreach (var p in paramsList)
                dict[p.key] = p.value;
            return dict;
        }
    }
}

[Serializable]
public struct Param
{
    public string key;
    public float value;
}