using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class TempCondSO : ScriptableObject
{
    public List<Condition> conditionList; // Condition reference in edge data
    public List<string> fieldNameList; // Field name list for dropdown choices

    public TempCondSO(List<Condition> conditionList)
    {
        this.conditionList = conditionList;
        fieldNameList = GameContextUtil.GetGameContextFieldNameList();
    }
}