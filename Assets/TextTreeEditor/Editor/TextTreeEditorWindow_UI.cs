using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;

public partial class TextTreeEditorWindow : EditorWindow
{
    private ObjectField narratorField; // Node.cs
    private DropdownField nodeTypeDropdown;
    private DropdownField nodeActorDropdown; // Node.cs
    private TextField nodeTextField; // Node.cs
    private PropertyField conditionListField;
    private PropertyField eventListField; // Node.cs, Window.cs
    private Dictionary<string, NodeElement> nodeElementDict; // Node.cs, Window.cs
    private TTNarrator narrator; // Node.cs, Window.cs

    // Window.cs
    private void SetUI()
    {
        InitTTNarratorField();
        InitNodeTypeDropdown();
        InitNodeActorDropdown();
        InitTextField();
        InitConditionListField();
        InitEventListField();

        SaveTextTreeButtonEvent();

        NarratorFieldEvent();
        NodeTextFieldEvent();
    }

    #region Allocate element

    private void InitTTNarratorField()
    {
        narratorField = rootVisualElement.Q<ObjectField>("NarratorField");
        if (narratorField == null) Debug.LogError("TextTreeManagerField not found");
        narratorField.objectType = typeof(TTNarrator);
    }

    private void InitNodeTypeDropdown()
    {
        nodeTypeDropdown = rootVisualElement.Q<DropdownField>("NodeTypeDropdown");
        if (nodeTypeDropdown == null) Debug.LogError("NodeTypeDropdown not found");

        nodeTypeDropdown.choices = new List<string> { "Text", "Choice" };
        nodeTypeDropdown.RegisterValueChangedCallback(evt =>
        {
            if (currentSelectNode == null) return;
            if (evt.newValue == "Text") currentSelectNode.UpdateNodeType(NodeType.Text);
            else if (evt.newValue == "Choice") currentSelectNode.UpdateNodeType(NodeType.Choice);
        });

        nodeTypeDropdown.style.display = DisplayStyle.None;
    }

    private void InitNodeActorDropdown()
    {
        nodeActorDropdown = rootVisualElement.Q<DropdownField>("NodeActorDropdown");
        if (nodeActorDropdown == null) Debug.LogError("NodeActorDropdown not found");

        nodeActorDropdown.RegisterValueChangedCallback(evt =>
        {
            if (currentSelectNode == null) return;
            currentSelectNode.UpdateActorName(evt.newValue);
        });

        nodeActorDropdown.style.display = DisplayStyle.None;
    }

    private void InitTextField()
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

    private void InitConditionListField()
    {
        conditionListField = rootVisualElement.Q<PropertyField>("ConditionListField");
        if (conditionListField == null) Debug.LogError("ConditionField not found");
    }

    private void InitEventListField()
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
            LoadNode(narrator.textTreeSO);
            LoadEdge();

            // 3. Reset state values
            currentSelectNode = null;
            currentCreatingEdge = null;
            currentSelectEdge = null;
            nodeActorDropdown.choices = GetTTActorNameList();
            nodeTextField.value = "";
            ResetCurrentEdgeField();
        });
    }
    #endregion

    #region Load node & edge
    private void LoadNode(TextTreeSO textTreeSO)
    {
        List<TextNodeData> textNodeDataList = textTreeSO.textNodeList;
        if (textNodeDataList == null) return;

        foreach (var textNodeData in textNodeDataList)
        {
            var nodeElement = new NodeElement(textNodeData.position, textNodeData);
            SetNodeEvent(nodeElement);
            nodeLayerElement.Add(nodeElement);
            nodeElementDict[textNodeData.key] = nodeElement;

            if (textTreeSO.initNodeKey == textNodeData.key) { SetInitNode(nodeElement); }
        }
    }

    private void LoadEdge()
    {
        foreach (var kvp in nodeElementDict) // kvp : key value pair
        {
            var fromNode = kvp.Value;
            foreach (var edge in fromNode.textNodeData.edgeList)
            {
                if (!nodeElementDict.ContainsKey(edge.nextKey)) continue; // Skip if next node not found
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
            currentSelectNode.UpdateText(evt.newValue);
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

    #region Init / Deselect Field

    // Edge.cs, Node.cs
    private void InitEventField(List<TTDEvent> eventList)
    {
        if (tempEventSO != null) DestroyImmediate(tempEventSO);
        tempEventSO = CreateInstance<TempEventSO>();
        tempEventSO.eventList = eventList;

        SerializedObject so = new SerializedObject(tempEventSO);
        SerializedProperty propertyToBind = so.FindProperty("eventList");

        eventListField.BindProperty(propertyToBind);
    }

    // Edge.cs, Node.cs
    private void ResetEventField()
    {
        if (tempEventSO != null) DestroyImmediate(tempEventSO);
        tempEventSO = null;
        eventListField.Unbind();
        eventListField.Clear();
    }

    #endregion
}