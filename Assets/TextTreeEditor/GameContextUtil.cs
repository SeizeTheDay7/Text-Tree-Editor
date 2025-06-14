using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;

/// <summary>
/// Stores information and helper function about fields in GameContext classes
/// </summary>
public static class GameContextUtil
{
    static readonly Dictionary<Type, string> TypeAliases = new()
    {
        { typeof(bool), "bool" },
        { typeof(byte), "byte" },
        { typeof(sbyte), "sbyte" },
        { typeof(char), "char" },
        { typeof(decimal), "decimal" },
        { typeof(double), "double" },
        { typeof(float), "float" },
        { typeof(int), "int" },
        { typeof(uint), "uint" },
        { typeof(long), "long" },
        { typeof(ulong), "ulong" },
        { typeof(short), "short" },
        { typeof(ushort), "ushort" },
        { typeof(object), "object" },
        { typeof(string), "string" },
    };

    private static List<GameContext> gameContextList = new List<GameContext>();
    private static Dictionary<string, FieldInfo> fieldInfoDict; // To get a fieldinfo by field name 

    #region Init
    static GameContextUtil()
    {
        SetGameContextFieldInfoDict();
    }

    private static void SetGameContextFieldInfoDict()
    {
        fieldInfoDict = new Dictionary<string, FieldInfo>();
        List<FieldInfo> fieldInfos = GetGameContextFieldInfoList();

        foreach (var fieldInfo in fieldInfos)
        {
            if (!fieldInfoDict.ContainsKey(fieldInfo.Name)) { fieldInfoDict.Add(fieldInfo.Name, fieldInfo); }
            else { Debug.LogError($"Duplicate field name detected: {fieldInfo.Name}"); }
        }
    }
    #endregion

    #region GameContext Instance

    /// <summary>
    /// Every 'GameContext' instances are registered on `OnEnable()`
    /// </summary>
    public static void RegisterGameContext(GameContext gameContextInstance)
    {
        gameContextList.Add(gameContextInstance);
    }

    public static void UnregisterGameContext(GameContext gameContextInstance)
    {
        gameContextList.Remove(gameContextInstance);
    }

    public static GameContext FindGameContextIncluding(FieldInfo fieldInfo)
    {
        foreach (var gameContext in gameContextList)
        {
            if (gameContext == null) continue; // Skip if the instance is null

            var type = gameContext.GetType();
            var field = type.GetField(fieldInfo.Name, BindingFlags.Public);
            if (field != null && field.FieldType == fieldInfo.FieldType)
            {
                return gameContext;
            }
        }

        Debug.LogError($"No GameContext found for field: {fieldInfo.Name}");
        return null;
    }

    #endregion

    #region field of GameContext

    public static FieldInfo GetGameContextFieldInfo(string fieldName)
    {
        if (!fieldInfoDict.ContainsKey(fieldName))
        {
            Debug.LogError($"Field name not found: {fieldName}");
            return null;
        }
        return fieldInfoDict[fieldName];
    }

    /// <summary>
    /// Used for field type name box
    /// </summary>
    public static string GetGameContextFieldTypeAlias(string fieldName)
    {
        if (string.IsNullOrEmpty(fieldName)) return null;
        if (!fieldInfoDict.ContainsKey(fieldName))
        {
            Debug.LogError($"Field name not found: {fieldName}");
            return null;
        }
        FieldInfo fieldInfo = fieldInfoDict[fieldName];
        return TypeAliases.ContainsKey(fieldInfo.FieldType) ? TypeAliases[fieldInfo.FieldType] : fieldInfo.FieldType.Name;
    }

    /// <summary>
    /// Used for warning sign
    /// </summary>
    public static bool CanCastValueToFieldType(string fieldName, string value)
    {
        try
        {
            if (value == "") return true;

            FieldInfo fieldInfo = GetGameContextFieldInfo(fieldName);
            Type targetType = fieldInfo.FieldType;
            object parsedValue = Convert.ChangeType(value, targetType);
            return true;
        }
        catch { return false; } // ChangType() throws exception when fails conversion
    }

    /// <summary>
    /// Returns list of field names in GameState classes
    /// Used for dropdown choices
    /// </summary>
    public static List<string> GetGameContextFieldNameList()
    {
        List<string> fieldNames = new List<string>();
        List<FieldInfo> fieldInfos = GetGameContextFieldInfoList();

        foreach (var fieldInfo in fieldInfos)
        {
            if (!fieldNames.Contains(fieldInfo.Name)) { fieldNames.Add(fieldInfo.Name); }
            else { Debug.LogError($"Duplicate field name detected: {fieldInfo.Name}"); }
        }

        return fieldNames;
    }

    /// <summary>
    /// Returns list of fieldinfo in GameState classes
    /// </summary>
    private static List<FieldInfo> GetGameContextFieldInfoList()
    {
        List<FieldInfo> gameContextFields = new List<FieldInfo>();
        var gameContextType = typeof(GameContext);
        var types = Assembly.Load("Assembly-CSharp").GetTypes();

        foreach (var type in types)
        {
            if (!type.IsClass || type.IsAbstract) continue; // Find class, but not abstract class
            if (!gameContextType.IsAssignableFrom(type)) continue; // Did the class inherited 'GameContext'
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);
            gameContextFields.AddRange(fields);
        }

        return gameContextFields;
    }
    #endregion
}