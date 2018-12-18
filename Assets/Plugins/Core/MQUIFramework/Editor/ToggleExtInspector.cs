// ==========================================
// 描述： 
// 作者： HAk
// 时间： 2018-11-22 13:28:27
// 版本： V 1.0
// ==========================================
using UnityEditor;
using UnityEditor.UI;

/// <summary>
/// Toggle扩展类的编辑框
/// </summary>
[CustomEditor(typeof(ToggleExt), true)]
[CanEditMultipleObjects]
public class ToggleExtInspector : ToggleEditor {

    SerializedProperty m_controledPanel;

    protected override void OnEnable()
    {
        base.OnEnable();

        m_controledPanel = serializedObject.FindProperty("controledPanel");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(m_controledPanel);
        serializedObject.ApplyModifiedProperties();
    }
}
