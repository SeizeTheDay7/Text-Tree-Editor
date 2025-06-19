using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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
    public List<TTEvent> nodeEventList;
    public List<TextEdge> edgeList;
}

[Serializable]
public class TextEdge
{
    public string nextKey;
    public List<Condition> conditionList;
    public List<TTEvent> edgeEventList;
}

public struct Condition
{
    public string command;
}

[Serializable]
public struct TTEvent
{
    public string command;
}