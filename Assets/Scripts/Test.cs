// ==========================================
// 描述： 
// 作者： HAK
// 时间： 2018-12-04 08:45:05
// 版本： V 1.0
// ==========================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{

    void Start()
    {
        Timer timer = new Timer(1000);
        timer.AddEventListener(EventDispatchType.TIMER, (aa,bb) =>
        {
            Debug.Log("aa");
        });
        timer.Start();

        //测试场景可以打开用以在开始时清空之前建立的草和树
        //ClearTreeAndDetail();
        //ClearHeightMap();

        float[,] a = new float[3,3]
        {
            {11,12,13 },
            {21,22,23},
            {31,32,33},
        };
        float[,] b = new float[6,3]
        {
            {11,12,13 },
            {21,22,23 },
            {31,32,33 },
            {41,42,43 },
            {51,52,53 },
            {61,62,63 },
        };

        //Debug.Log(b.GetLength(0));
        //Debug.Log(b.GetLength(1));

        float[,] c = a.Concat1(b);

        //for (int i = 0; i < c.GetLength(0); i++)
        //{
        //    for (int j = 0; j < c.GetLength(1); j++)
        //    {
        //        Debug.Log(c[i, j]);
        //    }
        //    Debug.Log("---------------");
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo = Utility.SendRay(LayerMask.GetMask("Terrain"));
            Terrain terrain = hitInfo.collider.GetComponent<Terrain>();
            //TerrainUtility.CreatTree(terrain, hitInfo.point, 200, 50);
            //TerrainUtility.AddDetial(terrain,hitInfo.point,10,0);
            //TerrainUtility.ChangeHeightWithBrush(terrain, hitInfo.point);

            //TerrainUtility.Rise(terrain, hitInfo.point, 1, 10);

            //跨地图测试
            //int[] index = TerrainUtility.GetHeightmapIndex(terrain, hitInfo.point);
            //float[,] temp = TerrainUtility.GetHeightMap(terrain,index[0],index[1],100,100);
            //for (int i = 0; i < temp.GetLength(0); i++)
            //{
            //    for (int j = 0; j < temp.GetLength(1); j++)
            //    {
            //        temp[i, j] += 0.01f;
            //    }
            //}
            //TerrainUtility.SetHeightMap(terrain, temp, index[0], index[1]);
        }
    }

    private void ApplyDetail(Terrain terrain)
    {
        TerrainData terrainData = terrain.terrainData;
    }

    private void ClearTreeAndDetail()
    {
        Terrain[] terrains = Terrain.activeTerrains;
        for (int i = 0; i < terrains.Length; i++)
        {
            terrains[i].terrainData.treeInstances = new TreeInstance[0];
        }

        for (int i = 0; i < terrains.Length; i++)
        {
            terrains[i].terrainData.detailPrototypes = new DetailPrototype[0];
        }
    }

    private void ClearHeightMap()
    {
        Terrain[] terrains = Terrain.activeTerrains;
        for (int i = 0; i < terrains.Length; i++)
        {
            float[,] map = terrains[i].terrainData.GetHeights(0, 0, terrains[i].terrainData.heightmapResolution, terrains[i].terrainData.heightmapResolution);
            for (int k = 0; k < map.GetLength(0); k++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    map[k, j] = 0;
                }
            }
            terrains[i].terrainData.SetHeights(0, 0, map);
        }
    }
}