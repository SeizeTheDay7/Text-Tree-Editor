using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NarrationSO", menuName = "ScriptableObjects/Narration", order = 2)]
public class NarrationSO : ScriptableObject
{
    [TextArea(3, 5)]
    public string[] narrationText;
    public NextNarration[] nextScript;
}