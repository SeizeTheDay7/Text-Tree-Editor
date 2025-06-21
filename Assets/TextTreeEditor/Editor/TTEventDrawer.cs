using UnityEditor;
using UnityEngine.UIElements;

// This drawer handles properties of type 'TTEvent'
[CustomPropertyDrawer(typeof(TTDEvent))]
public class TTDEventDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        TextTreeEditorWindow TTEditor = TextTreeEditorWindow.Instance;

        var actorDropdown = new DropdownField();
        actorDropdown.style.flexGrow = 1;
        actorDropdown.style.marginRight = 4;
        actorDropdown.choices = TTEditor.GetTTActorNameList();

        var eventDropdown = new DropdownField();
        eventDropdown.style.flexGrow = 1;

        var actorNameProp = property.FindPropertyRelative("actorName");
        var eventNameProp = property.FindPropertyRelative("eventName");

        // Initialize dropdowns with current values 
        if (!string.IsNullOrEmpty(actorNameProp.stringValue))
        {
            actorDropdown.value = actorNameProp.stringValue;
            eventDropdown.choices = TTEditor.GetTTEventNameList(actorNameProp.stringValue);
        }
        if (!string.IsNullOrEmpty(eventNameProp.stringValue))
        {
            eventDropdown.value = eventNameProp.stringValue;
        }

        actorDropdown.RegisterValueChangedCallback(evt =>
        {
            actorNameProp.stringValue = evt.newValue;
            eventDropdown.choices = TTEditor.GetTTEventNameList(evt.newValue);
            property.serializedObject.ApplyModifiedProperties();
        });

        eventDropdown.RegisterValueChangedCallback(evt =>
        {
            eventNameProp.stringValue = evt.newValue;
            property.serializedObject.ApplyModifiedProperties();
        });

        var container = new VisualElement();
        container.style.flexDirection = FlexDirection.Row;
        container.style.justifyContent = Justify.SpaceBetween;
        container.Add(actorDropdown);
        container.Add(eventDropdown);

        return container;
    }
}