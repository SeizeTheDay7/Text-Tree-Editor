using System.Collections.Generic;
using UnityEngine;

public class ExampleGameLogic : MonoBehaviour
{
    [SerializeField] TextAsset textTree;
    private Dictionary<string, TextNodeData> textNodeDict;
    private TextNodeData currentTextNode;

    public void InitTextTree()
    {
        textNodeDict = RuntimeJSONManager.LoadJsonToDict(textTree);
    }

    public void ProceedToNextNode()
    {
        currentTextNode = textNodeDict[TextTreeUtil.GetNextNodeKey(currentTextNode)];
    }

}
