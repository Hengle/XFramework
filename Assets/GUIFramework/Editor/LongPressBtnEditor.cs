using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(LongPressBtn), true)]
[CanEditMultipleObjects]
public class LongPressBtnEditor : ButtonEditor
{
    SerializedProperty m_maxTime;

    protected override void OnEnable()
    {
        base.OnEnable();
        m_maxTime = serializedObject.FindProperty("maxTime");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(m_maxTime);
        serializedObject.ApplyModifiedProperties();
        base.OnInspectorGUI();
    }
}