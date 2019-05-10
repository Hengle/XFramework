﻿using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using System.Text;
using System;
using UnityEngine.Networking;
using System.Collections;
using XDEDZL.Collections;
using XDEDZL;
using XDEDZL.UI;
using XDEDZL.Extd;
using UnityEditor;
using System.Text.RegularExpressions;

public class Test : MonoSingleton<Test>
{
    public GameObject obj;
    private Material lineMaterial;
    public SpriteRenderer sprite;
    public Mesh mesh;
    void Start() 
    {
        lineMaterial = new Material(Shader.Find("RunTimeHandles/VertexColor"));
        lineMaterial.color = Color.white;


        Regex reg = new Regex("[0-9]*[.]{1}[0-9]*");
        MatchCollection matchs = reg.Matches("(1.25,2,5.33,5.2222222)");
        Debug.Log(matchs.Count);
        for (int i = 0; i < matchs.Count; i++)
        {
            Debug.Log(matchs[i].Value);
        }
    }

    private void OnPostRender()
    {
        //for (int i = 0; i < 200; i++)
        //{
        //    for (int j = 0; j < 200; j++)
        //    {
        //        Graphics.DrawMeshNow(mesh,new Vector3(i,0,j),Quaternion.identity);
        //    }
        //}
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            Debug.LogError(Application.dataPath);
            Debug.LogError(System.Environment.CurrentDirectory);
            Debug.LogError(Assembly.GetExecutingAssembly().GetName().CodeBase);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            Assembly asmb = Assembly.LoadFrom(@"file:///E:/github/xdedzl/Library/ScriptAssemblies/Assembly-CSharp.dll");
        }
    }

    private int GetMaterialIndex(RaycastHit hitInfo)
    {
        int matLength = hitInfo.collider.GetComponent<MeshRenderer>().materials.Length;
        int totalCount = 0;
        MeshFilter meshFilter = hitInfo.collider.GetComponent<MeshFilter>();

        int triangleIndex = hitInfo.triangleIndex;
        for (int i = 0; i < matLength; i++)
        {
            int count = meshFilter.mesh.GetTriangles(i).Length / 3;
            totalCount += count;
            if (triangleIndex < totalCount)
                return i;
        }
        return -1;
    }

    IEnumerator WebGet()
    {
        UnityWebRequest webRequest = UnityWebRequestAssetBundle.GetAssetBundle("http://www.baidu.com");
        yield return webRequest.SendWebRequest();

        if (webRequest.isHttpError || webRequest.isNetworkError)
            Debug.Log(webRequest.error);
        else
        {
            //AssetDatabase
            Debug.Log(webRequest.downloadHandler.text);
        }
    }


    #region Graph Test

    void DirGraph()
    {
        Graph<string> graph = new Graph<string>();
        graph.AddVertex("");
        graph.AddVertex("");
        graph.AddVertex("");
        graph.AddVertex("");
        graph.AddVertex("");
        graph.AddVertex("");

        graph.AddEdge(0, 1);
        graph.AddEdge(0, 5);
        graph.AddEdge(1, 0);
        graph.AddEdge(1, 2);
        graph.AddEdge(1, 3);
        graph.AddEdge(1, 4);
        graph.AddEdge(2, 1);
        graph.AddEdge(2, 3);
        graph.AddEdge(3, 1);
        graph.AddEdge(3, 2);
        graph.AddEdge(3, 4);
        graph.AddEdge(4, 1);
        graph.AddEdge(4, 3);
        graph.AddEdge(4, 5);
        graph.AddEdge(5, 0);
        graph.AddEdge(5, 4);

        graph.GetShortPath(0);
        Debug.Log("-------------------");

        int[,] dis = graph.GetShortPath();
        for (int i = 0; i < dis.GetLength(0); i++)
        {
            Debug.Log(dis[0, i]);
        }
    }

    void NoDirGraph()
    {
        Graph<string> graph = new Graph<string>(10, false);
        graph.AddVertex("");
        graph.AddVertex("");
        graph.AddVertex("");
        graph.AddVertex("");
        graph.AddVertex("");
        graph.AddVertex("");

        graph.AddEdge(0, 1);
        graph.AddEdge(1, 2);
        graph.AddEdge(2, 3);
        graph.AddEdge(3, 4);
        graph.AddEdge(4, 5);
        graph.AddEdge(0, 5);
        graph.AddEdge(1, 4);
        graph.AddEdge(1, 3);

        graph.Foreach((a) =>
        {
            Debug.Log(a);
        });
    }

    #endregion
}