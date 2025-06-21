using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

public partial class TextTreeEditorWindow : EditorWindow
{
    private VisualElement backgroundElement;
    private VisualElement contentLayerElement;
    private VisualElement nodeLayerElement;
    private VisualElement edgeLayerElement;
    private VisualElement cursorElement;
    private Button addNodeButton;
    private bool isPanning = false;
    private Vector2 panStartMouse;
    private Vector2 panStartContentPos;
    private NodeElement currentSelectNode;
    private EdgeElement currentCreatingEdge;
    private EdgeElement currentSelectEdge;

    public static TextTreeEditorWindow Instance { get; private set; }
    void OnEnable() => Instance = this;
    void OnDisable() => Instance = null;


    [MenuItem("Window/TextTree Editor")]
    public static void ShowWindow()
    {
        var window = GetWindow<TextTreeEditorWindow>();
        window.titleContent = new GUIContent("TextTree Editor");
    }

    public void CreateGUI()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/TextTreeEditor/Editor/TextTreeEditorWindow.uxml");
        visualTree.CloneTree(rootVisualElement);

        // TextTreeEditorWindow_UI.cs
        SetUI();

        // Do not change the order of these
        SetClickBackground();
        SetContentArea();
        SetNodeArea();
        SetEdgeArea();
        SetCursor();

        CursorEvent();
        PanningEvent();
        NodeMovingEvent();
        BackgroundInitEvent();

        // TextTreeEditorWindow_Node.cs
        AddNodeEvent();
    }

    private enum ExceptFor { Nothing, Cursor, Node, Edge }

    private void ResetPick(ExceptFor exceptFor)
    {
        if (exceptFor != ExceptFor.Cursor)
            cursorElement.visible = false;
        if (exceptFor != ExceptFor.Node)
            DeselectCurrentNode();
        if (exceptFor != ExceptFor.Edge)
            DeselectCurrentEdge();
    }

    #region Actor Function

    public List<string> GetTTActorNameList()
    {
        return narrator.actorList.Select(actor => actor?.actorName).ToList();
    }

    public List<string> GetTTEventNameList(string actorName)
    {
        var actor = narrator.actorList.FirstOrDefault(a => a.actorName == actorName);
        if (actor == null) return new List<string>();

        return actor.eventList.Select(e => e.eventName).ToList();
    }

    #endregion

    #region Base Component
    private void SetClickBackground()
    {
        backgroundElement = rootVisualElement.Q<VisualElement>("Background");
    }

    private void SetCursor()
    {
        cursorElement = new VisualElement();
        cursorElement.style.width = 16;
        cursorElement.style.height = 16;
        cursorElement.style.position = Position.Absolute;
        cursorElement.style.backgroundImage = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/TextTreeEditor/Editor/Sprites/cursor.png");
        cursorElement.style.visibility = Visibility.Hidden;
        cursorElement.style.left = 0;
        cursorElement.style.top = 0;

        rootVisualElement.Add(cursorElement);
    }

    private void SetContentArea()
    {
        contentLayerElement = new VisualElement();
        contentLayerElement.AddToClassList("contentLayer");
        backgroundElement.Add(contentLayerElement);
    }

    private void SetNodeArea()
    {
        nodeLayerElement = new VisualElement
        {
            pickingMode = PickingMode.Ignore, // Ignore mouse event
            style = { position = Position.Absolute, left = 0, top = 0, right = 0, bottom = 0 }
        };
        contentLayerElement.Add(nodeLayerElement);
    }

    private void SetEdgeArea()
    {
        edgeLayerElement = new VisualElement
        {
            pickingMode = PickingMode.Ignore, // Ignore mouse event
            style = { position = Position.Absolute, left = 0, top = 0, right = 0, bottom = 0 }
        };
        contentLayerElement.Add(edgeLayerElement);
    }

    #endregion

    #region Background Init Event

    private void BackgroundInitEvent()
    {
        backgroundElement.RegisterCallback<MouseDownEvent>(evt =>
        {
            if (evt.button == 0)
            {
                DeselectCurrentNode();
                DeselectCurrentEdge();
                DeleteTempEdge();
            }
        });
    }

    #endregion

    #region Cursor & Panning Event

    /// <summary>
    /// Left click to set cursor
    /// </summary>
    private void CursorEvent()
    {
        backgroundElement.RegisterCallback<MouseDownEvent>(evt =>
        {
            if (evt.button == 0 && currentCreatingEdge == null) // left click
            {
                ResetPick(ExceptFor.Nothing);

                Vector2 rootPos = backgroundElement.ChangeCoordinatesTo(rootVisualElement, evt.localMousePosition);

                float adjustedX = rootPos.x - (cursorElement.resolvedStyle.width * 0.5f);
                float adjustedY = rootPos.y - (cursorElement.resolvedStyle.height * 0.5f);

                cursorElement.style.left = adjustedX;
                cursorElement.style.top = adjustedY;
                cursorElement.style.visibility = Visibility.Visible;
            }
        });
    }

    /// <summary>
    /// Right click to pan the screen
    /// </summary>
    private void PanningEvent()
    {
        backgroundElement.RegisterCallback<MouseDownEvent>(evt =>
        {
            if (evt.button == 2) // wheel button
            {
                isPanning = true;
                panStartMouse = evt.mousePosition;
                Vector3 startTranslate = contentLayerElement.resolvedStyle.translate;
                panStartContentPos = new Vector2(startTranslate.x, startTranslate.y);
                backgroundElement.CaptureMouse(); // keep focus when got outside of the window
                evt.StopPropagation();
            }
        });

        backgroundElement.RegisterCallback<MouseMoveEvent>(evt =>
        {
            if (!isPanning) return;

            Vector2 delta = evt.mousePosition - panStartMouse;
            Vector2 newOffset = panStartContentPos + delta;

            contentLayerElement.style.translate = new Translate(newOffset.x, newOffset.y);
        });

        backgroundElement.RegisterCallback<MouseUpEvent>(evt =>
        {
            if (evt.button == 2 && isPanning)
            {
                isPanning = false;
                backgroundElement.ReleaseMouse(); // stop event capturing for this window
            }
        });
    }
    #endregion

    #region Node Moving Event
    // ※ It breaks when it becomes a node element event
    private void NodeMovingEvent()
    {
        backgroundElement.RegisterCallback<MouseMoveEvent>(evt =>
        {
            if (currentSelectNode != null && isNodeMoving)
            {
                Vector2 contentLocalPos = backgroundElement.ChangeCoordinatesTo(contentLayerElement, evt.localMousePosition);
                Vector2 halfSize = new Vector2(currentSelectNode.resolvedStyle.width * 0.5f, currentSelectNode.resolvedStyle.height * 0.5f);
                Vector2 newPosition = contentLocalPos - halfSize;

                currentSelectNode.style.left = newPosition.x;
                currentSelectNode.style.top = newPosition.y;

                currentSelectNode.textNodeData.position = newPosition; // Update position in text node data

                foreach (EdgeElement edge in currentSelectNode.inEdge) edge.UpdateLine();
                foreach (EdgeElement edge in currentSelectNode.outEdge) edge.UpdateLine();
            }
        });

        backgroundElement.RegisterCallback<MouseUpEvent>(evt =>
        {
            if (evt.button == 0 && isNodeMoving)
            {
                isNodeMoving = false;
            }
        });
    }

    #endregion
}
