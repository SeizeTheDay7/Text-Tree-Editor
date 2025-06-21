using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

// Data classes for text tree SO

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
    public List<TTDEvent> nodeEventList;
    public List<TextEdge> edgeList;
}

[Serializable]
public class TextEdge
{
    public string nextKey;
    public List<TTDCondition> conditionList;
    public List<TTDEvent> edgeEventList;
}

/// Data classes for editor scripts

[Serializable]
public struct TTDCondition
{
    public string command;
}

[Serializable]
public struct TTDEvent
{
    public string actorName;
    public string eventName;
}

/// Data classes for runtime monobehaviour scripts

[Serializable]
public struct TTCondition
{
    // public string command;
}

[Serializable]
public struct TTEvent
{
    public string eventName;
    public UnityEvent eventList;
}