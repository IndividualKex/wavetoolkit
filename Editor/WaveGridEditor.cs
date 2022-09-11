using UnityEditor;
using WaveTools;

namespace WaveToolsEditor {
    public class WaveGridEditor {
        public WaveGrid target { get; private set; }

        public WaveGridEditor(WaveGrid target) {
            this.target = target;
        }

        public void OnInspectorGUI(SerializedProperty property) {
            // Name
            target.name = EditorGUILayout.TextField("Name", target.name);

            // Dimensions
            EditorGUILayout.PropertyField(property.FindPropertyRelative("width"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("length"));
            EditorGUILayout.PropertyField(property.FindPropertyRelative("height"));

            // Module size
            target.moduleSize = EditorGUILayout.FloatField("Module Size", target.moduleSize);

            // Offset
            EditorGUILayout.PropertyField(property.FindPropertyRelative("offset"));

            // Modules
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(property.FindPropertyRelative("modules"));
        }
    }
}