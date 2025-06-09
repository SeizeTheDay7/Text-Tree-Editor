using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System;

public partial class TextTreeEditorWindow : EditorWindow
{
    bool isNodeMoving = false;

    /// <summary>
    /// Add node at the cursor point (or init point if there's no visible cursor)
    /// </summary>
    private void AddNodeEvent()
    {
        addNodeButton = rootVisualElement.Q<Button>("addNodeButton");
        if (addNodeButton == null) { Debug.LogError("Button 'addNodeButton' not found!"); return; }
        addNodeButton.clicked += () => { AddNode(); };
    }

    private void AddNode()
    {
        if (textTreeField.value == null) { Debug.Log("No text tree loaded"); return; }
        var node = new NodeElement(contentLayerElement, cursorElement);
        AddNewNodeToTextTree(node);

        SetNodeEvent(node);
        nodeLayerElement.Add(node);
    }

    /// <summary>
    /// Add new node to text tree dict
    /// </summary>
    private void AddNewNodeToTextTree(NodeElement node)
    {
        if (node == null || nodeElementDict == null) { Debug.LogError("Node or nodeElementDict is null"); return; }

        var textNodeData = new TextNodeData
        {
            key = Guid.NewGuid().ToString(),
            text = "",
            position = new Vector2(
                node.style.left.value.value,
                node.style.top.value.value
            ),
            nextNodes = new List<TextEdge>()
        };

        node.textNodeData = textNodeData;
        nodeElementDict[textNodeData.key] = node;
    }

    /// <summary>
    /// 1. Left click to select node
    /// 2. Right click to create new edge
    /// 3. Left click to connect two nodes
    /// </summary>
    private void SetNodeEvent(NodeElement node)
    {
        node.RegisterCallback<MouseDownEvent>(evt =>
        {
            if (evt.button == 0 && currentCreatingEdge != null && currentCreatingEdge.fromNode != node)
            {
                ConfirmEdge(node);
            }
            else if (evt.button == 0)
            {
                isNodeMoving = true;

                DeselectCurrentNode();
                SelectNode(node);
            }
            evt.StopPropagation();
        });

        var manipulator = new ContextualMenuManipulator(evt =>
        {
            evt.menu.AppendAction("Create Edge", a =>
            {
                MakeNewEdge(node);
            });
        });

        node.AddManipulator(manipulator);
    }

    private void SelectNode(NodeElement node)
    {
        currentSelectNode = node;
        currentSelectNode.AddToClassList("highlighted");
        nodeTextField.value = currentSelectNode.textNodeData.text;
    }

    private void DeselectCurrentNode()
    {
        if (currentSelectNode != null) { currentSelectNode.RemoveFromClassList("highlighted"); }
        currentSelectNode = null;
        nodeTextField.value = "";
    }
}