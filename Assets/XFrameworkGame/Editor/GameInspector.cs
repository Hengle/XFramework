using UnityEditor;
using UnityEngine;
using XFramework;

[CustomEditor(typeof(Game))]
public class GameInspector : Editor
{
    private string[] typeNames = null;
    private int entranceProcedureIndex = 0;

    private void OnEnable()
    {
        entranceProcedureIndex = EditorPrefs.GetInt("index");
    }

    public override void OnInspectorGUI()
    {
        typeNames = typeof(ProcedureBase).GetSonNames();
        if (typeNames.Length == 0)
            return;

        GUILayout.BeginVertical("Box");

        entranceProcedureIndex = EditorGUILayout.Popup("Entrance Procedure", entranceProcedureIndex, typeNames);

        (target as Game).TypeName = typeNames[entranceProcedureIndex];
        

        GUILayout.EndVertical();
    }

    private void OnDestroy()
    {
        EditorPrefs.SetInt("index", entranceProcedureIndex);
    } 
}