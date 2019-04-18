using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(TargetCamera.CameraAttachTo))]
public class TargetCameraModeInspector : Editor
{
    SerializedProperty attachTarget;

    public override void OnInspectorGUI()
    {
        attachTarget = serializedObject.FindProperty("attachTarget");

        EditorGUILayout.PropertyField(attachTarget);
        serializedObject.ApplyModifiedProperties();
    }
}
