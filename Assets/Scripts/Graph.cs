using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 图形数据结构，有向图，无向图
/// </summary>
public class Graph<T>
{
    /// <summary>
    /// 图的顶点数组
    /// </summary>
    private VertexNode[] nodes;
    /// <summary>
    /// 当前索引
    /// </summary>
    private int index;

    public Graph(int capacity = 10)
    {
        nodes = new VertexNode[capacity];
    }

    public void AddVertex(T node)
    {
        // 扩容
        if(index >= nodes.Length)
        {
            VertexNode[] tempNodes = new VertexNode[nodes.Length * 2];
            for (int i = 0,length = nodes.Length; i < length; i++)
            {
                tempNodes[i] = nodes[i];
            }
            nodes = tempNodes;
        }
        nodes[index] = new VertexNode(node);
        index++;
    }




    /// <summary>
    /// 边表结点
    /// </summary>
    private class EdgeNode
    {
        /// <summary>
        /// 存储顶点对应的下标
        /// </summary>
        public int adjvex;
        /// <summary>
        /// 存储权值
        /// </summary>
        public int weight;
        /// <summary>
        /// 指向下一个邻接点
        /// </summary>
        public EdgeNode next;
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
        /// 指向第一条依附该顶点的弧
        /// </summary>
        public EdgeNode firstNode;
        /// <summary>
        /// 访问标识符
        /// </summary>
        public bool visited;

        public VertexNode(T _data)
        {
            data = _data;
        }
    }
}
