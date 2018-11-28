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
/// 埃尔米特曲线
/// </summary>
public class HermiteCurve
{
    private Vector3 startNode;
    private Vector3 endNode;
    private List<Vector3> nodeList;
    private List<Vector3> tangentsList;
    public List<HermiteSegement> segmentList { get; private set; }

    public HermiteCurve()
    {
        nodeList = new List<Vector3>();
        tangentsList = new List<Vector3>();
        segmentList = new List<HermiteSegement>();
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
            HermiteSegement a = new HermiteSegement(endNode, newNode);
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
    /// 起算曲线段起始切线
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
    public Vector3 startPos { get; private set; }
    public Vector3 endPos { get; private set; }
    public Vector3 startTangents;
    public Vector3 endTangents;

    /// <summary>
    /// 张力系数
    /// </summary>
    public float c { get;  set; }

    public HermiteSegement(Vector3 _startPos,Vector3 _endPos/*,Vector3 _startTan,Vector3 _endTan*/)
    {
        startPos = _startPos;
        endPos = _endPos;
        //startTangents = _startTan;
        //endTangents = _endTan;
        c = -5f;
    }

    /// <summary>
    /// 获取点
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public Vector3 GetPoint(float t)
    {
        Vector3 x = (2 * t * t * t - 3 * t * t + 1) * startPos;
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
