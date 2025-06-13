using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class TextTreeToolbox
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
    public static string GetNextNodeKey(TextNodeData currentNode)
    {
        TextEdge nextEdge = GetNextEdge(currentNode);
        // TODO :: calls included Unity events
        return nextEdge.nextKey;
    }

    public static TextEdge GetNextEdge(TextNodeData textNodeData)
    {
        foreach (TextEdge edge in textNodeData.edgeList)
        {
            if (SatisfiedCoditions(edge.condList)) return edge;
        }
        Debug.LogError("No valid edge found for the current node.");
        return null;
    }

    public static bool SatisfiedCoditions(List<Condition> condArr)
    {
        if (condArr == null) { return true; }

        bool isValidNode = true;

        foreach (Condition cond in condArr)
        {
            var fieldInfo = GameContextToolbox.GetGameContextFieldInfo(cond.field);
            if (fieldInfo == null) { isValidNode = false; break; }

            isValidNode = CompareConditionValue(cond, fieldInfo);
            if (!isValidNode) break;
        }

        return isValidNode;
    }

    private static bool CompareConditionValue(Condition cond, FieldInfo fieldInfo)
    {
        try
        {
            GameContext gameContext = GameContextToolbox.FindGameContextIncluding(fieldInfo);
            var fieldValue = fieldInfo.GetValue(gameContext);

            Type targetType = fieldInfo.FieldType;
            object parsedCondValue = Convert.ChangeType(cond.value, targetType);

            IComparable left = fieldValue as IComparable;
            IComparable right = parsedCondValue as IComparable;

            if (left == null || right == null)
            {
                Debug.LogError("Comparison values are not IComparable.");
                return false;
            }

            int result = left.CompareTo(right);

            switch (cond.compFunc)
            {
                case CompFunc.Same: return result == 0;
                case CompFunc.Diff: return result != 0;
                case CompFunc.GreaterThan: return result > 0;
                case CompFunc.LessThan: return result < 0;
                case CompFunc.GreaterThanOrEqual: return result >= 0;
                case CompFunc.LessThanOrEqual: return result <= 0;
                default:
                    Debug.LogError($"Unknown comparison function: {cond.compFunc}");
                    return false;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error comparing field '{cond.field}': {ex.Message}");
            return false;
        }
    }
    #endregion
}