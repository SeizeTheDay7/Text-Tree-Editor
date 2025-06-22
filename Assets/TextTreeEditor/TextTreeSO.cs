using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TextTree", menuName = "TextTree/TextTreeSO")]
public class TextTreeSO : ScriptableObject
{
    public string initNodeKey;
    public List<TextNodeData> textNodeList;
}