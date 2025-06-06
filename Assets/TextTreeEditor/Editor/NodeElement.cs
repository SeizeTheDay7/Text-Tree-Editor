using UnityEngine;
using UnityEngine.UIElements;

internal sealed class NodeElement : VisualElement
{
    public TextNodeData textNodeData;

    public NodeElement(VisualElement contentLayer, VisualElement cursor, bool isNew)
    {
        if (cursor.visible)
        {
            Vector3 t = contentLayer.resolvedStyle.translate;

            float localX = cursor.resolvedStyle.left - t.x;
            float localY = cursor.resolvedStyle.top - t.y;

            this.style.left = localX + cursor.resolvedStyle.width * 0.5f;
            this.style.top = localY + cursor.resolvedStyle.height * 0.5f;
        }
        else
        {
            this.style.left = 200; // x coord
            this.style.top = 100; // y coord
        }

        // TODO :: need to add placeholder text

        this.AddToClassList("Node");

        if (isNew) textNodeData = new TextNodeData();
        // else textNode = 
    }
}