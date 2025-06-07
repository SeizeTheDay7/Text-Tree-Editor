using UnityEngine;
using System;
using System.Collections.Generic;


[Serializable]
public class Serialization<T>
{
    public List<T> items;

    public Serialization(IEnumerable<T> items)
    {
        this.items = new List<T>(items);
    }
}

[Serializable]
public class TextNodeData
{
    public string key;
    public string text;
    public Vector2 position;
    public List<TextEdge> nextNodes;
}

[Serializable]
public class TextEdge
{
    public string nextKey;
    public List<Condition> condArr;
}

[Serializable]
public class Condition
{
    public string field;
    public CompFunc compFunc;
    public float value;
}