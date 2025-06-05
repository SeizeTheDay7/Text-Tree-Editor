using UnityEngine;
using UnityEngine.UIElements;

internal sealed class ConnectionElement : VisualElement
{
    public readonly VisualElement fromNode;
    private readonly VisualElement _bg;
    private readonly VisualElement _root;
    private VisualElement _toNode;
    private Vector2 _tempLocal;
    private bool _isTemp = true;

    public ConnectionElement(VisualElement from, VisualElement root, VisualElement bg)
    {
        fromNode = from;
        _root = root;
        _bg = bg;
        pickingMode = PickingMode.Ignore;
        name = "connection";
        generateVisualContent += OnGenerate;
    }

    public void UpdateLine(Vector2 mouseLocalInBg)
    {
        _tempLocal = _root.WorldToLocal(_bg.LocalToWorld(mouseLocalInBg));
        MarkDirtyRepaint();
    }
    public void ConfirmConnection(VisualElement to)
    {
        _toNode = to;
        _isTemp = false;
        MarkDirtyRepaint();
    }

    private void OnGenerate(MeshGenerationContext ctx)
    {
        var p = ctx.painter2D;
        p.lineWidth = 2;
        p.strokeColor = Color.white;
        p.fillColor = Color.white;               // ✓ 모든 버전 동일

        // ── 좌표
        Vector2 start = _root.WorldToLocal(fromNode.worldBound.center);
        Vector2 end = _isTemp ? _tempLocal
                                : _root.WorldToLocal(_toNode.worldBound.center);

        // ── 베지어
        Vector2 h = new(Mathf.Abs(end.x - start.x) * .5f, 0);
        p.BeginPath();
        p.MoveTo(start);
        p.BezierCurveTo(start + h, end - h, end);
        p.Stroke();

        // ── 화살촉
        const float head = 6f;
        Vector2 dir = (end - start).normalized;
        Vector2 n = new(-dir.y, dir.x);

        Vector2 b = end - dir * head + n * head * .5f;
        Vector2 c = end - dir * head - n * head * .5f;

        p.BeginPath();
        p.MoveTo(end); p.LineTo(b); p.LineTo(c);
        p.ClosePath(); p.Fill();
    }
}
