using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class TextTreeUtil
{
    #region Find next edge

    /// <summary>
    /// When nodes are choices itself, receive the choice and go to the node.
    /// For usual visual novel games.
    /// </summary>
    // public static TextNodeData SelectNextNode(string nextNodeText)
    // {
    //     // Not Implemented yet
    // }


    /// <summary>
    /// Validate the current node's edges and determine the next node
    /// </summary>
    // public static string GetNextNodeKey(TextNodeData currentNode)
    // {
    //     TextEdge nextEdge = GetNextEdge(currentNode);
    //     // TODO :: calls included Unity events
    //     return nextEdge.nextKey;
    // }

    // public static TextEdge GetNextEdge(TextNodeData textNodeData)
    // {
    //     foreach (TextEdge edge in textNodeData.edgeList)
    //     {
    //         if (SatisfiedCoditions(edge.condList)) return edge;
    //     }
    //     Debug.LogError("No valid edge found for the current node.");
    //     return null;
    // }
    #endregion
}