using System.Collections.Generic;
using UnityEngine;

public class TempCondSO : ScriptableObject
{
    public List<Condition> conditionList;

    public TempCondSO(List<Condition> conditionList)
    {
        this.conditionList = conditionList;
    }
}