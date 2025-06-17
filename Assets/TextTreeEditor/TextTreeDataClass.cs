using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
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
    public List<TTEvent> nodeEventList;
    public List<TextEdge> edgeList;
}

[Serializable]
public class TextEdge
{
    public string nextKey;
    public List<TTEvent> edgeEventList;
    public List<Condition> condList;
}

[Serializable]
public struct Condition
{
    public string field;
    public CompFunc compFunc;
    public string value;
}

[Serializable]
public struct TTEvent
{
    public ExposedReference<UnityEngine.Object> target;
    public string methodName;
    public List<string> args;
    public List<Type> argTyps;
}