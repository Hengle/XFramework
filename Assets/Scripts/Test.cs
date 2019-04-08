using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using System.Text;
using System;
using XDEDZL.Collections;

public class Test : MonoSingleton<Test>
{
    [Serializable]
    public class AAA
    {
        public float a;
        public float b;
        public int c;

    }
    public string Close;
    public string Open;

    void Start()
    {

        AAA aaa = new AAA();
        aaa.a = 10;
        aaa.b = 108.999f;
        aaa.c = 800;

        byte[] t = IOUtility.Serialize(aaa);
        AAA q = IOUtility.Deserialize(t) as AAA;

        Debug.Log(q.a);
        Debug.Log(q.b);
        Debug.Log(q.c);
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

        if (Input.GetKeyDown(KeyCode.O))
        {
            UIHelper.Instance.OpenPanel(Open);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            UIHelper.Instance.ClosePanel(Close);
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