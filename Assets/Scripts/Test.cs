using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Test : MonoSingleton<Test>
{
    private bool boolTest = true;
    public Mesh mesh;
    private void QQQ(in List<int> yyy)
    {
        yyy[0] = 100;
        Debug.Log(yyy[0]);
    }

    void Start()
    {
        Debug.Log(transform.rotation);
        Debug.Log(Quaternion.LookRotation(transform.forward));
        Debug.Log(Quaternion.Euler(transform.eulerAngles));
    }

    private void OnPostRender()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J)) 
        {

        }
    }

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

    private void HAKULAMATATA()
    {
        var a = typeof(BBB).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
        BBB b = new BBB();
    }
}

public class AAA
{
    
    protected AAA()
    {
        Debug.Log("AAA午餐");
        
    }

    public AAA(int a)
    {
        Debug.Log("AAAwan餐");
    }
}

public class BBB : AAA
{

}