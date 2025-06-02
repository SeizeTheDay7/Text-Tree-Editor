using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

public class TextTreeEditorWindow : EditorWindow
{
    private VisualElement cursorElement;
    private VisualElement backgroundElement;
    private VisualElement contentAreaElement;
    private Button addNodeButton;
    bool showCursor = false;
    private bool isPanning = false;
    private Vector2 panStartMouse;
    private Vector2 panStartContentPos;

    [MenuItem("Window/TextTree Editor")]
    public static void ShowWindow()
    {
        var window = GetWindow<TextTreeEditorWindow>();
        window.titleContent = new GUIContent("TextTree Editor");
    }

    public void CreateGUI()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/TextTreeEditor/TextTreeEditorWindow.uxml");
        visualTree.CloneTree(rootVisualElement);

        SetClickBackground();
        SetContentArea();
        SetCursor();

        BackgroundEvent();
        ContentAreaEvent();
        AddNodeButtonEvent();
    }

    private void SetCursor()
    {
        cursorElement = new VisualElement();
        cursorElement.style.width = 16;
        cursorElement.style.height = 16;
        cursorElement.style.position = Position.Absolute;
        cursorElement.style.backgroundImage = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/TextTreeEditor/Sprites/cursor.png");
        cursorElement.style.visibility = Visibility.Hidden;
        cursorElement.style.left = 0;
        cursorElement.style.top = 0;

        rootVisualElement.Add(cursorElement);
    }

    private void SetClickBackground()
    {
        backgroundElement = new VisualElement();
        backgroundElement.AddToClassList("background");
        backgroundElement.style.flexGrow = 1;
        rootVisualElement.Add(backgroundElement);
    }

    private void SetContentArea()
    {
        contentAreaElement = new VisualElement();
        contentAreaElement.AddToClassList("contentArea");
        backgroundElement.Add(contentAreaElement);
    }

    /// <summary>
    /// Left click to set cursor
    /// </summary>
    private void BackgroundEvent()
    {
        backgroundElement.RegisterCallback<MouseDownEvent>(evt =>
        {
            if (evt.button == 0) // left click
            {
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
    private void ContentAreaEvent()
    {
        backgroundElement.RegisterCallback<MouseDownEvent>(evt =>
        {
            if (evt.button == 1) // right click
            {
                isPanning = true;
                panStartMouse = evt.mousePosition;
                Vector3 startTranslate = contentAreaElement.resolvedStyle.translate;
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

            contentAreaElement.style.translate = new Translate(newOffset.x, newOffset.y);
        });

        backgroundElement.RegisterCallback<MouseUpEvent>(evt =>
        {
            if (evt.button == 1 && isPanning)
            {
                isPanning = false;
                backgroundElement.ReleaseMouse(); // stop event capturing for this window
            }
        });
    }

    /// <summary>
    /// Add node at the cursor point (or init point if there's no cursor)
    /// </summary>
    private void AddNodeButtonEvent()
    {
        addNodeButton = rootVisualElement.Q<Button>("addNodeButton");
        if (addNodeButton == null) { Debug.LogError("Button 'addNodeButton' not found!"); return; }

        addNodeButton.clicked += () =>
        {
            var node = new VisualElement();
            node.style.width = 150;
            node.style.height = 100;
            node.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
            node.style.position = Position.Absolute;

            if (cursorElement.visible)
            {
                node.style.left = cursorElement.resolvedStyle.left + cursorElement.resolvedStyle.width * 0.5f;
                node.style.top = cursorElement.resolvedStyle.top - cursorElement.resolvedStyle.height;
            }
            else
            {
                node.style.left = 200; // x coord
                node.style.top = 100; // y coord
            }

            var label = new Label("test");
            node.Add(label);

            contentAreaElement.Add(node);
        };
    }
}
