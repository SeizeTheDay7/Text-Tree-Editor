using UnityEngine;
using EditorAttributes;

public class CharacterManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Character[] characterArray;

    [Header("Parameters")]
    [SerializeField] Color loseFocusColor;
    [SerializeField] float initX = -50;

    [Button]
    public void LoseAllFocus()
    {
        foreach (Character character in characterArray)
        {
            character.LoseFocus(loseFocusColor);
        }
    }

    public void MoveAwayAll()
    {
        foreach (Character character in characterArray)
        {
            character.SetPosX(initX);
        }
    }
}