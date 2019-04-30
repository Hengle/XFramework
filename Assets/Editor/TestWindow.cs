using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TestWindow : EditorWindow
{
    [MenuItem("XDEDZL/TestWindow")]
    private static void Open()
    {
        GetWindow(typeof(TestWindow));
    }

    int grid;
    Rect rect = new Rect(20, 20, 120, 50);

    private void OnEnable()
    {
        rect = new Rect(this.position.xMin, this.position.yMin, 500, 500);
        Debug.Log(rect);  
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();
        if (GUILayout.Button("Hello World"))
            Debug.Log("aaa");
        rect = GUILayout.Window(grid, rect, ChildWindow, "MyWindow");

        GUILayout.EndVertical();
    }

    private void ChildWindow(int id)
    {
        if (GUILayout.Button("Hello World"))
            Debug.Log("aaa");
    }
}