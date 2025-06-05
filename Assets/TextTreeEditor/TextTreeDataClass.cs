using UnityEngine;
using System;

[Serializable]
public class TextNode
{
    public Guid key;
    public string text;
    public Vector2 position;
    public TextEdge[] nextNodes;
}

[Serializable]
public class TextEdge
{
    public Guid nextKey;
    public Condition[] condArr;
}

[Serializable]
public class Condition
{
    public string field;
    public CompFunc compFunc;
    public float value;
}