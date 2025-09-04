using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public partial class TextTreeEditorWindow : EditorWindow
{
    TempCondSO tempCondSO;
    TempEventSO tempEventSO; // UI.cs

    // Node.cs
    private void MakeNewEdge(NodeElement fromNode)
    {
        cursorElement.style.visibility = Visibility.Hidden;
        currentCreatingEdge = new EdgeElement(fromNode, edgeLayerElement, backgroundElement);
        BeginEdgeMoving();
    }

    // Window.cs
    private void DeleteTempEdge()
    {
        if (currentCreatingEdge == null) return;
        StopEdgeMouseMoving();
        currentCreatingEdge.RemoveFromHierarchy();
        currentCreatingEdge = null;
    }

    // Node.cs
    private void ConfirmEdge(NodeElement toNode)
    {
        currentCreatingEdge.ConfirmEdge(toNode);
        EdgeEvent(currentCreatingEdge);
        currentCreatingEdge = null;
        StopEdgeMouseMoving();
    }

    public void BeginEdgeMoving() => backgroundElement.RegisterCallback<MouseMoveEvent>(UpdateLine);
    public void StopEdgeMouseMoving() => backgroundElement.UnregisterCallback<MouseMoveEvent>(UpdateLine);
    private void UpdateLine(MouseMoveEvent evt)
    {
        if (currentCreatingEdge == null) return;
        currentCreatingEdge.UpdateLine(evt.localMousePosition);
    }

    // UI.cs
    private void EdgeEvent(EdgeElement edge)
    {
        VisualElement edgeClickArea = edge.clickArea;
        edgeClickArea.RegisterCallback<MouseDownEvent>(evt =>
            {
                if (evt.button == 0 && currentCreatingEdge == null) // left click
                {
                    SelectEdge(edge);
                    evt.StopPropagation();
                }
            }
        );
    }

    private void SelectEdge(EdgeElement edge)
    {
        ResetPick(ExceptFor.Edge);
        edge.Highlight = true;
        InitEdgeField(edge);

        if (currentSelectEdge != null) currentSelectEdge.Highlight = false;
        currentSelectEdge = edge;
    }

    // Window.cs
    private void DeselectCurrentEdge()
    {
        if (currentSelectEdge == null) return;
        currentSelectEdge.Highlight = false;
        currentSelectEdge = null;
        ResetCurrentEdgeField();
    }

    #region Edge Field

    private void InitEdgeField(EdgeElement edge)
    {
        // if (tempCondSO != null) DestroyImmediate(tempCondSO);
        // tempCondSO = CreateInstance<TempCondSO>();
        // tempCondSO.conditionList = edge.textEdge.condList;

        // so = new SerializedObject(tempCondSO);
        // SerializedProperty propertyToBind = so.FindProperty("conditionList");

        // conditionField.BindProperty(propertyToBind);

        InitEventField(edge.textEdge.edgeEventList);
    }

    // UI.cs
    private void ResetCurrentEdgeField()
    {
        // if (tempCondSO != null) DestroyImmediate(tempCondSO);
        // tempCondSO = null;
        // conditionField.Unbind();
        // conditionField.Clear();

        ResetEventField();
    }

    #endregion
}