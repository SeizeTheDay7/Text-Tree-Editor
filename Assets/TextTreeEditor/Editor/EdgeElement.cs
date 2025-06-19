using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using System.Collections.Generic;

internal sealed class EdgeElement : VisualElement
{

    private VisualElement background;
    private VisualElement edgeLayer;
    public NodeElement fromNode;
    private NodeElement toNode;
    public TextEdge textEdge; // reference of TextEdge in fromNode
    private Vector2 mousePosition;
    private bool _highlight;
    public bool Highlight
    {
        get => _highlight;
        set
        {
            if (_highlight == value) return;
            _highlight = value;
            style.backgroundColor = value ? Color.yellow : Color.white;
            MarkDirtyRepaint();
        }
    }
    const float renderThickness = 2f;
    const float clickThickness = 13f;
    const float HeadSize = 10f;
    public VisualElement clickArea;

    #region Constructor
    private void Init(VisualElement edgeLayer, VisualElement background)
    {
        name = "edge";
        this.edgeLayer = edgeLayer;
        this.background = background;
        edgeLayer.Add(this);
        AddClickArea();

        style.position = Position.Absolute;
        style.height = renderThickness;
        style.backgroundColor = Color.white;
        style.transformOrigin = new TransformOrigin(0, .5f, 0); // left center
        pickingMode = PickingMode.Ignore; // to only gets event from clickArea
    }

    private void AddClickArea()
    {
        clickArea = new VisualElement();
        clickArea.style.position = Position.Absolute;
        clickArea.style.left = 0;
        clickArea.style.top = 0;
        clickArea.style.width = new Length(100, LengthUnit.Percent);
        clickArea.style.height = clickThickness;
        clickArea.style.translate = new Translate(0, -clickThickness * 0.5f);
        clickArea.style.backgroundColor = Color.clear;
        this.Add(clickArea);
    }

    // Constructor for new edge. Not getting click until confirm edge.
    public EdgeElement(NodeElement from, VisualElement edgeLayer, VisualElement background)
    {
        Init(edgeLayer, background);
        clickArea.pickingMode = PickingMode.Ignore;
        fromNode = from;

        generateVisualContent += OnGenerate;
    }

    // Constructor for loaded edge
    public EdgeElement(NodeElement from, NodeElement to, TextEdge textEdge, VisualElement edgeLayer, VisualElement background)
    {
        Init(edgeLayer, background);
        fromNode = from;
        toNode = to;
        this.textEdge = textEdge;

        AddEdgeRef();
        generateVisualContent += OnGenerate;
        RegisterCallbackOnce<GeometryChangedEvent>(_ => LayoutBody());
    }

    #endregion

    #region Confirm edge
    public void ConfirmEdge(NodeElement to)
    {
        clickArea.pickingMode = PickingMode.Position;

        toNode = to;
        TextEdge newTextEdge = new TextEdge
        {
            nextKey = toNode.textNodeData.key,
            conditionList = new List<Condition>(),
            edgeEventList = new List<TTEvent>()
        };
        fromNode.textNodeData.edgeList.Add(newTextEdge);
        textEdge = newTextEdge;

        AddEdgeRef();
        LayoutBody();
    }

    private void AddEdgeRef()
    {
        fromNode.outEdge.Add(this);
        toNode.inEdge.Add(this);
    }

    // TODO :: Call this function when delete this edge
    private void RemoveEdgeRef()
    {
        fromNode.outEdge.Remove(this);
        toNode.inEdge.Remove(this);
    }
    #endregion

    #region Update rendering

    // Line geometry update for a new edge
    public void UpdateLine(Vector2 mouseLocalInBg)
    {
        mousePosition = background.ChangeCoordinatesTo(edgeLayer, mouseLocalInBg);
        LayoutBody();
    }

    // Line geometry update while moving the node
    public void UpdateLine() => LayoutBody();

    private void LayoutBody()
    {
        Vector2 start = edgeLayer.WorldToLocal(new Vector2(fromNode.worldBound.center.x, fromNode.worldBound.yMax));
        Vector2 end = (toNode == null)
            ? mousePosition
            : edgeLayer.WorldToLocal(new Vector2(toNode.worldBound.center.x, toNode.worldBound.yMin));

        Vector2 dir = end - start;
        float len = dir.magnitude;
        float angleDeg = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        style.left = start.x;
        style.top = start.y;
        style.width = len;
        style.rotate = new Rotate(new Angle(angleDeg));

        MarkDirtyRepaint();
    }

    // For drawing arrowhead
    private void OnGenerate(MeshGenerationContext ctx)
    {
        var p = ctx.painter2D;
        float h = resolvedStyle.height;     // BodyThickness

        Vector2 tip = new(resolvedStyle.width, h * .5f);
        Vector2 dir = Vector2.right;
        Vector2 n = new(0, 1);

        Vector2 b = tip - dir * HeadSize + n * HeadSize * .5f;
        Vector2 c = tip - dir * HeadSize - n * HeadSize * .5f;

        p.fillColor = Highlight ? Color.yellow : Color.white;
        p.BeginPath();
        p.MoveTo(tip); p.LineTo(b); p.LineTo(c);
        p.ClosePath(); p.Fill();
    }
    #endregion
}
