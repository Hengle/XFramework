// ==========================================
// 描述： 
// 作者： HAK
// 时间： 2018-10-31 12:07:09
// 版本： V 1.0
// ==========================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RCXC;
using System.Linq;

namespace RCXC
{
    /// <summary>
    /// 所有的MeshPrefab 的管理类, 单例
    /// </summary>
    public class MeshManager
    {
        //// 类型枚举
        //public enum Shape
        //{
        //    Cylinder = 0,
        //    Polygon,
        //    AirCorridorSpace
        //}
        
        // 检测数据集是否正确
        public List<Vector3> DataCheck(List<Vector3> list)
        {
            List<Vector3> outList = new List<Vector3>();
            for (int i = 0; i < list.Count - 1; i++)
            {
                if (list[i] != list[i + 1])
                {
                    outList.Add(list[i]);
                }
            }
            if (list[list.Count - 1] != list[0])
            {
                outList.Add(list[list.Count - 1]);
            }
            return outList;
        }

        public Dictionary<int, List<GameObject>> meshDic = new Dictionary<int, List<GameObject>>();     // 命令与mesh对应字典

        /// <summary>
        /// 创建圆柱形区域
        /// </summary>
        /// <param name="id"> 命令ID,用作字典key </param>
        /// <param name="point"> 圆心点 </param>
        /// <param name="radius"> 半径 </param>
        /// <param name="height"> 高度 </param>
        public void CreateCylinder(int id, Vector3 point, float radius, float height, Color Fillcolor,Color BoradColor)
        {
            LineRenderer[] lineRenderers = new LineRenderer[4];
            List<GameObject> meshList = GetMeshPrefab(lineRenderers, 1);                // 获取mesh
            meshDic.Add(id, meshList);                                                  // 加入字典
            MeshFilter meshFilter = meshList[0].GetComponent<MeshFilter>();             // 获取meshfilter
            meshList[0].GetComponent<MeshRenderer>().material.color = Fillcolor;

            Vector3[] vertices = PhysicsMath.GetCirclePoints(point, radius);             // 获取点集
            DrawTriangles.DrawCylinder(vertices, height, meshFilter, lineRenderers,BoradColor);    // 画出图形                 
          
        }

        /// <summary>
        /// 创建两个圆柱，近距火力支援等待空域
        /// </summary>
        /// <param name="id"> 命令ID,用作字典key </param>
        /// <param name="point">低点圆柱的底面圆心点</param>
        /// <param name="radius">半径</param>
        /// <param name="height">圆柱高度</param>
        /// <param name="heightDifference">两个圆柱的高度差</param>
        public void DoubleCylinder(int id, Vector3 point, float radius, float height, float heightDifference, Color Fillcolor,Color BoradColor)
        {
            //CreateCylinder(id, point, radius, height, color);
            //Vector3 highPoint = point + Vector3.up * (height + heightDifference);
            //CreateCylinder(++id, highPoint, radius, height, color);

            LineRenderer[] lineRenderers1 = new LineRenderer[4];
            LineRenderer[] lineRenderers2 = new LineRenderer[4];
            List<GameObject> meshList = GetMeshPrefab(lineRenderers1, 1);
            meshList.Add(GetMeshPrefab(lineRenderers2, 1)[0]);
            meshDic.Add(id, meshList);                                                  // 加入字典

            // 下面圆柱
            MeshFilter meshFilter1 = meshList[0].GetComponent<MeshFilter>();            
            meshList[0].GetComponent<MeshRenderer>().material.color = Fillcolor;
            Vector3[] vertices1 = PhysicsMath.GetCirclePoints(point, radius);            
            DrawTriangles.DrawCylinder(vertices1, height, meshFilter1, lineRenderers1,BoradColor);

            // 上面圆柱
            Vector3 highPoint = point + Vector3.up * (height + heightDifference);
            MeshFilter meshFilter2 = meshList[1].GetComponent<MeshFilter>();            
            meshList[1].GetComponent<MeshRenderer>().material.color = Fillcolor;
            Vector3[] vertices2 = PhysicsMath.GetCirclePoints(highPoint, radius);            
            DrawTriangles.DrawCylinder(vertices2, height, meshFilter2, lineRenderers1,BoradColor); 

        }

        /// <summary>
        /// 创建通用多边形区域
        /// </summary>
        /// <param name="id"> 命令ID,用作字典key </param>
        /// <param name="list">下底面点链表</param>
        /// <param name="height">高度</param>
        public void CreatePolygon(int id, List<Vector3> list, float height, Color Fillcolor,Color BoradColor)
        {
            LineRenderer[] lineRenderers = new LineRenderer[4];
            List<GameObject> meshList = GetMeshPrefab(lineRenderers, 1);                    // 获取mesh
            meshDic.Add(id, meshList);                                                      // 加入字典
            MeshFilter meshFilter = meshList[0].GetComponent<MeshFilter>();                 // 获取meshfilter
            meshList[0].GetComponent<MeshRenderer>().material.color = Fillcolor;

            Vector3[] vector3s = PhysicsMath.CheckVector(list);                              // 使数组逆时针排序
            DrawTriangles.DrawPolygon(vector3s, height, meshFilter,lineRenderers,BoradColor);          // 画出图形
        }

        /// <summary>
        /// 创建空中走廊
        /// </summary>
        /// <param name="id"> 命令ID,用作字典key </param>
        /// <param name="list">中心点链表</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        public void CreateAirCorridorSpace(int id, List<Vector3> list, float width, float height, Color Fillcolor)
        {
            LineRenderer[] lineRenderers = new LineRenderer[4];
            List<GameObject> meshList = GetMeshPrefab(lineRenderers, 1);                    // 获取mesh
            meshDic.Add(id, meshList);                                                      // 加入字典
            MeshFilter meshFilter = meshList[0].GetComponent<MeshFilter>();                 // 获取meshfilter
            meshList[0].GetComponent<MeshRenderer>().material.color = Fillcolor;

            //Vector3[] vertices = PhysicsMath.GetAirCorridorSpace(list, width, height);       // 获取点集
            //DrawTriangles.DrawAirCorridorSpace(vertices, meshFilter, lineRenderers);        // 画出图形

            Vector3[] bottomPoints = PhysicsMath.GetAirSpaceBottomPoints(list, width, height);
            Vector3[] vertices = PhysicsMath.GetAirBottomSpaceWithSector(bottomPoints.ToList(), width);

            DrawTriangles.GetAirSpaceWithSector(vertices.ToList(), width, height, list.Count - 1, meshFilter);
        }

        /// <summary>
        /// 创建扇形防空区
        /// </summary>
        /// <param name="id"> 命令ID,用作字典key </param>
        /// <param name="origin">起始点</param>
        /// <param name="tarPoint">水平最远距离点</param>
        /// <param name="alpha">横向张角</param>
        /// <param name="theta">纵向张角</param>
        public void CreateSector(int id, Vector3 origin, Vector3 tarPoint, float alpha, float theta, Color Fillcolor,Color BoradColor)
        {
            LineRenderer[] lineRenderers = new LineRenderer[4];
            List<GameObject> meshList = GetMeshPrefab(lineRenderers, 1);
            meshDic.Add(id, meshList);
            MeshFilter meshFilter = meshList[0].GetComponent<MeshFilter>();
            meshList[0].GetComponent<MeshRenderer>().material.color = Fillcolor;

            Vector3[] vertices = PhysicsMath.GetSectorPoints_2(origin, tarPoint, alpha, theta);
            DrawTriangles.DrawPolygon(vertices, meshFilter,lineRenderers,BoradColor);
        }

        /// <summary>
        /// 创建紫色杀伤盒
        /// </summary>
        /// <param name="id"> 命令ID,用作字典key </param>
        /// <param name="list">底面四点链表</param>
        /// <param name="lower">下限高度</param>
        /// <param name="Ceiling">上限高度</param>
        public void CreateKillBox(int id, List<Vector3> list, float lower, float Ceiling, Color Fillcolor,Color BoradColor)
        {            
            LineRenderer[] lineRenderers1 = new LineRenderer[4];
            LineRenderer[] lineRenderers2 = new LineRenderer[4];
            List<GameObject> meshList = GetMeshPrefab(lineRenderers1, 1);
            meshList.Add(GetMeshPrefab(lineRenderers2, 1)[0]);
            meshDic.Add(id, meshList);                                                              // 创建2个meshPrefab加入字典

            // 第一个杀伤盒
            MeshFilter meshFilter1 = meshList[0].GetComponent<MeshFilter>();                        // 获取meshfilter
            meshList[0].GetComponent<MeshRenderer>().material.color = Fillcolor;
            Vector3[] vector3s1 = PhysicsMath.CheckVector(list);                                     // 使数组逆时针排序
            DrawTriangles.DrawPolygon(vector3s1, lower, meshFilter1, lineRenderers1,BoradColor);               // 画出图形

            // 第二个杀伤盒
            MeshFilter meshFilter2 = meshList[1].GetComponent<MeshFilter>();                        // 获取meshfilter
            meshList[1].GetComponent<MeshRenderer>().material.color = Fillcolor;

            List<Vector3> CeilingList = new List<Vector3>();   // 中层顶点集合
            foreach (var item in list)
            {
                CeilingList.Add(item + Vector3.up * lower);
            }

            Vector3[] vector3s2 = PhysicsMath.CheckVector(CeilingList);                               // 使数组逆时针排序
            DrawTriangles.DrawPolygon(vector3s2, Ceiling - lower, meshFilter2, lineRenderers2,BoradColor);      // 画出图形
        }

        /// <summary>
        /// 创建蓝色杀伤盒
        /// </summary>
        /// <param name="id"> 命令ID,用作字典key </param>
        /// <param name="list">底面四点链表</param>
        /// <param name="ceiling">上限高度</param>
        public void CreateKillBox(int id, List<Vector3> list, float ceiling, Color Fillcolor,Color BoradColor)
        {
            CreatePolygon(id, list, ceiling, Fillcolor,BoradColor);
        }

        /// <summary>
        /// 创建指示线条
        /// </summary>
        /// <param name="id"> 命令ID,用作字典key </param>
        /// <param name="positions">点集</param>
        /// <param name="wide">宽度</param>
        public void CreateLine(int id, List<Vector3> positions, float wide, Color Fillcolor)
        {
            LineRenderer[] lineRenderers = new LineRenderer[5];
            List<GameObject> meshList = GetMeshPrefab(lineRenderers, 1);                    // 获取mesh与lineRenderer
            meshDic.Add(id, meshList);                                                      // 加入字典

            LineRenderer lineRenderer = lineRenderers[4];
            lineRenderer.material.color = Fillcolor;
            DrawTriangles.DrawLine(positions.ToArray(), wide, lineRenderer,Fillcolor);                // 调用函数创建线条
        }

        /// <summary>
        /// 创建半圆形防空区域
        /// </summary>
        /// <param name="origin">中心点</param>
        /// <param name="radius">半径</param>
        /// <param name="angle">张角（默认90度）</param>
        public void CreateHemisphere(int id, Vector3 origin, float radius, Color Fillcolor, int angle = 90)
        {
            LineRenderer[] lineRenderers = new LineRenderer[4];
            List<GameObject> meshList = GetMeshPrefab(lineRenderers, 1);
            meshDic.Add(id, meshList);
            MeshFilter meshFilter = meshList[0].GetComponent<MeshFilter>();
            meshList[0].GetComponent<MeshRenderer>().material.color = Fillcolor;

            DrawTriangles.DrawHemisphere(origin, radius, meshFilter, 90);
        }

        /// <summary>
        /// 获取 count 个 MeshPrefab, 默认 1 个
        /// </summary>
        /// <returns></returns>
        private List<GameObject> GetMeshPrefab(LineRenderer[] lineRenderers, int count = 1)
        {
            List<GameObject> list = new List<GameObject>();
            for (int i = 0; i < count; i++)
            {
                // 创建 MeshPrefab 加入list中
                GameObject meshPrefab = Singleton<GameObjectFactory>.Instance.Instantiate("MeshPrefab");
                meshPrefab.SetActive(true);

                list.Add(meshPrefab);
            }

            for (int i = 0; i < lineRenderers.Length; i++)
            {
                lineRenderers[i] = list[0].transform.GetChild(i).GetComponent<LineRenderer>();
                lineRenderers[i].enabled = true;
            }     

            return list;
        }

        /// <summary>
        /// 关闭 MeshPrefab 显示
        /// </summary>
        /// <param name="id"> 命令ID </param>
        public void SetMeshPrefabFalse(int id)
        {
            List<GameObject> list;
            bool isExist = meshDic.TryGetValue(id, out list);

            if (!isExist)
            {
                return;
            }

            for (int i = 0; i < 5; i++)
            {
                try
                {
                    list[0].transform.GetChild(i).GetComponent<LineRenderer>().enabled = false;
                }
                catch (System.Exception e)
                {

                    Debug.Log(e.Message);
                }

            }
            for (int i = 0; i < list.Count; i++)
            {
                list[i].SetActive(false);       // 重置 MeshPrefab
                list[i].GetComponent<MeshFilter>().mesh = null;//清空Mesh
                list[i] = null;
            }
            meshDic.Remove(id);     // 从字典中删除
        }
    }
}
