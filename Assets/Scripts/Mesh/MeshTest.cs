// ==========================================
// 描述： 
// 作者： HAK
// 时间： 2018-10-31 13:13:54
// 版本： V 1.0
// ==========================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RCXC;
using System.Linq;
using System;

public class MeshTest : MonoBehaviour
{
    public bool isDraw = false;

    List<Vector3> positions = new List<Vector3>();
    Vector3 point = Vector3.zero;
    int id = 0;

    public float alpha = 30;
    public float theta = 30;
    public float polygonHeight = 500;
    public float AirSpaceWidth = 600;
    public float AirSpaceHeight = 600;
    public Color cylinderColor = Color.red;
    public Color polygonColor = Color.red;
    public Color airCorridorSpaceColor = Color.red;
    public Color sectorColor = Color.red;

    List<Vector3> points_1 = new List<Vector3>();
    List<Vector3> points_2 = new List<Vector3>();

    void Start()
    {
    }

    private void Update()
    {
        if (isDraw)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    Vector3 worldHitPos = hit.point + new Vector3(0, 1, 0);
                    positions.Add(worldHitPos);
                    Debug.Log(worldHitPos);
                }
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                Revert();
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                SetFalse(--id);
            }

            if (positions == null || positions.Count == 0)
            {
                return;
            }

            //圆柱
            if (Input.GetKeyDown(KeyCode.Z))
            {
                CreateCylinder();
                Revert();
            }
            //多边形
            if (Input.GetKeyDown(KeyCode.X) && positions.Count > 2)
            {
                CreatePolygon();
                Revert();
            }
            //空中走廊
            if (Input.GetKeyDown(KeyCode.C) && positions.Count > 1)
            {
                CreateAirCorridorSpace();
                Revert();
            }
            //扇形区域
            if (Input.GetKeyDown(KeyCode.V) && positions.Count > 1)
            {
                CreatSector_2();
                Revert();
            }
            //半圆防空区域
            if (Input.GetKeyDown(KeyCode.B))
            {
                CreateHemisphere();
                Revert();
            }
            //两个圆柱，近距离等待空域
            if (Input.GetKeyDown(KeyCode.N))
            {
                CreatDoubleCylinder();
                Revert();
            }
            //杀伤盒 两个矩形 
            if (Input.GetKeyDown(KeyCode.M) && positions.Count > 2)
            {
                CreatKillBox();
                Revert();
            }
        }
    }

    private void CreateCylinder()
    {
        Singleton<MeshManager>.Instance.CreateCylinder(id++, positions[0], 2500, 1500, Color.blue, Color.blue);
    }

    private void CreatePolygon()
    {
        Singleton<MeshManager>.Instance.CreatePolygon(id++, positions, 0.01f, Color.red, Color.red);
    }

    private void CreateAirCorridorSpace()
    {
        Singleton<MeshManager>.Instance.CreateAirCorridorSpace(id++, positions, 1500, 2500, Color.green);
    }

    private void CreatSector_2()
    {
        Vector3[] points = PhysicsMath.GetSectorPoints_2(positions[0], new Vector3(positions[1].x, positions[0].y, positions[1].z), alpha, theta);
        Singleton<MeshManager>.Instance.CreateSector(id++, positions[0], positions[1], alpha, theta, Color.yellow, Color.yellow);
    }

    private void CreateHemisphere()
    {
        Singleton<MeshManager>.Instance.CreateHemisphere(id++, positions[0] - Vector3.up * 1500, 7000, new Color(0.784f, 0.784f, 1));
    }

    private void CreatKillBox()
    {
        Singleton<MeshManager>.Instance.CreateKillBox(id++, positions, 3400, 6000, new Color(0.706f, 0.235f, 1), Color.black);
    }

    private void CreatDoubleCylinder()
    {
        Singleton<MeshManager>.Instance.DoubleCylinder(id++, positions[0], 2500, 1500, 1000, new Color(1, 0.392f, 0), Color.black);
    }

    /// <summary>
    /// 清空所有点
    /// </summary>
    private void Revert()
    {
        positions.Clear();
        positions = new List<Vector3>();
    }

    private void SetFalse(int id)
    {
        Singleton<MeshManager>.Instance.SetMeshPrefabFalse(id);
    }
}
