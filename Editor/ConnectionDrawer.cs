using UnityEditor;
using UnityEngine;
using WaveTools;

namespace WaveToolsEditor {
    [CustomPropertyDrawer(typeof(Connection))]
    public class ConnectionDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);

            EditorGUI.BeginChangeCheck();

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            SerializedProperty nameValue = property.FindPropertyRelative("name");
            EditorGUI.PropertyField(position, nameValue, GUIContent.none);

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
}