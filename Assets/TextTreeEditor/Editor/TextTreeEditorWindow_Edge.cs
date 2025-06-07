using UnityEditor;
using UnityEngine.UIElements;

public partial class TextTreeEditorWindow : EditorWindow
{
    private void MakeNewEdge(NodeElement fromNode)
    {
        currentEdge = new EdgeElement(fromNode, contentLayerElement, backgroundElement);
        BeginEdgeMoving();
        edgeLayerElement.Add(currentEdge);
    }

    private void DeleteTempEdge()
    {
        if (currentEdge == null) return;
        StopEdgeMouseMoving();
        currentEdge.RemoveFromHierarchy();
        currentEdge = null;
    }

    private void ConfirmEdge(NodeElement toNode)
    {
        currentEdge.ConfirmEdge(toNode);
        currentEdge = null;
        StopEdgeMouseMoving();
    }

    public void BeginEdgeMoving() => backgroundElement.RegisterCallback<MouseMoveEvent>(UpdateLine);
    public void StopEdgeMouseMoving() => backgroundElement.UnregisterCallback<MouseMoveEvent>(UpdateLine);
    private void UpdateLine(MouseMoveEvent evt)
    {
        if (currentEdge == null) return;
        currentEdge.UpdateLine(evt.localMousePosition);
    }
}