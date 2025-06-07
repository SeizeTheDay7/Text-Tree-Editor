using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;

public partial class TextTreeEditorWindow : EditorWindow
{
    private ObjectField textTreeField;
    private TextField nodeTextField;
    private Dictionary<string, TextNodeData> textTreeDataDict; // Used for saving

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
            contentLayerElement.style.translate = new StyleTranslate(new Translate(0, 0, 0));
            nodeLayerElement.Clear();
            edgeLayerElement.Clear();
            if (evt.newValue == null) { return; }

            textTreeDataDict = JSONManager.LoadJsonToDict(evt.newValue as TextAsset);
            if (textTreeDataDict == null) { Debug.LogError("Failed to load text tree data"); return; }
            InitContentLayer();

            currentSelectNode = null;
            currentEdge = null;
            nodeTextField.value = "";
        });
    }

    private void InitContentLayer()
    {
        if (textTreeDataDict == null) return;
        LoadNode(); // Calls LoadEdge() at the end
    }

    private void LoadNode()
    {
        var tempNodeElementDict = new Dictionary<string, NodeElement>();
        foreach (var kvp in textTreeDataDict) // kvp : key value pair
        {
            var nodeElement = new NodeElement(kvp.Value.position, kvp.Value);
            SetNodeEvent(nodeElement);
            nodeLayerElement.Add(nodeElement);
            tempNodeElementDict[kvp.Key] = nodeElement;
        }

        LoadEdge(tempNodeElementDict);
    }

    private void LoadEdge(Dictionary<string, NodeElement> tempNodeElementDict)
    {
        foreach (var kvp in textTreeDataDict)
        {
            var textNodeData = kvp.Value;
            var fromNode = tempNodeElementDict[textNodeData.key];
            foreach (var edge in textNodeData.nextNodes)
            {
                var toNode = tempNodeElementDict[edge.nextKey];
                var edgeElement = new EdgeElement(fromNode, toNode, edgeLayerElement, backgroundElement);
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
        if (textTreeDataDict == null) { Debug.LogError("textNodeDict is null"); return; }
        JSONManager.SaveTreeToJson(textTreeDataDict, textTreeField);
    }

}