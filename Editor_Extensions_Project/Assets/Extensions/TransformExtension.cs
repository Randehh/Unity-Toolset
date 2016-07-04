using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects, CustomEditor(typeof(Transform))]
public class TransformExtension : Editor {

    private const float POSITION_MAX = 100000.0f;

    private static GUIContent positionGUIContent = new GUIContent(LocalString("Position")
                                                                 , LocalString("The local position of this Game Object relative to the parent."));
    private static GUIContent rotationGUIContent = new GUIContent(LocalString("Rotation")
                                                                 , LocalString("The local rotation of this Game Object relative to the parent."));
    private static GUIContent scaleGUIContent = new GUIContent(LocalString("Scale")
                                                                 , LocalString("The local scaling of this Game Object relative to the parent."));
    private static GUIContent resetGUIContent = new GUIContent(LocalString("Reset")
                                                                 , LocalString("Reset the parameters of this type to 0."));

    private static string positionWarningText = LocalString("Due to floating-point precision limitations, it is recommended to bring the world coordinates of the GameObject within a smaller range.");

    private SerializedProperty positionProperty;
    private SerializedProperty rotationProperty;
    private SerializedProperty scaleProperty;

    private static string LocalString(string text) {
        return LocalizationDatabase.GetLocalizedString(text);
    }

    public void OnEnable() {
        this.positionProperty = this.serializedObject.FindProperty("m_LocalPosition");
        this.rotationProperty = this.serializedObject.FindProperty("m_LocalRotation");
        this.scaleProperty = this.serializedObject.FindProperty("m_LocalScale");
    }

    public override void OnInspectorGUI() {
        this.serializedObject.Update();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(this.positionProperty, positionGUIContent);
        if (GUILayout.Button(resetGUIContent, GUILayout.Width(50))) {
            this.positionProperty.vector3Value = Vector3.zero;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        this.RotationPropertyField(this.rotationProperty, rotationGUIContent);
        if (GUILayout.Button(resetGUIContent, GUILayout.Width(50))) {
            this.rotationProperty.quaternionValue = Quaternion.identity;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(this.scaleProperty, scaleGUIContent);
        if (GUILayout.Button(resetGUIContent, GUILayout.Width(50))) {
            this.scaleProperty.vector3Value = new Vector3(1, 1, 1);
        }
        EditorGUILayout.EndHorizontal();

        if (!ValidatePosition(((Transform)this.target).position)) {
            EditorGUILayout.HelpBox(positionWarningText, MessageType.Warning);
        }

        this.serializedObject.ApplyModifiedProperties();
    }

    private bool ValidatePosition(Vector3 position) {
        if (Mathf.Abs(position.x) > TransformExtension.POSITION_MAX) return false;
        if (Mathf.Abs(position.y) > TransformExtension.POSITION_MAX) return false;
        if (Mathf.Abs(position.z) > TransformExtension.POSITION_MAX) return false;
        return true;
    }

    private void RotationPropertyField(SerializedProperty rotationProperty, GUIContent content) {
        Transform transform = (Transform)this.targets[0];
        Quaternion localRotation = transform.localRotation;
        foreach (UnityEngine.Object t in (UnityEngine.Object[])this.targets) {
            if (!SameRotation(localRotation, ((Transform)t).localRotation)) {
                EditorGUI.showMixedValue = true;
                break;
            }
        }

        EditorGUI.BeginChangeCheck();

        Vector3 eulerAngles = EditorGUILayout.Vector3Field(content, localRotation.eulerAngles);

        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObjects(this.targets, "Rotation Changed");
            foreach (UnityEngine.Object obj in this.targets) {
                Transform t = (Transform)obj;
                t.localEulerAngles = eulerAngles;
            }
            rotationProperty.serializedObject.SetIsDifferentCacheDirty();
        }

        EditorGUI.showMixedValue = false;
    }

    private bool SameRotation(Quaternion rot1, Quaternion rot2) {
        if (rot1.x != rot2.x) return false;
        if (rot1.y != rot2.y) return false;
        if (rot1.z != rot2.z) return false;
        if (rot1.w != rot2.w) return false;
        return true;
    }
}