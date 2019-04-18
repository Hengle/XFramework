﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

[CustomEditor(typeof(TargetCamera),true),CanEditMultipleObjects]
public class TargetCameraInspector : Editor
{
    private TargetCamera targetCamera;
    private SerializedProperty mode;
    //private SerializedProperty attachTo;

    private void OnEnable()
    {
        targetCamera = (TargetCamera)target;
        mode = serializedObject.FindProperty("mode");
        //attachTo = serializedObject.FindProperty("attachTo");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(mode);
        SerializableObj(targetCamera);
        switch (targetCamera.mode)
        {
            case TargetCamera.Mode.AttachTo:
                SerializableObj(targetCamera.attachTo);
                break;
            case TargetCamera.Mode.SmoothFollow:
                //EditorGUILayout.PropertyField(attachTo, true);
                SerializableObj(targetCamera.smoothFollow);
                break;
            case TargetCamera.Mode.MouseOrbit:
                SerializableObj(targetCamera.mouseOrbit);
                break;
            case TargetCamera.Mode.LookAt:
                SerializableObj(targetCamera.lookAt);
                break;
            default:
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }


    /// <summary>
    /// 特么写了半天才发现给EditorGUILayout.PropertyField参数加个true就可以了，不管，老子就要用自己写的
    /// </summary>
    private void SerializableObj(object obj)
    {
        Type type = obj.GetType();
        FieldInfo[] fieldInfos = type.GetFields();

        GUI.backgroundColor = new Color32(0,170,255,30);
        EditorGUILayout.BeginVertical("box");
        GUI.backgroundColor = Color.white;
        foreach (var field in fieldInfos)
        {
            switch (field.FieldType.ToString())
            {
                case "System.Int32":
                    field.SetValue(obj, EditorGUILayout.IntField(field.Name, (int)field.GetValue(obj)));
                    break;
                case "System.Single":
                    field.SetValue(obj, EditorGUILayout.FloatField(field.Name, (float)field.GetValue(obj)));
                    break;
                case "System.Double":
                    field.SetValue(obj, EditorGUILayout.DoubleField(field.Name, (double)field.GetValue(obj)));
                    break;
                case "System.Boolean":
                    field.SetValue(obj, EditorGUILayout.Toggle(field.Name, (bool)field.GetValue(obj)));
                    break;
                case "System.String":
                    field.SetValue(obj, EditorGUILayout.TextField(field.Name, (string)field.GetValue(obj)));
                    break;
                case "System.Enum":
                    Debug.Log("a");
                    field.SetValue(obj, EditorGUILayout.EnumPopup(field.Name, (Enum)field.GetValue(obj)));
                    break;
                case "UnityEngine.Transform":
                    field.SetValue(obj, EditorGUILayout.ObjectField(field.Name, (Transform)field.GetValue(obj), typeof(Transform), true) as Transform);
                    break;
                case "UnityEngine.Vector3":
                    field.SetValue(obj, EditorGUILayout.Vector3Field(field.Name, (Vector3)field.GetValue(obj)));
                    break;
                case "UnityEngine.Vector2":
                    field.SetValue(obj, EditorGUILayout.Vector2Field(field.Name, (Vector3)field.GetValue(obj)));
                    break;
            }
        }
        EditorGUILayout.EndVertical();
    }
}