using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

internal sealed class NodeElement : VisualElement
{
    public TextNodeData textNodeData;
    public List<EdgeElement> inEdge = new List<EdgeElement>();
    public List<EdgeElement> outEdge = new List<EdgeElement>();

    // Constructor for new node at cursor position
    public NodeElement(VisualElement contentLayer, VisualElement cursor)
    {
        if (cursor.visible)
        {
            Vector3 t = contentLayer.resolvedStyle.translate;

            float localX = cursor.resolvedStyle.left - t.x;
            float localY = cursor.resolvedStyle.top - t.y;

            style.left = localX + cursor.resolvedStyle.width * 0.5f;
            style.top = localY + cursor.resolvedStyle.height * 0.5f;
        }
        else
        {
            style.left = 200; // x coord
            style.top = 100; // y coord
        }

        textNodeData = new TextNodeData();

        // TODO :: need to add placeholder text

        AddToClassList("Node");
    }

    // Constructor for loaded node from file
    public NodeElement(Vector2 pos, TextNodeData dataFromFile)
    {
        textNodeData = dataFromFile;
        style.left = pos.x;
        style.top = pos.y;

        AddToClassList("Node");
    }
}