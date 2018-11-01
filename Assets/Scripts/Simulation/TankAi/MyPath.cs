// ==========================================
// 描述： 
// 作者： HAK
// 时间： 2018-10-23 11:48:30
// 版本： V 1.0
// ==========================================
using Pathfinding;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MyPath
{
    public Vector3[] wayPoints;     // 所有路径点
    public int index = -1;          // 当前路点索引
    bool isLoop = false;            // 是否循环
    public float deviation = 5;     // 到达误差
    public bool isFinish = true;    // 是否完成

    public Vector3 WayPoint { get; private set; } // 当前的路点

    /// <summary>
    /// 是否到达目的地
    /// </summary>
    public bool IsReach(Transform trans)
    {
        Vector3 pos = trans.position;
        float distance = Vector3.Distance(WayPoint, pos);
        return distance < deviation;
    }

    /// <summary>
    /// 下一个路点
    /// </summary>
    public void NextWaypoint()
    {
        if (index < 0)
            return;

        if (index < wayPoints.Length - 1)
        {
            index++;
        }
        else
        {
            if (isLoop)
                index = 0;
            else
                isFinish = true;
        }
        WayPoint = wayPoints[index];
    }

    #region 生成路径的三种方式
    /// <summary>
    /// 根据据场景标识物生成路点
    /// </summary>
    /// <param name="obj">路径点父物体</param>
    /// <param name="isLoop">是否循环</param>
    public void InitByObj(GameObject obj, bool isLoop = false)
    {
        int length = obj.transform.childCount;
        //没有子物体
        if (length == 0)
        {
            wayPoints = null;
            index = -1;
            Debug.LogWarning("Path.InitByObj length == 0");
            return;
        }
        //遍历子物体生成路点
        wayPoints = new Vector3[length];
        for (int i = 0; i < length; i++)
        {
            Transform trans = obj.transform.GetChild(i);
            wayPoints[i] = trans.position;
        }
        //设置一些参数
        index = 0;
        WayPoint = wayPoints[index];
        this.isLoop = isLoop;
        isFinish = false;
    }

    /// <summary>
    /// 根据NavMesh初始化路径
    /// </summary>
    /// <param name="pos">当前位置</param>
    /// <param name="targetPos">目标位置</param>
    /// <param name="transform">行动单位</param>
    public void InitByNavMeshPath(Vector3 pos, Vector3 targetPos, Transform transform)
    {
        //重置
        wayPoints = null;
        index = -1;

        NavMeshPath navPath = new NavMeshPath();
        Terrain terrain = Terrain.activeTerrain;
        bool hasFoundPath = NavMesh.CalculatePath(pos, targetPos, NavMesh.AllAreas, navPath); // 计算路径
        if (!hasFoundPath)
        {
            Vector3 dir = (pos - targetPos).normalized;
            for (int i = 1; i < Vector3.Distance(pos, targetPos); i++)
            {
                //如果targetPos不可达，就让目标位置与物体方向各自相对移动一个单位
                targetPos += dir;
                targetPos.y = terrain.SampleHeight(targetPos);
                pos -= dir;
                pos.y = terrain.SampleHeight(pos);
                hasFoundPath = NavMesh.CalculatePath(pos, targetPos, NavMesh.AllAreas, navPath); // 计算到达新的目标的路径

                if (hasFoundPath)
                {
                    break;
                }
            }
            // for循环结束依然没有找到可达的目标的则return
            if (!hasFoundPath)
            {
                Debug.LogError("当前位置到目标位置连线上点均不可达: " + transform.name);
                return;
            }
        }
        
        //生成路径
        int length = navPath.corners.Length;
        wayPoints = new Vector3[length];
        for (int i = 0; i < length; i++)
        {
            // 路径点集赋值
            wayPoints[i] = navPath.corners[i];
        }
        index = 0;
        WayPoint = wayPoints[index]; // 设置当前路径点为除去自身点以外的第一个点
        isFinish = false;
    }

    /// <summary>
    /// 通过AStar寻找路径
    /// </summary>
    /// <param name="startPos">起始位置</param>
    /// <param name="endPos">目标位置</param>
    /// <param name="callBack">路径计算完成调用的方法</param>
    public void InitByAStar(Vector3 startPos, Vector3 endPos, OnPathDelegate callBack = null)
    {
        ABPath path = ABPath.Construct(startPos, endPos, callBack);
        AstarPath.StartPath(path);  // 异步执行


    }
    #endregion

    /// <summary>
    /// 重置路径
    /// </summary>
    /// <param name="wayPints"></param>
    public void ResetPath(Vector3[] wayPints)
    {
        wayPoints = wayPints;
        index = 0;
        WayPoint = wayPints[index];
        isFinish = false;
    }

    /// <summary>
    /// 在场景中显示观察路径点
    /// </summary>
    public void DrawWaypoints()
    {
        if (wayPoints == null)
            return;
        int length = wayPoints.Length;
        for (int i = 0; i < length; i++)
        {
            if (i == index)
                Gizmos.DrawSphere(wayPoints[i], 1);
            else
                Gizmos.DrawCube(wayPoints[i], Vector3.one);
        }
    }
}