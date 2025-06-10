using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

public partial class TextTreeEditorWindow : EditorWindow
{
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

    private void DeselectCurrentEdge()
    {
        if (currentSelectEdge == null) return;
        currentSelectEdge.Highlight = false;
        currentSelectEdge = null;
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

    private void InitEdgeField(EdgeElement edge)
    {
        TempCondSO tempSO = CreateInstance<TempCondSO>();
        tempSO.conditionList = edge.conditionList;
        SerializedObject so = new SerializedObject(tempSO);

    }
}