using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;

public partial class TextTreeEditorWindow : EditorWindow
{
    private ObjectField textTreeField;
    private TextField nodeTextField;
    private PropertyField eventDrawer;
    private PropertyField conditionDrawer;
    private Dictionary<string, NodeElement> nodeElementDict;

    private void SetUI()
    {
        SetTextTreeField();
        SetTextField();
        SetEdgeField();

        TextTreeFieldEvent();
        NodeTextFieldEvent();
        NewTextTreeButtonEvent();
        SaveTextTreeButtonEvent();
    }

    #region Allocate element
    private void SetTextTreeField()
    {
        textTreeField = rootVisualElement.Q<ObjectField>("TextTreeField");
        if (textTreeField == null) Debug.LogError("TextTreeField not found");
        textTreeField.objectType = typeof(TextTreeSO);
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
    }

    private void SetEdgeField()
    {
        conditionDrawer = rootVisualElement.Q<PropertyField>("ConditionDrawer");
        if (conditionDrawer == null) Debug.LogError("ConditionField not found");

        eventDrawer = rootVisualElement.Q<PropertyField>("EventDrawer");
        if (eventDrawer == null) Debug.LogError("EventDrawer not found");
    }
    #endregion

    #region Tree field event
    /// <summary>
    /// Occurs when new text tree allocated
    /// </summary>
    private void TextTreeFieldEvent()
    {
        textTreeField.RegisterValueChangedCallback(evt =>
        {
            // 1. Reset the layers unconditionally
            contentLayerElement.style.translate = new StyleTranslate(new Translate(0, 0, 0));
            nodeLayerElement.Clear();
            edgeLayerElement.Clear();
            if (evt.newValue == null) { return; }

            // 2. Draw nodes and edges from text tree
            TextTreeSO textTreeSO = textTreeField.value as TextTreeSO;
            nodeElementDict = new Dictionary<string, NodeElement>();
            LoadNode(textTreeSO.textNodeList);
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

    #region Field event

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

    private void NewTextTreeButtonEvent()
    {
        var newTextTreeButton = rootVisualElement.Q<Button>("NewTextTreeButton");
        if (newTextTreeButton == null) Debug.LogError("NewTextTreeButton not found");

        newTextTreeButton.clicked += () => TextTreeSOUtil.CreateNewTreeFile(textTreeField);
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
        TextTreeSOUtil.SaveTreeToSO(nodeElementDict, textTreeField);
    }
    #endregion

}