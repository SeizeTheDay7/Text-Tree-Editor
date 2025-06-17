using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;

public class TTEventField : VisualElement
{
    // Cache
    public UnityEngine.Object target;
    public List<string> methodList;
    public Dictionary<string, MethodInfo> methodMap;

    // SP
    public SerializedProperty targetProp;
    public SerializedProperty methodNameProp;
    public SerializedProperty argsProp;

    // UI
    public ObjectField targetField;
    public PropertyField argsField;
    public PopupField<string> methodDropdown;
    private VisualElement argsContainer;

    public TTEventField(SerializedProperty property)
    {
        targetProp = property.FindPropertyRelative("target");
        methodNameProp = property.FindPropertyRelative("methodName");
        argsProp = property.FindPropertyRelative("args");

        targetField = new ObjectField("Target") { objectType = typeof(UnityEngine.Object), allowSceneObjects = true };
        methodDropdown = new PopupField<string>(new List<string> { "(Select target first)" }, 0);
        methodDropdown.label = "Method";

        argsContainer = new VisualElement();
        argsContainer.style.flexDirection = FlexDirection.Column;

        Add(targetField);
        Add(methodDropdown);
        Add(argsContainer);

        MethodDropdownEvent();
        TargetFieldEvent();
    }

    #region Target field
    private void TargetFieldEvent()
    {
        targetField.RegisterValueChangedCallback(evt =>
        {
            target = evt.newValue as UnityEngine.Object;
            if (target == null) return;
            TextTreeSOUtil.SetReferenceValue(targetProp, evt.newValue);

            // Setup method list
            methodList = new List<string>();
            methodMap = new Dictionary<string, MethodInfo>();

            if (target is GameObject targetGo)
            {
                foreach (var comp in targetGo.GetComponents<Component>())
                {
                    if (comp == null) continue;
                    CollectMethodsFromObject(comp, methodList, methodMap);
                }
            }
            else if (target is Component targetComp)
            {
                CollectMethodsFromObject(targetComp, methodList, methodMap);
            }
            else // ScriptableObject, Material...
            {
                CollectMethodsFromObject(target, methodList, methodMap);
            }

            methodDropdown.choices = methodList;
            methodDropdown.index = 0;
            methodNameProp.stringValue = "";
            methodNameProp.serializedObject.ApplyModifiedProperties();
        });
    }

    void CollectMethodsFromObject(UnityEngine.Object obj, List<string> list, Dictionary<string, MethodInfo> map)
    {
        if (obj == null) return;

        var type = obj.GetType();
        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!method.IsSpecialName)
            {
                string displayName = $"{type.Name}/{method.Name}";

                if (!map.ContainsKey(displayName))
                {
                    list.Add(displayName);
                    map[displayName] = method;
                }
            }
        }
    }

    #endregion

    #region Method dropdown
    private void MethodDropdownEvent()
    {
        methodDropdown.RegisterValueChangedCallback(evt =>
        {
            if (target == null) return;
            if (string.IsNullOrEmpty(evt.newValue)) return;

            methodNameProp.stringValue = evt.newValue.Split('/')[1];
            methodNameProp.serializedObject.ApplyModifiedProperties();

            var selectedMethodInfo = methodMap[evt.newValue];
            var parameters = selectedMethodInfo.GetParameters();

            // Setup parameter fields

            argsContainer.Clear();

            foreach (var parameter in parameters)
            {
                VisualElement argField = CreateArgField(parameter);
                argsContainer.Add(argField);
            }
        });
    }

    #endregion

    #region Argument field

    private VisualElement CreateArgField(ParameterInfo parameter)
    {
        Type type = parameter.ParameterType;
        string name = parameter.Name ?? "Arg";

        // Create arg field based on parameter type
        VisualElement field = null;
        if (type == typeof(int)) field = new IntegerField(name);
        else if (type == typeof(float)) field = new FloatField(name);
        else if (type == typeof(string)) field = new TextField(name);
        else if (type == typeof(bool)) field = new Toggle(name);
        else if (type == typeof(double)) field = new DoubleField(name);
        else if (type == typeof(long)) field = new LongField(name);
        else if (type == typeof(Vector2)) field = new Vector2Field(name);
        else if (type == typeof(Vector3)) field = new Vector3Field(name);
        else if (type == typeof(Vector4)) field = new Vector4Field(name);
        else if (type == typeof(Color)) field = new ColorField(name);
        else if (type == typeof(Rect)) field = new RectField(name);
        else if (type == typeof(Bounds)) field = new BoundsField(name);
        else if (type == typeof(AnimationCurve)) field = new CurveField(name);
        else if (type == typeof(Gradient)) field = new GradientField(name);
        else if (type.IsEnum) field = new EnumField(name) { value = (Enum)Enum.GetValues(type).GetValue(0) };
        else if (typeof(UnityEngine.Object).IsAssignableFrom(type)) field = new ObjectField(name) { objectType = type, allowSceneObjects = true };
        else return new Label($"Unsupported parameter type: {type.Name}");

        // Register value changed callback to update args
        if (field is BaseField<int> intField)
            intField.RegisterValueChangedCallback(_ => UpdateArgValue());
        else if (field is BaseField<float> floatField)
            floatField.RegisterValueChangedCallback(_ => UpdateArgValue());
        else if (field is BaseField<string> textField)
            textField.RegisterValueChangedCallback(_ => UpdateArgValue());
        else if (field is Toggle toggle)
            toggle.RegisterValueChangedCallback(_ => UpdateArgValue());
        else if (field is BaseField<double> doubleField)
            doubleField.RegisterValueChangedCallback(_ => UpdateArgValue());
        else if (field is BaseField<long> longField)
            longField.RegisterValueChangedCallback(_ => UpdateArgValue());
        else if (field is BaseField<Vector2> vector2Field)
            vector2Field.RegisterValueChangedCallback(_ => UpdateArgValue());
        else if (field is BaseField<Vector3> vector3Field)
            vector3Field.RegisterValueChangedCallback(_ => UpdateArgValue());
        else if (field is BaseField<Vector4> vector4Field)
            vector4Field.RegisterValueChangedCallback(_ => UpdateArgValue());
        else if (field is BaseField<Color> colorField)
            colorField.RegisterValueChangedCallback(_ => UpdateArgValue());
        else if (field is BaseField<Rect> rectField)
            rectField.RegisterValueChangedCallback(_ => UpdateArgValue());
        else if (field is BaseField<Bounds> boundsField)
            boundsField.RegisterValueChangedCallback(_ => UpdateArgValue());
        else if (field is BaseField<AnimationCurve> curveField)
            curveField.RegisterValueChangedCallback(_ => UpdateArgValue());
        else if (field is BaseField<Gradient> gradientField)
            gradientField.RegisterValueChangedCallback(_ => UpdateArgValue());
        else if (field is EnumField enumField)
            enumField.RegisterValueChangedCallback(_ => UpdateArgValue());
        else if (field is ObjectField objectField)
            objectField.RegisterValueChangedCallback(_ => UpdateArgValue());

        return field;
    }

    private void UpdateArgValue()
    {
        List<string> argsList = new List<string>();
        foreach (var field in argsContainer.Children())
        {
            string fieldValue = GetFieldString(field);
            if (!string.IsNullOrEmpty(fieldValue))
                argsList.Add(fieldValue);
        }
        argsProp.ClearArray();
        argsProp.arraySize = argsList.Count;
        for (int i = 0; i < argsList.Count; i++)
        {
            argsProp.GetArrayElementAtIndex(i).stringValue = argsList[i];
        }
        argsProp.serializedObject.ApplyModifiedProperties();
    }

    private static string GetFieldString(VisualElement field)
    {
        if (field is IntegerField intField) return intField.value.ToString();
        if (field is FloatField floatField) return floatField.value.ToString();
        if (field is TextField textField) return textField.value;
        if (field is Toggle toggle) return toggle.value.ToString();
        if (field is DoubleField doubleField) return doubleField.value.ToString();
        if (field is LongField longField) return longField.value.ToString();
        if (field is Vector2Field vector2Field) return vector2Field.value.ToString();
        if (field is Vector3Field vector3Field) return vector3Field.value.ToString();
        if (field is Vector4Field vector4Field) return vector4Field.value.ToString();
        if (field is ColorField colorField) return colorField.value.ToString();
        if (field is RectField rectField) return rectField.value.ToString();
        if (field is BoundsField boundsField) return boundsField.value.ToString();
        if (field is CurveField curveField) return curveField.value.ToString();
        if (field is GradientField gradientField) return gradientField.value.ToString();
        if (field is EnumField enumField) return enumField.value.ToString();
        if (field is ObjectField objectField)
        {
            if (objectField.value is GameObject go)
            {
                if (go.scene.IsValid())
                    return $"[Scene] {go.name}";
                else
                    return $"[Asset] {go.name}";
            }
            return objectField.value != null ? objectField.value.name : "null";
        }

        return "";
    }

    #endregion
}
