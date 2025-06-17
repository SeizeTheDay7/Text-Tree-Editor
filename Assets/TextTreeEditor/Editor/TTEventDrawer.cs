using UnityEditor;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(TTEvent))]
public class TTEventDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // property means TTEvent element. not list.
        return new TTEventField(property);
    }
}