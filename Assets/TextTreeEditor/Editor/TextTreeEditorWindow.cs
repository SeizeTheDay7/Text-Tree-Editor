using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using Codice.CM.Common.Merge;

public class TextTreeEditorWindow : EditorWindow
{
    private VisualElement backgroundElement;
    private VisualElement contentLayerElement;
    private VisualElement connectionLayerElement;
    private VisualElement cursorElement;
    private Button addNodeButton;
    private bool isPanning = false;
    private Vector2 panStartMouse;
    private Vector2 panStartContentPos;
    private VisualElement currentSelectNode;
    private EventCallback<MouseMoveEvent> _onMouseMoveCallback;
    private Vector2 currentMousePos;
    private ConnectionElement currentConnection;


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

        _onMouseMoveCallback = UpdateLine;

        // Do not change the order of these
        SetClickBackground();
        SetContentArea();
        SetConnectionArea();
        SetCursor();

        CursorEvent();
        PanningEvent();
        AddNodeEvent();
    }

    #region VisualElement
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

    private void SetConnectionArea()
    {
        connectionLayerElement = new VisualElement
        {
            pickingMode = PickingMode.Ignore, // Ignore mouse event
            style = { position = Position.Absolute, left = 0, top = 0, right = 0, bottom = 0 }
        };
        contentLayerElement.Add(connectionLayerElement);
    }

    #endregion

    #region Event

    /// <summary>
    /// Left click to set cursor
    /// </summary>
    private void CursorEvent()
    {
        backgroundElement.RegisterCallback<MouseDownEvent>(evt =>
        {
            if (evt.button == 0) // left click
            {
                DeselectCurrentNode();
                DeleteTempConnection();

                Vector2 world = backgroundElement.LocalToWorld(evt.localMousePosition);
                Vector2 rootPos = rootVisualElement.WorldToLocal(world);

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
            if (evt.button == 2) // right click
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

    /// <summary>
    /// Add node at the cursor point (or init point if there's no cursor)
    /// </summary>
    private void AddNodeEvent()
    {
        addNodeButton = rootVisualElement.Q<Button>("addNodeButton");
        if (addNodeButton == null) { Debug.LogError("Button 'addNodeButton' not found!"); return; }
        addNodeButton.clicked += () => { AddNode(); };
    }

    private void AddNode()
    {
        var node = new VisualElement();
        node.AddToClassList("Node");

        if (cursorElement.visible)
        {
            Vector3 t = contentLayerElement.resolvedStyle.translate;

            float localX = cursorElement.resolvedStyle.left - t.x;
            float localY = cursorElement.resolvedStyle.top - t.y;

            node.style.left = localX + cursorElement.resolvedStyle.width * 0.5f;
            node.style.top = localY + cursorElement.resolvedStyle.height * 0.5f;
        }
        else
        {
            node.style.left = 200; // x coord
            node.style.top = 100; // y coord
        }

        var label = new Label("test");
        node.Add(label);

        SetNodeEvent(node);
        contentLayerElement.Add(node);
    }

    /// <summary>
    /// 1. Left click to select node
    /// 2. Right click to create new connection
    /// 3. Left click to connect two nodes
    /// </summary>
    private void SetNodeEvent(VisualElement node)
    {
        node.RegisterCallback<MouseDownEvent>(evt =>
        {
            if (evt.button == 0 && currentConnection != null && currentConnection.fromNode != node)
            {
                ConfirmConnection(node);
            }
            if (evt.button == 0)
            {
                DeselectCurrentNode();
                SelectNode(node);
                evt.StopPropagation();
            }
        });

        var manipulator = new ContextualMenuManipulator(evt =>
        {
            evt.menu.AppendAction("Create Connection", a =>
            {
                MakeNewConnection(node);
            });
        });

        node.AddManipulator(manipulator);
    }

    private void SelectNode(VisualElement node)
    {
        currentSelectNode = node;
        currentSelectNode.AddToClassList("highlighted");
    }

    private void DeselectCurrentNode()
    {
        if (currentSelectNode != null) { currentSelectNode.RemoveFromClassList("highlighted"); }
        currentSelectNode = null;
    }

    private void MakeNewConnection(VisualElement fromNode)
    {
        currentConnection = new ConnectionElement(fromNode, contentLayerElement, backgroundElement);
        BeginConnectionMoving();
        connectionLayerElement.Add(currentConnection);
    }

    private void DeleteTempConnection()
    {
        if (currentConnection == null) return;
        StopConnectionMouseMoving();
        currentConnection.RemoveFromHierarchy();
        currentConnection = null;
    }

    private void ConfirmConnection(VisualElement toNode)
    {
        currentConnection.ConfirmConnection(toNode);
        currentConnection = null;
        StopConnectionMouseMoving();
    }

    public void BeginConnectionMoving() => backgroundElement.RegisterCallback<MouseMoveEvent>(UpdateLine);
    public void StopConnectionMouseMoving() => backgroundElement.UnregisterCallback<MouseMoveEvent>(UpdateLine);
    private void UpdateLine(MouseMoveEvent evt)
    {
        if (currentConnection == null) return;
        currentConnection.UpdateLine(evt.localMousePosition);
    }

    #endregion
}
