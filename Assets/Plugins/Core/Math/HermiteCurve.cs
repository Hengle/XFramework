// ==========================================
// 描述： 
// 作者： HAK
// 时间： 2018-11-28 11:31:34
// 版本： V 1.0
// ==========================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 样条类型
/// </summary>
public enum SplineMode
{
    Hermite,               // 埃尔米特样条
    Catmull_Rom,           // 
    CentripetalCatmull_Rom,// 向心Catmull_Rom
}

/// <summary>
/// 埃尔米特曲线
/// </summary>
public class HermiteCurve
{
    /// <summary>
    /// 曲线起始节点
    /// </summary>
    private Vector3 startNode;
    /// <summary>
    /// 曲线终结点
    /// </summary>
    private Vector3 endNode;
    /// <summary>
    /// 节点集合
    /// </summary>
    private List<Vector3> nodeList;
    /// <summary>
    /// 节点法线集合
    /// </summary>
    private List<Vector3> tangentsList;
    /// <summary>
    /// 曲线段集合
    /// </summary>
    public List<HermiteSegement> segmentList { get; private set; }
    /// <summary>
    /// 曲线构造类型
    /// </summary>
    public SplineMode mode { get; private set; }

    public HermiteCurve(SplineMode _mode = SplineMode.Hermite)
    {
        nodeList = new List<Vector3>();
        tangentsList = new List<Vector3>();
        segmentList = new List<HermiteSegement>();
        mode = _mode;
    }

    private void AddCatmull_RomControl()
    {
        if(mode != SplineMode.Catmull_Rom)
        {
            Debug.Log("不是Catmull样条");
            return;
        }
        if(nodeList.Count < 2)
        {
            Debug.Log("Catmull_Rom样条取点要大于等于2");
            return;
        }
        nodeList.Insert(0, startNode + (nodeList[0] - nodeList[1]));
        nodeList.Add(endNode + (endNode - nodeList[nodeList.Count - 2]));
    }

    /// <summary>
    /// 添加节点
    /// </summary>
    /// <param name="newNode"></param>
    public void AddNode(Vector3 newNode, float c)
    {
        nodeList.Add(newNode);
        
        if(nodeList.Count > 1)
        {
            HermiteSegement a = new HermiteSegement(endNode, newNode,this);
            a.c = c;
            segmentList.Add(a);
            CaculateTangents(segmentList.Count - 1);               // 计算新加入的曲线段起始切线
        }
        else // 加入第一个节点
        {
            startNode = newNode;
        }
        endNode = newNode;
    }

    /// <summary>
    /// 获取点
    /// </summary>
    /// <param name="index"></param>
    /// <param name="t"></param>
    public void GetPoint(int index, float t)
    {
        segmentList[index].GetPoint(t);
    }

    /// <summary>
    /// 获取切线
    /// </summary>
    /// <param name="index"></param>
    /// <param name="t"></param>
    public void GetTangents(int index, float t)
    {
        segmentList[index].GetTangents(t);
    }

    /// <summary>
    /// 计算曲线段首尾切线
    /// </summary>
    /// <param name="index"></param>
    private void CaculateTangents(int index)
    {
        HermiteSegement segement = segmentList[index];

        if(index == 0)
        {
            segement.startTangents = segement.endPos - segement.startPos;
            segement.endTangents = segement.endPos - segement.startPos;
            return;
        }

        HermiteSegement preSegement = segmentList[index - 1];

        segement.startTangents = 0.5f * (1 - segement.c) * (segement.endPos - preSegement.endPos);
        segement.endTangents = segement.endPos - segement.startPos;
        preSegement.endTangents = segement.startTangents;

    }
}

/// <summary>
/// 曲线段
/// </summary>
public class HermiteSegement
{
    /// <summary>
    /// 所属曲线
    /// </summary>
    public HermiteCurve rootCurve;

    /// <summary>
    /// 曲线段起始位置
    /// </summary>
    public Vector3 startPos { get; private set; }
    /// <summary>
    /// 曲线段末尾位置
    /// </summary>
    public Vector3 endPos { get; private set; }
    /// <summary>
    /// 前一个线段的末尾位置
    /// </summary>
    public Vector3 prePos { get; private set; }
    /// <summary>
    /// 后一个线段的起始位置
    /// </summary>
    public Vector3 nextPos { get; private set; }

    public Vector3 startTangents;
    public Vector3 endTangents;

    /// <summary>
    /// 张力系数
    /// </summary>
    public float c { get;  set; }

    public HermiteSegement(Vector3 _startPos,Vector3 _endPos,HermiteCurve _rootCurve)
    {
        startPos = _startPos;
        endPos = _endPos;
        rootCurve = _rootCurve;
        c = -5f;
    }

    /// <summary>
    /// 获取点
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public Vector3 GetPoint(float t)
    {
        Vector3 x = Vector3.zero;
        switch (rootCurve.mode)
        {
            case SplineMode.Hermite:
                x = (2 * t * t * t - 3 * t * t + 1) * startPos;
                x += (-2 * t * t * t + 3 * t * t) * endPos;
                x += (t * t * t - 2 * t * t + t) * startTangents;
                x += (t * t * t - t * t) * endTangents;
                //float y = (2 * t * t * t - 3 * t * t + 1) * startPos.y;
                //y += (-2 * t * t * t + 3 * t * t) * endPos.y;
                //y += (t * t * t - 2 * t * t + t) * startTangents.y;
                //y += (t * t * t - t * t) * endTangents.z;

                //float z = (2 * t * t * t - 3 * t * t + 1) * startPos.z;
                //z += (t * t * t - 2 * t * t + t) * startTangents.z;
                //z += (-2 * t * t * t + 3 * t * t) * endPos.z;
                //z += (t * t * t - t * t) * endTangents.z;
                break;
            case SplineMode.Catmull_Rom:
                break;
            case SplineMode.CentripetalCatmull_Rom:
                break;
            default:
                break;
        }
        

        

        return x;

    }

    /// <summary>
    /// 获取切线
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public Vector3 GetTangents(float t)
    {
        Vector3 a = (6 * t * t - 6 * t) * startPos;
        a += (-6 * t * t + 6 * t) * endPos;
        a += (3 * t * t - 4 * t + 1) * startTangents;
        a += (3 * t * t - 2 * t) * endTangents;
        return a;
    }
}

public class Node
{
    public Vector3 node;
    public Node preNode;
    public Node nextNode;
}
