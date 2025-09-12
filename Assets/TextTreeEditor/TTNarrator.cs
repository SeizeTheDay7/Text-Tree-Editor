using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class TTNarrator : MonoBehaviour
{
    [Header("Text & Actors")]
    public TextTreeSO textTreeSO;
    public List<TTActor> actorList;

    [Header("Cache")]
    Dictionary<string, TextNodeData> ttdict;
    TextNodeData currentNode;

    void Awake()
    {
        SetUpTTSO();
    }

    private void SetUpTTSO()
    {
        ttdict = new Dictionary<string, TextNodeData>();
        foreach (var nodeData in textTreeSO.textNodeList)
        {
            ttdict[nodeData.key] = nodeData;
        }
    }

    public TTActor GetActorByName(string name)
    {
        foreach (TTActor actor in actorList)
        {
            if (actor.actorName == name) return actor;
        }
        Debug.LogError("Actor not found: " + name);
        return null;
    }

    public TextNodeData GetCurrentNode()
    {
        if (currentNode == null)
        {
            currentNode = ttdict[textTreeSO.initNodeKey];
        }
        return currentNode;
    }

    // Usage : When the next node is choice type, show up Choice UI
    // Or the node is just a text node, proceed to next dialogue

    /// <summary>
    /// Return next node
    /// </summary>
    public TextNodeData GetNextNodeSample()
    {
        if (currentNode == null)
        {
            return ttdict[textTreeSO.initNodeKey];
        }
        else if (currentNode.edgeList.Count == 0)
        {
            return null;
        }
        else
        {
            return ttdict[currentNode.edgeList[0].nextKey];
        }
    }

    public TextNodeData GetNodeByKey(string key)
    {
        if (ttdict.ContainsKey(key)) return ttdict[key];
        Debug.LogError("Node not found: " + key);
        return null;
    }

    public void GoToNextNode()
    {
        currentNode = GetNextNodeSample();
    }

    public void GoToNextNode(TextNodeData node)
    {
        currentNode = node;
    }
}