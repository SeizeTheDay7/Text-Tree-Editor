using UnityEngine;

public class ExampleGameLogic : MonoBehaviour
{
    [SerializeField] ExampleGameState exampleGameState;

    public bool EvaluateCoditions(Condition[] condArr)
    {
        bool isValidNode = true;
        foreach (Condition cond in condArr)
        {
            // get a field value from the game state
            var fieldInfo = typeof(ExampleGameState).GetField(cond.field);
            float fieldValue = (float)fieldInfo.GetValue(exampleGameState);

            isValidNode = CompareConditionValue(cond, fieldValue);
            if (isValidNode == false) break;
        }
        return isValidNode;
    }

    private bool CompareConditionValue(Condition cond, float fieldValue)
    {
        bool isValidNode;

        switch (cond.compFunc)
        {
            case CompFunc.Same:
                isValidNode = (fieldValue == cond.value);
                break;
            case CompFunc.Diff:
                isValidNode = (fieldValue != cond.value);
                break;
            case CompFunc.GreaterThan:
                isValidNode = (fieldValue > cond.value);
                break;
            case CompFunc.LessThan:
                isValidNode = (fieldValue < cond.value);
                break;
            case CompFunc.GreaterThanOrEqual:
                isValidNode = (fieldValue >= cond.value);
                break;
            case CompFunc.LessThanOrEqual:
                isValidNode = (fieldValue <= cond.value);
                break;
            default:
                Debug.LogError($"Unknown comparison function: {cond.compFunc}");
                isValidNode = false;
                break;
        }

        return isValidNode;
    }
}