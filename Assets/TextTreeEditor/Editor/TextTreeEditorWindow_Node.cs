using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System;

public partial class TextTreeEditorWindow : EditorWindow
{
    /// <summary>
    /// Add node at the cursor point (or init point if there's no cursor)
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
        var node = new NodeElement(contentLayerElement, cursorElement, isNew: true);
        AddNewNodeToTextTree(node);

        SetNodeEvent(node);
        contentLayerElement.Add(node);
    }

    /// <summary>
    /// Add new node to text tree dictionary
    /// </summary>
    private void AddNewNodeToTextTree(NodeElement node)
    {
        if (node == null || textTreeDataList == null) { Debug.LogError("Node or textNodeDict is null"); return; }

        var textNodeData = new TextNodeData
        {
            key = Guid.NewGuid().ToString(),
            text = "",
            position = node.resolvedStyle.left * Vector2.right + node.resolvedStyle.top * Vector2.up,
            nextNodes = new List<TextEdge>()
        };

        // shallow copy. node and dict has the same reference to textNodeData.
        node.textNodeData = textNodeData;
        textTreeDataList.Add(textNodeData);
    }

    /// <summary>
    /// 1. Left click to select node
    /// 2. Right click to create new connection
    /// 3. Left click to connect two nodes
    /// </summary>
    private void SetNodeEvent(NodeElement node)
    {
        node.RegisterCallback<MouseDownEvent>(evt =>
        {
            if (evt.button == 0 && currentConnection != null && currentConnection.fromNode != node)
            {
                ConfirmConnection(node);
            }
            if (evt.button == 0)
            {
                DeselectCurrentNode();
                SelectNode(node);
                evt.StopPropagation();
            }
        });

        var manipulator = new ContextualMenuManipulator(evt =>
        {
            evt.menu.AppendAction("Create Connection", a =>
            {
                MakeNewConnection(node);
            });
        });

        node.AddManipulator(manipulator);
    }

    private void SelectNode(NodeElement node)
    {
        currentSelectNode = node;
        currentSelectNode.AddToClassList("highlighted");
        // nodeTextField.value = ;
    }

    private void DeselectCurrentNode()
    {
        if (currentSelectNode != null) { currentSelectNode.RemoveFromClassList("highlighted"); }
        currentSelectNode = null;
    }
}