﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using XDEDZL.UI;

[CustomEditor(typeof(GameEntry))]
public class GameEntryInspector : Editor
{
    private string[] typeNames = null;
    private bool[] isActives = null;
    private int entranceProcedureIndex = 0;

    bool test;
    public override void OnInspectorGUI()
    {
        typeNames = typeof(BasePanel).GetSonNames();
        if (isActives == null)
            isActives = new bool[typeNames.Length];
        else if(isActives.Length != typeNames.Length)
            isActives = new bool[typeNames.Length];

        GUILayout.BeginVertical("Box");

        for (int i = 0; i < typeNames.Length; i++)
        {
            isActives[i] = EditorGUILayout.Toggle(typeNames[i], isActives[i]);
        }
        GUILayout.EndVertical();
        entranceProcedureIndex = EditorGUILayout.Popup("Entrance Procedure", entranceProcedureIndex, typeNames);
    }
}