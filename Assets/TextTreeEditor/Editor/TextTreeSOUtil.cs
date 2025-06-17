using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor.UIElements;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.SceneManagement;


/// <summary>
/// Handling text tree for Editor 
/// </summary>
public static class TextTreeSOUtil
{
    public static void CreateNewTreeFile(ObjectField textTreeField)
    {
        string path = EditorUtility.SaveFilePanelInProject(
            "Create New Text Tree",
            "TextTree_Name",
            "asset",
            "Please enter a file name to save the text tree to"
        );

        if (string.IsNullOrEmpty(path)) return;

        var textTreeSO = ScriptableObject.CreateInstance<TextTreeSO>();
        AssetDatabase.CreateAsset(textTreeSO, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        textTreeField.value = textTreeSO;
    }

    internal static void SaveTreeToSO(Dictionary<string, NodeElement> nodeElementDict, ObjectField textTreeField)
    {
        if (textTreeField.value == null)
        {
            Debug.LogError("No text tree asset selected. Please select an asset to save.");
            return;
        }

        var textTreeSO = (TextTreeSO)textTreeField.value;
        textTreeSO.textNodeList = nodeElementDict.Values.Select(nodeElem => nodeElem.textNodeData).ToList();

        EditorUtility.SetDirty(textTreeSO);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Text tree saved to {AssetDatabase.GetAssetPath(textTreeSO)}");
    }

    // internal static List<UnityEvent> ConvertToUnityEventList(List<TTEvent> ttEventList)
    // {
    //     List<UnityEvent> unityEventList = new List<UnityEvent>();

    //     foreach (var ttEvent in ttEventList)
    //     {
    //         var ttmanager = GameObject.FindFirstObjectByType<TextTreeManager>();
    //         GameObject target = ttEvent.target.Resolve(ttmanager);

    //         UnityEvent unityEvent = new UnityEvent();
    //         MethodInfo methodInfo = target.GetType().GetMethod(ttEvent.methodName);

    //     }

    //     return unityEventList;
    // }


    internal static void SetReferenceValue(SerializedProperty exposedRefProp, Object value)
    {

        TextTreeManager manager = null;
        var activeScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
        foreach (var rootObj in activeScene.GetRootGameObjects())
        {
            manager = rootObj.GetComponentInChildren<TextTreeManager>(true);
            if (manager != null) break;
        }
        if (manager == null) { Debug.LogError("TextTreeManager not found in the active scene."); return; }

        var exposedNameProp = exposedRefProp.FindPropertyRelative("exposedName");
        if (string.IsNullOrEmpty(exposedNameProp.stringValue))
        {
            exposedNameProp.stringValue = System.Guid.NewGuid().ToString();
        }

        var propertyName = new PropertyName(exposedNameProp.stringValue);
        manager.SetReferenceValue(propertyName, value); // Set propertyName into manager in current scene

        exposedRefProp.serializedObject.ApplyModifiedProperties();
        if (manager.gameObject.scene.isLoaded)
        {
            EditorSceneManager.MarkSceneDirty(manager.gameObject.scene);
        }
    }

    internal static GameObject GetReferenceValue()
    {
        return null;
    }

    internal static void ClearReferenceValue(PropertyName id)
    {

    }
}