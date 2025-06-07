using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

internal sealed class EdgeElement : VisualElement
{
    private VisualElement background;
    private VisualElement edgeLayer;
    public NodeElement fromNode;
    private NodeElement toNode;
    private Vector2 mousePosition;

    // Constructor for new edge
    public EdgeElement(NodeElement from, VisualElement edgeLayer, VisualElement background)
    {
        fromNode = from;
        this.edgeLayer = edgeLayer;
        this.background = background;
        name = "edge";
        generateVisualContent += OnGenerate;
    }

    // Constructor for loaded edge
    public EdgeElement(NodeElement from, NodeElement to, VisualElement edgeLayer, VisualElement background)
    {
        fromNode = from;
        toNode = to;
        this.edgeLayer = edgeLayer;
        this.background = background;
        name = "edge";
        generateVisualContent += OnGenerate;
    }

    public void UpdateLine(Vector2 mouseLocalInBg)
    {
        mousePosition = edgeLayer.WorldToLocal(background.LocalToWorld(mouseLocalInBg));
        MarkDirtyRepaint();
    }
    public void ConfirmEdge(NodeElement to)
    {
        toNode = to;
        fromNode.textNodeData.nextNodes.Add(new TextEdge
        {
            nextKey = toNode.textNodeData.key,
            condArr = new List<Condition>()
        });
        MarkDirtyRepaint();
    }

    private void OnGenerate(MeshGenerationContext ctx)
    {
        var p = ctx.painter2D;
        p.lineWidth = 2;
        p.strokeColor = Color.white;
        p.fillColor = Color.white;

        // coordinate
        Vector2 start = edgeLayer.WorldToLocal(new Vector2(fromNode.worldBound.center.x, fromNode.worldBound.yMax));
        Vector2 end = (toNode == null)
            ? mousePosition
            : edgeLayer.WorldToLocal(new Vector2(toNode.worldBound.center.x, toNode.worldBound.yMin));

        // line
        p.BeginPath();
        p.MoveTo(start);
        p.LineTo(end);
        p.Stroke();
        // Vector2 h = new(Mathf.Abs(end.x - start.x) * .5f, 0);
        // p.BezierCurveTo(start + h, end - h, end);

        // arrowhead
        const float head = 10f; // Increased from 6f to 10f
        Vector2 dir = (end - start).normalized;
        Vector2 n = new(-dir.y, dir.x);

        Vector2 b = end - dir * head + n * head * .5f;
        Vector2 c = end - dir * head - n * head * .5f;

        p.BeginPath();
        p.MoveTo(end); p.LineTo(b); p.LineTo(c);
        p.ClosePath(); p.Fill();
    }
}
