using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;

public partial class TextTreeEditorWindow : EditorWindow
{
    private ObjectField narratorField;
    private TextField nodeTextField;
    private PropertyField conditionListField;
    private PropertyField eventListField;
    private Dictionary<string, NodeElement> nodeElementDict;
    private TTNarrator narrator;

    private void SetUI()
    {
        SetTTNarratorField();
        SetTextField();
        SetConditionListField();
        SetEventListField();

        SaveTextTreeButtonEvent();

        NarratorFieldEvent();
        NodeTextFieldEvent();
    }

    #region Allocate element

    private void SetTTNarratorField()
    {
        narratorField = rootVisualElement.Q<ObjectField>("NarratorField");
        if (narratorField == null) Debug.LogError("TextTreeManagerField not found");
        narratorField.objectType = typeof(TTNarrator);
    }

    private void SetTextField()
    {
        nodeTextField = rootVisualElement.Q<TextField>("NodeText");
        if (nodeTextField == null) Debug.LogError("NodeText not found");
        nodeTextField.multiline = true;
        nodeTextField.verticalScrollerVisibility = ScrollerVisibility.AlwaysVisible; // Resolves height reset bug

        var input = nodeTextField.Q("unity-text-input");
        input.style.unityTextAlign = TextAnchor.UpperLeft; // Resolves line break bug
        input.style.whiteSpace = WhiteSpace.Pre;

        nodeTextField.style.display = DisplayStyle.None;
    }

    private void SetConditionListField()
    {
        conditionListField = rootVisualElement.Q<PropertyField>("ConditionListField");
        if (conditionListField == null) Debug.LogError("ConditionField not found");
    }

    private void SetEventListField()
    {
        eventListField = rootVisualElement.Q<PropertyField>("EventListField");
        if (eventListField == null) Debug.LogError("EventField not found");
    }
    #endregion

    #region Tree field event
    /// <summary>
    /// Occurs when new Narrator allocated
    /// </summary>
    private void NarratorFieldEvent()
    {
        narratorField.RegisterValueChangedCallback(evt =>
        {
            // 1. Reset the layers unconditionally
            contentLayerElement.style.translate = new StyleTranslate(new Translate(0, 0, 0));
            nodeLayerElement.Clear();
            edgeLayerElement.Clear();
            if (evt.newValue == null) { return; }

            // 2. Draw nodes and edges from text tree
            narrator = narratorField.value as TTNarrator;
            nodeElementDict = new Dictionary<string, NodeElement>();
            LoadNode(narrator.textTreeSO.textNodeList);
            LoadEdge();

            // 3. Reset state values
            currentSelectNode = null;
            currentCreatingEdge = null;
            currentSelectEdge = null;
            nodeTextField.value = "";
            ResetCurrentEdgeField();
        });
    }
    #endregion

    #region Load node & edge
    private void LoadNode(List<TextNodeData> textNodeDataList)
    {
        if (textNodeDataList == null) return;

        foreach (var textNodeData in textNodeDataList)
        {
            var nodeElement = new NodeElement(textNodeData.position, textNodeData);
            SetNodeEvent(nodeElement);
            nodeLayerElement.Add(nodeElement);
            nodeElementDict[textNodeData.key] = nodeElement;
        }
    }

    private void LoadEdge()
    {
        foreach (var kvp in nodeElementDict) // kvp : key value pair
        {
            var fromNode = kvp.Value;
            foreach (var edge in fromNode.textNodeData.edgeList)
            {
                var toNode = nodeElementDict[edge.nextKey];
                var edgeElement = new EdgeElement(fromNode, toNode, edge, edgeLayerElement, backgroundElement);
                EdgeEvent(edgeElement);
            }
        }
    }
    #endregion

    #region Field, Button event

    /// <summary>
    /// Occurs when text has edited
    /// </summary>
    private void NodeTextFieldEvent()
    {
        nodeTextField.RegisterValueChangedCallback(evt =>
        {
            if (currentSelectNode == null) return;
            currentSelectNode.textNodeData.text = evt.newValue;
        });
    }

    private void SaveTextTreeButtonEvent()
    {
        var saveTextTreeButton = rootVisualElement.Q<Button>("SaveTextTreeButton");
        if (saveTextTreeButton == null) Debug.LogError("SaveTextTreeButton not found");

        saveTextTreeButton.clicked += () => SaveTextTree();
    }

    private void SaveTextTree()
    {
        if (nodeElementDict == null) { Debug.LogError("nodeElementDict is null"); return; }
        TextTreeSOUtil.SaveTreeToSO(nodeElementDict, narrator.textTreeSO);
    }
    #endregion
}