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
public class TextTreeMetaData
{
    // public Vector2 lastTranslate;
    // public float lastZoom;
    public SceneAsset scene; // To expose in inspector
    public string sceneAssetPath; // To use in runtime
}

[Serializable]
public class TextNodeData
{
    public string key;
    public string text;
    public Vector2 position;
    public List<UnityEvent> nodeEvent;
    public List<TextEdge> edgeList;
}

[Serializable]
public class TextEdge
{
    public string nextKey;
    public List<UnityEvent> edgeEvent;
    public List<Condition> condList;
}

[Serializable]
public class Condition
{
    public string field;
    public CompFunc compFunc;
    public string value;
}