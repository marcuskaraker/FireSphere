using UnityEditor;
using UnityEngine;
using MK;

[CustomPropertyDrawer(typeof(MinMax))]
public class MinMaxDrawerUIE : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        property.serializedObject.ApplyModifiedProperties();
        property.serializedObject.Update();

        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Get properties
        SerializedProperty minProperty = property.FindPropertyRelative("min");
        SerializedProperty maxProperty = property.FindPropertyRelative("max");

        float[] propertyValues = new float[] { minProperty.floatValue, maxProperty.floatValue };

        // Display the floats on the same line
        EditorGUI.MultiFloatField(
            position,
            new GUIContent[] { new GUIContent("Min"), new GUIContent("Max") },
            propertyValues
        );

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        minProperty.floatValue = propertyValues[0];
        maxProperty.floatValue = propertyValues[1];

        EditorGUI.EndProperty();

        property.serializedObject.ApplyModifiedProperties();
    }
}
