using UnityEngine;
using System.Collections.Generic;

public class TextTreeManager : MonoBehaviour, IExposedPropertyTable
{
    [HideInInspector] public Dictionary<PropertyName, Object> sceneTable = new();

    public void SetReferenceValue(PropertyName id, Object value)
    {
        if (sceneTable.ContainsKey(id))
            return;
        sceneTable[id] = value;
    }

    public Object GetReferenceValue(PropertyName id, out bool isValid)
    {
        isValid = sceneTable.TryGetValue(id, out var obj);
        return obj;
    }

    public void ClearReferenceValue(PropertyName id)
    {
        sceneTable.Remove(id);
    }
}