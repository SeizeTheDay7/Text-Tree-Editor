using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;

public partial class TextTreeEditorWindow : EditorWindow
{
    bool isNodeMoving = false; // Window.cs


    // Window.cs
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
        if (narratorField.value == null) { Debug.Log("No narrator loaded"); return; }
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
            edgeList = new List<TextEdge>()
        };

        node.textNodeData = textNodeData;
        nodeElementDict[textNodeData.key] = node;
    }

    // UI.cs
    /// <summary>
    /// 1. Left click to select node
    /// 2. Right click to create new edge
    /// 3. Left click to connect two nodes
    /// </summary>
    private void SetNodeEvent(NodeElement node)
    {
        node.RegisterCallback<MouseDownEvent>(evt =>
        {
            // Left click + making edge
            if (evt.button == 0 && currentCreatingEdge != null && currentCreatingEdge.fromNode != node)
            {
                ConfirmEdge(node);
            }
            // Left click to select node
            else if (evt.button == 0)
            {
                isNodeMoving = true;

                ResetPick(ExceptFor.Nothing);
                SelectNode(node);
            }
            evt.StopPropagation();
        });

        // Right click to open menu
        var manipulator = new ContextualMenuManipulator(evt =>
        {
            evt.menu.AppendAction("Create Edge", a =>
            {
                ResetPick(ExceptFor.Nothing);
                MakeNewEdge(node);
            });

            evt.menu.AppendAction("Set Init Node", a =>
            {
                SetInitNode(node);
            });
        });

        node.AddManipulator(manipulator);
    }

    // UI.cs
    private void SetInitNode(NodeElement node)
    {
        if (narratorField.value == null) { Debug.Log("No narrator loaded"); return; }

        initNode?.RemoveFromClassList("init-node");
        initNode = node;
        node.AddToClassList("init-node");

        narratorField.value.GetComponent<TTNarrator>().textTreeSO.initNodeKey = node.textNodeData.key;
    }

    private void SelectNode(NodeElement node)
    {
        currentSelectNode = node;
        currentSelectNode.AddToClassList("highlighted");

        nodeTextField.style.display = DisplayStyle.Flex;
        nodeTextField.value = currentSelectNode.textNodeData.text;
        nodeActorDropdown.style.display = DisplayStyle.Flex;
        nodeActorDropdown.value = currentSelectNode.textNodeData.actorName;

        InitEventField(node.textNodeData.nodeEventList);
    }

    // Window.cs
    private void DeselectCurrentNode()
    {
        if (currentSelectNode != null) { currentSelectNode.RemoveFromClassList("highlighted"); }
        currentSelectNode = null;

        nodeTextField.style.display = DisplayStyle.None;
        nodeActorDropdown.style.display = DisplayStyle.None;

        ResetEventField();
    }
}