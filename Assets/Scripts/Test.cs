﻿using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using System.Text;
using System;

public class Test : MonoSingleton<Test>
{
    private short filter = 255;
    public List<byte> buf = new List<byte>();

    private int academyId = 1001;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {

        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            
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