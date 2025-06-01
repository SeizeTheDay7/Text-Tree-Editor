using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

public class DialogueEditorWindow : EditorWindow
{
    [MenuItem("Window/Dialogue Editor")]
    public static void ShowWindow()
    {
        var window = GetWindow<DialogueEditorWindow>();
        window.titleContent = new GUIContent("Dialogue Editor");
    }

    public void CreateGUI()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/DialogueEditor/DialogueEditorWindow.uxml");
        visualTree.CloneTree(rootVisualElement);

        var addButton = rootVisualElement.Q<Button>("addNodeButton");
        if (addButton == null) { Debug.LogError("Button 'addNodeButton' not found!"); return; }
        addButton.clicked += () =>
        {

        };
    }
}
