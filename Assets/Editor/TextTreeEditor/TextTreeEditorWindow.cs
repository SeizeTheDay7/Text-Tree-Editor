using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

public class TextTreeEditorWindow : EditorWindow
{
    private VisualElement cursorElement;
    bool showCursor = false;
    private bool isPanning = false;
    private Vector2 panStartPos;

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

        SetCursor();
        SetClickBackground();
        SetAddNodeButton();
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
        var background = new VisualElement();
        background.style.flexGrow = 1;
        rootVisualElement.Add(background);

        BackgroundClickEvent(background);
    }

    /// <summary>
    /// Left click : cursor goes to the position
    /// Right click : pan the screen
    /// </summary>
    private void BackgroundClickEvent(VisualElement background)
    {
        background.RegisterCallback<MouseDownEvent>(evt =>
        {
            if (evt.button == 0) // left click event
            {
                Vector2 world = background.LocalToWorld(evt.localMousePosition);
                Vector2 rootPos = rootVisualElement.WorldToLocal(world);

                float adjustedX = rootPos.x - (cursorElement.resolvedStyle.width * 0.5f);
                float adjustedY = rootPos.y - (cursorElement.resolvedStyle.height * 0.5f);

                cursorElement.style.left = adjustedX;
                cursorElement.style.top = adjustedY;
                cursorElement.style.visibility = Visibility.Visible;
            }
            else if (evt.button == 1) // right click event - start pan
            {
                isPanning = true;
                panStartPos = evt.localMousePosition;
            }
        });

        // background.RegisterCallback<MouseMoveEvent>(evt =>
        // {
        //     if (isPanning)
        //     {
        //         Vector2 delta = evt.localMousePosition - panStartPos;
        //         panStartPos = evt.localMousePosition;

        //         // Apply pan by adjusting scroll position or element translation
        //         // Example: moving the content element
        //         var content = background.Q("Content"); // replace with your actual movable element name
        //         if (content != null)
        //         {
        //             float newLeft = content.style.left.value + delta.x;
        //             float newTop = content.style.top.value + delta.y;

        //             content.style.left = newLeft;
        //             content.style.top = newTop;
        //         }
        //     }
        // });

        // background.RegisterCallback<MouseUpEvent>(evt =>
        // {
        //     if (evt.button == 1 && isPanning)
        //     {
        //         isPanning = false; // stop panning on right mouse release
        //     }
        // });
    }

    private void SetAddNodeButton()
    {
        var addButton = rootVisualElement.Q<Button>("addNodeButton");
        if (addButton == null) { Debug.LogError("Button 'addNodeButton' not found!"); return; }

        AddNodeButtonEvent(addButton);
    }

    /// <summary>
    /// Add node at the cursor point (or init point if there's no cursor)
    /// </summary>
    private void AddNodeButtonEvent(Button addButton)
    {
        addButton.clicked += () =>
        {
            var node = new VisualElement();
            node.style.width = 150;
            node.style.height = 100;
            node.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
            node.style.position = Position.Absolute;

            if (cursorElement.visible)
            {
                rootVisualElement.schedule.Execute(() =>
                {
                    node.style.left = cursorElement.resolvedStyle.left;
                    node.style.top = cursorElement.resolvedStyle.top;
                }).ExecuteLater(0);
            }
            else
            {
                node.style.left = 200; // x coord
                node.style.top = 100; // y coord
            }

            var label = new Label("test");
            node.Add(label);

            rootVisualElement.Add(node);
        };
    }
}
