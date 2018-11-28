using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XDEDZL.Mathematics;
using UniRx;

public class Test : MonoBehaviour
{
    private List<Vector3> positions;
    RaycastHit hit;
    HermiteCurve curve;
    public float c;

    public MeshFilter meshFilter;

    private List<Vector3> path = new List<Vector3>();

    private List<GameObject> objs = new List<GameObject>();


    private void Start()
    {
        curve = new HermiteCurve();
        positions = new List<Vector3>();
    }


    private void Update()
    {
        MouseLeft();

        if (Input.GetKeyDown(KeyCode.K))
        {
            for (int i = 0; i < positions.Count; i++)
            {
                curve.AddNode(positions[i], c);
            }

            for (int i = 0; i < curve.segmentList.Count; i++)
            {
                float add = 1f / 20;
                for (float j = 0; j < 1; j += add)
                {
                    Vector3 point = curve.segmentList[i].GetPoint(j);
                    path.Add(point);
                    objs.Add(Utility.CreatPrimitiveType(PrimitiveType.Sphere, point, 1, Color.red));
                }
            }

            //CreateRoads(meshFilter, path, 6);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            objs.ForEach((a) => { Destroy(a); });
            objs.Clear();
            curve = new HermiteCurve();
            positions.Clear();
            path.Clear();
        }
    }

    private void MouseLeft()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Vector3 worldHitPos = hit.point + Vector3.up * 5;
                positions.Add(worldHitPos);
                GameObject gameObject = Utility.CreatPrimitiveType(PrimitiveType.Cube, worldHitPos, 1f,Color.red);
                objs.Add(gameObject);
            }
        }
    }



    /// <summary>
    /// 根据传参 路点信息 创建路面
    /// </summary>
    /// <param name="meshFilter">路面网格</param>
    /// <param name="_roadPoints">路点</param>
    /// <param name="_width">路面宽度</param>
    private void CreateRoads(MeshFilter meshFilter, List<Vector3> _roadPoints, float _width)
    {
        if (_roadPoints.Count < 2) return;

        Mesh mesh = meshFilter.mesh;
        List<Vector3> vertice = new List<Vector3>();    // 顶点
        List<int> triangles = new List<int>();          // 三角形排序
        List<Vector2> uv = new List<Vector2>();         // uv排序

        Vector3 dir = PhysicsMath.GetHorizontalDir(_roadPoints[1], _roadPoints[0]);   // 获取两点间的垂直向量
        vertice.Add(_roadPoints[0] + dir * _width);
        vertice.Add(_roadPoints[0] - dir * _width);

        uv.Add(Vector2.zero);
        uv.Add(Vector2.right);

        for (int i = 1, count = _roadPoints.Count - 1; i < count; i++)
        {
            // 添加由路点生成的点集

            //Vector3 center = CircleCenter(_roadPoints[i - 1], _roadPoints[i], _roadPoints[i + 1]);
            //if (center == _roadPoints[i])
            //{
            //    // 如果三点成直线则无法形成一个圆
            //    dir = PhysicsMath.GetHorizontalDir(_roadPoints[i], _roadPoints[i - 1]);
            //}
            //else
            //{
            //    List<Vector3> temp = new List<Vector3>(_roadPoints);
            //    temp.RemoveAt(i);

            //    if (PhysicsMath.IsPointInsidePolygon(_roadPoints[i], temp))
            //    {
            //        dir = (center - _roadPoints[i]).normalized;
            //    }
            //    else
            //    {
            //        dir = (_roadPoints[i] - center).normalized; ;
            //    }
            //}

            dir = PhysicsMath.GetHorizontalDir(_roadPoints[i], _roadPoints[i - 1]);

            vertice.Add(_roadPoints[i] + dir * _width);
            vertice.Add(_roadPoints[i] - dir * _width);

            // 添加三jio形排序
            triangles.Add(2 * i - 2);
            triangles.Add(2 * i);
            triangles.Add(2 * i - 1);

            triangles.Add(2 * i);
            triangles.Add(2 * i + 1);
            triangles.Add(2 * i - 1);

            // 添加uv排序
            if (i % 2 == 1)
            {
                uv.Add(Vector2.up);
                uv.Add(Vector2.one);
            }
            else
            {
                uv.Add(Vector2.zero);
                uv.Add(Vector2.right);
            }
        }

        meshFilter.mesh.Clear();
        meshFilter.mesh.vertices = vertice.ToArray();
        meshFilter.mesh.triangles = triangles.ToArray();
        meshFilter.mesh.uv = uv.ToArray();

        meshFilter.mesh.RecalculateBounds();     // 重置范围
        meshFilter.mesh.RecalculateNormals();    // 重置法线
        meshFilter.mesh.RecalculateTangents();    // 重置切线
    }

    /// <summary>
    /// 获取三个点确定一个圆的圆心
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <param name="v3"></param>
    private Vector3 CircleCenter(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        // 三点可连成一条直线时无法确定一个圆
        if (Mathf.Abs((v1.x - v2.x) * (v1.y - v3.y) - (v1.x - v3.x) * (v1.y - v2.y)) < 0.2f)
        {
            return v2;
        }

        float a = v1.x - v2.x;
        float b = v1.y - v2.y;
        float c = v1.x - v3.x;
        float d = v1.y - v3.y;

        float e = ((v1.x * v1.x - v2.x * v2.x) - (v2.y * v2.y - v1.y * v1.y)) * 0.5f;
        float f = ((v1.x * v1.x - v3.x * v3.x) - (v3.y * v3.y - v1.y * v1.y)) * 0.5f;

        float x = -(d * e - b * f) / (b * c - a * d);
        float z = -(a * f - c * e) / (b * c - a * d);

        return new Vector3(-x, v2.y, -z);
    }
}