using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

// This drawer handles properties of type 'Condition'
[CustomPropertyDrawer(typeof(Condition))]
public class ConditionDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Make a dropdown for field property
        SerializedProperty fieldProperty = property.FindPropertyRelative("field");
        var dropdown = new DropdownField("Field");
        dropdown.choices = GameContextUtil.GetGameContextFieldNameList();
        dropdown.BindProperty(fieldProperty);

        // Make a box showing field type
        var fieldTypeField = new TextField("FieldType");
        fieldTypeField.multiline = false;
        fieldTypeField.isReadOnly = true;
        dropdown.RegisterValueChangedCallback(evt =>
        {
            string fieldName = evt.newValue;
            if (string.IsNullOrEmpty(fieldName)) { fieldTypeField.value = ""; }
            fieldTypeField.value = GameContextUtil.GetGameContextFieldTypeAlias(fieldName);
        });

        // Make a field for compFunc and value
        SerializedProperty compFuncProperty = property.FindPropertyRelative("compFunc");
        SerializedProperty valueProperty = property.FindPropertyRelative("value");
        var compFuncField = new PropertyField(compFuncProperty, "Comp Func");
        var valueField = new PropertyField(valueProperty, "Value");

        // Checks type casting for value
        var warningSign = new Label("Value does not match field type!") { name = "WarningSign" };
        warningSign.style.display = DisplayStyle.None;
        valueField.RegisterCallback<SerializedPropertyChangeEvent>(evt =>
        {
            string newValue = valueProperty.boxedValue.ToString();
            if (GameContextUtil.CanCastValueToFieldType(fieldProperty.stringValue, newValue))
            {
                warningSign.style.display = DisplayStyle.None;
            }
            else
            {
                warningSign.style.display = DisplayStyle.Flex;
            }
        });

        var container = new VisualElement();
        container.Add(fieldTypeField);
        container.Add(dropdown);
        container.Add(compFuncField);
        container.Add(valueField);
        container.Add(warningSign);

        // if anything changed in container, save it

        return container;
    }
}