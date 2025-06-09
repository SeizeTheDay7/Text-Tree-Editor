using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;

public partial class TextTreeEditorWindow : EditorWindow
{
    private ObjectField textTreeField;
    private TextField nodeTextField;
    private Dictionary<string, NodeElement> nodeElementDict;

    private void SetUI()
    {
        SetTextTreeField();
        SetTextField();

        TextTreeFieldEvent();
        NodeTextFieldEvent();
        NewTextTreeButtonEvent();
        SaveTextTreeButtonEvent();
    }

    private void SetTextTreeField()
    {
        textTreeField = rootVisualElement.Q<ObjectField>("TextTreeField");
        if (textTreeField == null) Debug.LogError("TextTreeField not found");
        textTreeField.objectType = typeof(TextAsset);
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

            // 2. Load data from JSON file
            var textNodeDataList = JSONManager.LoadJsonToList(evt.newValue as TextAsset);
            if (textNodeDataList == null) { Debug.LogError("Failed to load text tree data"); return; }

            // 3. Ready new scene and fill up the dictionary
            nodeElementDict = new Dictionary<string, NodeElement>();
            LoadNode(textNodeDataList);
            LoadEdge();

            // 4. Reset state values
            currentSelectNode = null;
            currentCreatingEdge = null;
            currentSelectEdge = null;
            nodeTextField.value = "";
        });
    }
    private void LoadNode(List<TextNodeData> textNodeDataList)
    {
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
            foreach (var edge in fromNode.textNodeData.nextNodes)
            {
                var toNode = nodeElementDict[edge.nextKey];
                var edgeElement = new EdgeElement(fromNode, toNode, edgeLayerElement, backgroundElement);
                EdgeEvent(edgeElement);
                edgeLayerElement.Add(edgeElement);
            }
        }
    }

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

        newTextTreeButton.clicked += () => JSONManager.CreateNewTreeFile(textTreeField);
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
        JSONManager.SaveTreeToJson(nodeElementDict, textTreeField);
    }

}