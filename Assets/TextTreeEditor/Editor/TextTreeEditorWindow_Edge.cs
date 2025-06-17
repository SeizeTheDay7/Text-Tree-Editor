using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public partial class TextTreeEditorWindow : EditorWindow
{
    TempCondSO tempCondSO;
    TempEventSO tempEventSO;
    SerializedObject condSO;
    SerializedObject eventSO;

    private void MakeNewEdge(NodeElement fromNode)
    {
        cursorElement.style.visibility = Visibility.Hidden;
        currentCreatingEdge = new EdgeElement(fromNode, edgeLayerElement, backgroundElement);
        BeginEdgeMoving();
    }

    private void DeleteTempEdge()
    {
        if (currentCreatingEdge == null) return;
        StopEdgeMouseMoving();
        currentCreatingEdge.RemoveFromHierarchy();
        currentCreatingEdge = null;
    }

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
        // Init condition field
        if (tempCondSO != null) DestroyImmediate(tempCondSO);
        tempCondSO = CreateInstance<TempCondSO>();
        tempCondSO.conditionList = edge.textEdge.condList;

        condSO = new SerializedObject(tempCondSO);
        SerializedProperty propertyToBind = condSO.FindProperty("conditionList");
        conditionDrawer.BindProperty(propertyToBind);

        // Init event field
        if (tempEventSO != null) DestroyImmediate(tempEventSO);
        tempEventSO = CreateInstance<TempEventSO>();
        tempEventSO.eventList = edge.textEdge.edgeEventList;

        eventSO = new SerializedObject(tempEventSO);
        SerializedProperty eventPropertyToBind = eventSO.FindProperty("eventList");
        eventDrawer.BindProperty(eventPropertyToBind);
    }

    private void ResetCurrentEdgeField()
    {
        if (tempCondSO != null) DestroyImmediate(tempCondSO);
        tempCondSO = null;
        condSO = null;
        conditionDrawer.Unbind();
        conditionDrawer.Clear();

        if (tempEventSO != null) DestroyImmediate(tempEventSO);
        tempEventSO = null;
        eventSO = null;
        eventDrawer.Unbind();
        eventDrawer.Clear();
    }

    #endregion
}