using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using System.Text;
using System;
using UnityEngine.Networking;
using System.Collections;
using XDEDZL.Collections;
using XDEDZL.UI;
using UnityEditor;

public class Test : MonoSingleton<Test>
{
    void Start()
    {
        XDEDZL.Collections.List<BasePanel> list = new XDEDZL.Collections.List<BasePanel>();
        
        for (int i = 0; i < 1000000; i++)
        {
            list.Add(new BasePanel());
        }
        BasePanel a = new ButtonListPanel();
        list.Add(a);

        Debug.Log(Utility.DebugActionRunTime(() =>
        {
            list.Remove(a);
        }));


        System.Collections.Generic.List<BasePanel> l = new System.Collections.Generic.List<BasePanel>();
       
        for (int i = 0; i < 1000000; i++)
        {
            l.Add(new BasePanel());
        }
        BasePanel b = new ButtonListPanel();
        l.Add(b);

        Debug.Log(Utility.DebugActionRunTime(() =>
        {
            l.Remove(b);
        }));
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