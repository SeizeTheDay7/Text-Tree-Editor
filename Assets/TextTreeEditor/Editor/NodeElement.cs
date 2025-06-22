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
        AddToClassList("Node");

        AddLabel("Actor Name", "script text");
    }

    // Constructor for loaded node from file
    public NodeElement(Vector2 pos, TextNodeData dataFromFile)
    {
        textNodeData = dataFromFile;
        style.left = pos.x;
        style.top = pos.y;

        AddToClassList("Node");

        AddLabel(textNodeData.actorName, textNodeData.text);
    }

    private void AddLabel(string actorName, string textdata)
    {
        Label actorNameLabel = new Label(actorName);
        Label textLabel = new Label(textdata);
        actorNameLabel.name = "actorname-label";
        textLabel.name = "text-label";

        actorNameLabel.AddToClassList("actorname-label");
        textLabel.AddToClassList("text-label");

        Add(actorNameLabel);
        Add(textLabel);
    }

    public void UpdateText(string textdata)
    {
        textNodeData.text = textdata;

        var textLabel = this.Q<Label>("text-label");
        textLabel.text = textdata;
    }

    public void UpdateActorName(string name)
    {
        textNodeData.actorName = name;

        var actorNameLabel = this.Q<Label>("actorname-label");
        actorNameLabel.text = name;
    }
}