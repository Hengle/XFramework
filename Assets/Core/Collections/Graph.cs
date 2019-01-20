using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 图形数据结构，有向图
/// </summary>
public class Graph<T>
{
    /// <summary>
    /// 图的顶点数组
    /// </summary>
    private VertexNode[] vertexs;
    /// <summary>
    /// 下一个添加的顶点索值引，当前顶点数量
    /// </summary>
    private int index;

    public Graph(int capacity = 10)
    {
        vertexs = new VertexNode[capacity];
    }

    /// <summary>
    /// 添加顶点
    /// </summary>
    /// <param name="data"></param>
    public void AddVertex(T data)
    {
        // 扩容
        if(index >= vertexs.Length)
        {
            VertexNode[] tempNodes = new VertexNode[vertexs.Length * 2];
            for (int i = 0,length = vertexs.Length; i < length; i++)
            {
                tempNodes[i] = vertexs[i];
            }
            vertexs = tempNodes;
        }
        vertexs[index] = new VertexNode(data);
        index++;
    }

    /// <summary>
    /// 添加边
    /// </summary>
    /// <param name="fromIndex"></param>
    /// <param name="toIndex"></param>
    /// <param name="weight"></param>
    public void AddEdge(int fromIndex, int toIndex, int weight = 1)
    {
        if (fromIndex > index || toIndex > index)
        {
            throw new System.Exception("添加临界点的索引超出范围");
        }

        Edge newEdge = new Edge(fromIndex, toIndex, weight);
        // 添加邻接表元素
        Edge edge = vertexs[fromIndex].firstOut;
        if (edge == null)
        {
            vertexs[fromIndex].firstOut = newEdge;
        }
        else
        {
            while (edge.headLink != null)
            {
                edge = edge.headLink;
                // 重复添加的判断
            }
            edge.headLink = newEdge;
        }

        // 添加逆邻接表元素
        edge = vertexs[toIndex].firstIn;
        if (edge == null)
        {
            vertexs[toIndex].firstIn = newEdge;
        }
        else
        {
            while (edge.tailLink != null)
            {
                edge = edge.tailLink;
                // 重复添加的判断

            }
            edge.tailLink = newEdge;
        }
    }

    /// <summary>
    /// 找寻最短路径
    /// </summary>
    public void GetShortPath(int startIndex, int endIndex)
    {
        // 初始化最短路径集合，第i个值代表从startIndex到i的最短路径
        Path[] paths = new Path[index];
        for (int i = 0,length = paths.Length; i < length; i++)
        {
            paths[i] = new Path();
            paths[i].length = int.MaxValue;
        }
        Edge edge = vertexs[startIndex].firstOut;
        paths[startIndex].length = 0;
        paths[startIndex].ensure = true;
        // 以startIndex为起点距离最短的边
        Edge shortLink = edge;
        // 给路径长度数组附初始值
        while (edge != null)
        {
            paths[edge.tailIndex].length = edge.weight;
            if(edge.weight < shortLink.weight)
            {
                shortLink = edge;
            }
            edge = edge.headLink;
        }

        // 已经确定了最短路径的顶点数量
        int count = 1;
        while(count < paths.Length)
        {
            // 记录未确定的路径长度最小值
            int tempIndex = 0;
            int min = int.MaxValue;
            for (int i = 0; i < paths.Length; i++)
            {
                if(paths[i].ensure == false && paths[i].length < min)
                {
                    min = paths[i].length;
                    tempIndex = i;
                }
            }

            Edge edgeIn = vertexs[tempIndex].firstIn;
            while(edgeIn != null)
            {
                Debug.Log(edgeIn.headIndex + " : " + paths[edgeIn.headIndex]);
                if (paths[edgeIn.headIndex].length != int.MaxValue && paths[edgeIn.headIndex].length + edgeIn.weight < paths[tempIndex].length)
                {
                    paths[tempIndex].length = paths[edgeIn.headIndex].length + edgeIn.weight;
                }
                edgeIn = edgeIn.tailLink;
            }
            count++;
        }

        foreach (var item in paths)
        {
            UnityEngine.Debug.Log(item.length);
        }
    }

    /// <summary>
    /// 边表结点
    /// </summary>
    private class Edge
    {
        /// <summary>
        /// 有向边起点对应的下标
        /// </summary>
        public int headIndex;
        /// <summary>
        /// 有向边尾点对应的下标
        /// </summary>
        public int tailIndex;
        /// <summary>
        /// 存储权值
        /// </summary>
        public int weight;
        /// <summary>
        /// 指向起点相同的下一条边
        /// </summary>
        public Edge headLink;
        /// <summary>
        /// 指向终点相同的下一条边
        /// </summary>
        public Edge tailLink;

        public Edge(int _headIdnex, int _tailIndex, int _weight = 1)
        {
            headIndex = _headIdnex;
            tailIndex = _tailIndex;
            weight = _weight;
        }
    }

    /// <summary>
    /// 顶点表结构
    /// </summary>
    private class VertexNode
    {
        /// <summary>
        /// 顶点信息
        /// </summary>
        public T data;
        /// <summary>
        /// 指向出边表的第一个结点，组成逆邻接表
        /// </summary>
        public Edge firstOut;
        /// <summary>
        /// 指向入边表的第一个结点，组成邻接表
        /// </summary>
        public Edge firstIn;
        /// <summary>
        /// 访问标识符
        /// </summary>
        public bool visited;

        public VertexNode(T _data)
        {
            data = _data;
        }
    }

    private class Path
    {
        public List<int> pathIndexes = new List<int>();
        public int length;
        public bool ensure;
    }
}
