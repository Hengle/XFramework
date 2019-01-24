﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RCXC.Mathematics;
/** 
* Terrain的HeightMap坐标原点在左下角
*   z
*   ↑
*   0 → x
*/
/// <summary>
/// Terrain工具
/// terrainData.GetHeights和SetHeights的参数都是 值域为[0,1]的比例值
/// </summary>
public static class TerrainUtility
{
    /// <summary>
    /// 用于修改高度的单位高度
    /// </summary>
    private static readonly float deltaHeight;
    /// <summary>
    /// 地形大小
    /// </summary>
    private static readonly Vector3 terrainSize;
    /// <summary>
    /// 高度图分辨率
    /// </summary>
    private static readonly int heightMapRes;
    /// <summary>
    /// 用于记录要修改的Terrain目标数据，修改后统一刷新
    /// </summary>
    private static readonly Dictionary<int, float[,]> brushDic = new Dictionary<int, float[,]>();
    /// <summary>
    /// 用于记录要修改的Terrain目标数据，修改后统一刷新
    /// </summary>
    private static List<Terrain> terrainList = new List<Terrain>();

    /// <summary>
    /// 高度图每一小格的宽高
    /// </summary>
    private static readonly float pieceWidth;
    private static readonly float pieceHeight;

    /// <summary>
    /// 用于地形操作的撤回
    /// </summary>
    private static Stack<TerrainCmdData> terrainDataStack = new Stack<TerrainCmdData>();
    private static Dictionary<Terrain, float[,]> terrainDataDic = new Dictionary<Terrain, float[,]>();

    //private static Dictionary<int, RoadTerrainData> roadTerrainDataDic = new Dictionary<int, RoadTerrainData>();

    /// <summary>
    /// 静态构造函数
    /// </summary>
    static TerrainUtility()
    {
        deltaHeight = 1 / Terrain.activeTerrain.terrainData.size.y;
        terrainSize = Terrain.activeTerrain.terrainData.size;
        heightMapRes = Terrain.activeTerrain.terrainData.heightmapResolution;
        InitBrushs();
        InitPrototype(false, false);

        pieceWidth = terrainSize.x / (heightMapRes - 1);
        pieceHeight = terrainSize.z / (heightMapRes - 1);
    }

    public static void InitPrototype(bool tree = false, bool detail = false, bool texture = false)
    {
        if (tree)
            InitTreePrototype();
        if (detail)
            InitDetailPrototype();
        //if (texture)
        //    InitTextures();
    }

    #region 高度图相关

    /// <summary>
    /// 初始化笔刷
    /// </summary>
    private static void InitBrushs()
    {
        Texture2D[] textures = Resources.LoadAll<Texture2D>("Terrain/Brushs");

        for (int i = 0, length = textures.Length; i < length; i++)
        {
            // 获取图片颜色ARGB信息
            Color[] colors = textures[i].GetPixels();
            // terrainData.GetHeightMap得到的二维数组是[y,x]
            float[,] alphas = new float[textures[i].height, textures[i].width];

            for (int j = 0, length0 = textures[i].height, index = 0; j < length0; j++)
            {
                for (int k = 0, length1 = textures[i].width; k < length1; k++)
                {
                    alphas[j, k] = colors[index].a;
                    index++;
                }
            }
            brushDic.Add(i, alphas);
        }
    }

    /// <summary>
    /// 返回Terrain上某一点的HeightMap索引。
    /// </summary>
    /// <param name="terrain">Terrain</param>
    /// <param name="point">Terrain上的某点</param>
    /// <returns>该点在HeightMap中的位置索引</returns>
    private static Vector2Int GetHeightmapIndex(Terrain terrain, Vector3 point)
    {
        TerrainData tData = terrain.terrainData;
        float width = tData.size.x;
        float length = tData.size.z;

        // 根据相对位置计算索引
        int x = (int)((point.x - terrain.GetPosition().x) / width * tData.heightmapWidth);
        int z = (int)((point.z - terrain.GetPosition().z) / length * tData.heightmapHeight);

        return new Vector2Int(x, z);
    }

    /// <summary>
    /// 返回地图Index对应的世界坐标系位置
    /// </summary>
    /// <param name="terrain"></param>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public static Vector3 GetIndexWorldPoint(Terrain terrain, int x, int z)
    {
        TerrainData data = terrain.terrainData;
        float _x = data.size.x / (data.heightmapWidth - 1) * x;
        float _z = data.size.z / (data.heightmapHeight - 1) * z;

        float _y = GetPointHeight(terrain, new Vector3(_x, 0, _z));
        return new Vector3(_x, _y, _z) + terrain.GetPosition();
    }

    /// <summary>
    /// 返回Terrain的HeightMap的一部分
    /// 场景中有多块地图时不要直接调用terrainData.getheights
    /// 这个方法会解决跨多块地形的问题
    /// </summary>
    /// <param name="terrain">Terrain</param>
    /// <param name="xBase">检索HeightMap时的X索引起点</param>
    /// <param name="yBase">检索HeightMap时的Y索引起点</param>
    /// <param name="width">在X轴上的检索长度</param>
    /// <param name="height">在Y轴上的检索长度</param>
    /// <returns></returns>
    public static float[,] GetHeightMap(Terrain terrain, int xBase = 0, int yBase = 0, int width = 0, int height = 0)
    {
        // 如果后四个均为默认参数，则直接返回当前地形的整个高度图
        if (xBase + yBase + width + height == 0)
        {
            return terrain.terrainData.GetHeights(xBase, yBase, heightMapRes, heightMapRes);
        }

        TerrainData terrainData = terrain.terrainData;
        int differX = xBase + width - (terrainData.heightmapResolution - 1);   // 右溢出量级
        int differY = yBase + height - (terrainData.heightmapResolution - 1);  // 上溢出量级

        float[,] ret;
        if (differX <= 0 && differY <= 0)  // 无溢出
        {
            ret = terrain.terrainData.GetHeights(xBase, yBase, width, height);
        }
        else if (differX > 0 && differY <= 0) // 右边溢出
        {
            ret = terrain.terrainData.GetHeights(xBase, yBase, width - differX, height);
            float[,] right = terrain.Right()?.terrainData.GetHeights(0, yBase, differX, height);
            if (right != null)
                ret = ret.Concat0(right);
        }
        else if (differX <= 0 && differY > 0)  // 上边溢出
        {
            ret = terrain.terrainData.GetHeights(xBase, yBase, width, height - differY);
            float[,] up = terrain.Top()?.terrainData.GetHeights(xBase, 0, width, differY);
            if (up != null)
                ret = ret.Concat1(up);
        }
        else // 上右均溢出
        {
            ret = terrain.terrainData.GetHeights(xBase, yBase, width - differX, height - differY);

            float[,] right = terrain.Right()?.terrainData.GetHeights(0, yBase, differX, height - differY);
            float[,] up = terrain.Top()?.terrainData.GetHeights(xBase, 0, width - differX, differY);
            float[,] upRight = terrain.Right()?.Top()?.terrainData.GetHeights(0, 0, differX, differY);

            if (right != null)
                ret = ret.Concat0(right);
            if (upRight != null)
                ret = ret.Concat1(up.Concat0(upRight));
        }

        return ret;
    }

    /// <summary>
    /// 设置Terrain的HeightMap
    /// 有不只一块地形的场景不要直接调用terrainData.SetHeights
    /// 这个方法会解决跨多块地形的问题
    /// </summary>
    /// <param name="terrain">Terrain</param>
    /// <param name="heights">HeightMap</param>
    /// <param name="xBase">X起点</param>
    /// <param name="yBase">Y起点</param>
    /// <param name="immediate">是否立即刷新地图</param>
    public static void SetHeightMap(Terrain terrain, float[,] heights, int xBase = 0, int yBase = 0, bool immediate = true)
    {
        TerrainData terrainData = terrain.terrainData;
        int length_1 = heights.GetLength(1);
        int length_0 = heights.GetLength(0);

        int differX = xBase + length_1 - (terrainData.heightmapResolution - 1);
        int differY = yBase + length_0 - (terrainData.heightmapResolution - 1);

        if (differX <= 0 && differY <= 0) // 无溢出
        {
            terrain.SetSingleHeightMap(xBase, yBase, heights, immediate);
        }
        else if (differX > 0 && differY <= 0) // 右溢出
        {
            terrain.SetSingleHeightMap(xBase, yBase, heights.GetPart(0, 0, length_0, length_1 - differX + 1), immediate);  // 最后的 +1是为了和右边的地图拼接
            terrain.Right()?.SetSingleHeightMap(0, yBase, heights.GetPart(0, length_1 - differX, length_0, differX), immediate);
        }
        else if (differX <= 0 && differY > 0) // 上溢出
        {
            terrain.SetSingleHeightMap(xBase, yBase, heights.GetPart(0, 0, length_0 - differY + 1, length_1), immediate);  // 最后的 +1是为了和上边的地图拼接
            terrain.Top()?.SetSingleHeightMap(xBase, 0, heights.GetPart(length_0 - differY, 0, differY, length_1), immediate);
        }
        else  // 右上均溢出
        {
            terrain.SetSingleHeightMap(xBase, yBase, heights.GetPart(0, 0, length_0 - differY + 1, length_1 - differX + 1), immediate);  // 最后的 +1是为了和上边及右边的地图拼接
            terrain.Right()?.SetSingleHeightMap(0, yBase, heights.GetPart(0, length_1 - differX, length_0 - differY + 1, differX), immediate);
            terrain.Top()?.SetSingleHeightMap(xBase, 0, heights.GetPart(length_0 - differY, 0, differY, length_1 - differX + 1), immediate);
            terrain.Top()?.Right()?.SetSingleHeightMap(0, 0, heights.GetPart(length_0 - differY, length_1 - differX, differY, differX), immediate);
        }
    }

    /// <summary>
    /// 设置单块地图的高度图
    /// </summary>
    /// <param name="immediate"></param>
    private static void SetSingleHeightMap(this Terrain terrain, int xBase, int yBase, float[,] heights, bool immediate = true)
    {
        if (!terrainDataDic.ContainsKey(terrain))
            terrainDataDic.Add(terrain, GetHeightMap(terrain));


        if (immediate)
            terrain.terrainData.SetHeights(xBase, yBase, heights);
        else
            SetSingleHeightMapDelayLOD(terrain, xBase, yBase, heights);
    }

    /// <summary>
    /// 刷新地图的LOD
    /// </summary>
    public static void Refresh()
    {
        foreach (var item in terrainList)
        {
            item.ApplyDelayedHeightmapModification();
        }
        terrainList.Clear();
    }

    /// <summary>
    /// 快速设置高度图，之后调用Refresh设置LOD
    /// </summary>
    /// <param name="terrain"></param>
    /// <param name="xBase"></param>
    /// <param name="yBase"></param>
    /// <param name="heights"></param>
    private static void SetSingleHeightMapDelayLOD(Terrain terrain, int xBase, int yBase, float[,] heights)
    {
        if (!terrainList.Contains(terrain))
        {
            terrainList.Add(terrain);
        }

        terrain.terrainData.SetHeightsDelayLOD(xBase, yBase, heights);
    }

    /// <summary>
    /// 初始化地形高度图编辑所需要的参数
    /// 后四个参数需要在调用前定义
    /// 编辑高度时所需要用到的参数后期打算用一个结构体封装
    /// </summary>
    /// <param name="center">目标中心</param>
    /// <param name="radius">半径</param>
    /// <param name="mapIndex">起始修改点在高度图上的索引</param>
    /// <param name="heightMap">要修改的高度二维数组</param>
    /// <param name="mapRadius">修改半径对应的索引半径</param>
    /// <param name="limit">限制高度</param>
    /// <returns></returns>
    private static Terrain InitHMArg(Vector3 center, float radius, out HMArg arg)
    {
        Vector3 leftDown = new Vector3(center.x - radius, 0, center.z - radius);
        // 左下方Terrain
        Terrain terrain = Utility.SendRayDown(center, LayerMask.GetMask("Terrain")).collider?.GetComponent<Terrain>();
        // 左下至少有一个方向没有Terrain
        if (terrain != null)
        {
            // 获取相关参数
            arg.mapRadiusX = (int)(heightMapRes / terrainSize.x * radius);
            arg.mapRadiusZ = (int)(heightMapRes / terrainSize.z * radius);
            arg.mapRadiusX = arg.mapRadiusX < 1 ? 1 : arg.mapRadiusX;
            arg.mapRadiusZ = arg.mapRadiusZ < 1 ? 1 : arg.mapRadiusZ;

            arg.centerMapIndex = GetHeightmapIndex(terrain, center);
            arg.startMapIndex = new Vector2Int(arg.centerMapIndex.x - arg.mapRadiusX, arg.centerMapIndex.y - arg.mapRadiusZ);

            int width = 2 * arg.mapRadiusX, height = 2 * arg.mapRadiusZ;
            if (arg.startMapIndex.x < 0)
            {
                width += arg.startMapIndex.x;
                arg.startMapIndex.x = 0;
            }
            if (arg.startMapIndex.y < 0)
            {
                height += arg.startMapIndex.y;
                arg.startMapIndex.y = 0;
            }
            arg.startMapIndex.y = arg.startMapIndex.y < 0 ? 0 : arg.startMapIndex.y;

            arg.heightMap = GetHeightMap(terrain, arg.startMapIndex.x, arg.startMapIndex.y, width, height);
            arg.limit = 0/*heightMap[mapRadius, mapRadius]*/;
        }
        else
        {
            terrain = Utility.SendRayDown(center, LayerMask.GetMask("Terrain")).collider?.GetComponent<Terrain>();
            if (terrain != null)
            {
                arg.centerMapIndex = new Vector2Int(0, 0);
            }
            arg = default(HMArg);
        }
        return terrain;
    }

    /// <summary>
    /// 改变地形高度
    /// </summary>
    /// <param name="center"></param>
    /// <param name="radius"></param>
    /// <param name="opacity"></param>
    /// <param name="amass"></param>
    public static void ChangeHeight(Vector3 center, float radius, float opacity, bool isRise = true, bool amass = true)
    {
        HMArg arg;
        Terrain terrain = InitHMArg(center, radius, out arg);
        if (terrain == null) return;

        if (!isRise) opacity = -opacity;

        // 修改高度图
        for (int i = 0, length_0 = arg.heightMap.GetLength(0); i < length_0; i++)
        {
            for (int j = 0, length_1 = arg.heightMap.GetLength(1); j < length_1; j++)
            {
                // 限制范围为一个圆
                float rPow = Mathf.Pow(i + arg.mapRadiusZ - (arg.centerMapIndex.y - arg.startMapIndex.y) - arg.mapRadiusZ, 2) + Mathf.Pow(j + arg.mapRadiusX - (arg.centerMapIndex.x - arg.startMapIndex.x) - arg.mapRadiusX, 2);
                if (rPow > arg.mapRadiusX * arg.mapRadiusZ)
                    continue;

                float differ = 1 - rPow / (arg.mapRadiusX * arg.mapRadiusZ);
                if (amass)
                {
                    arg.heightMap[i, j] += differ * deltaHeight * opacity;
                }
                else if (isRise)
                {
                    arg.heightMap[i, j] = arg.heightMap[i, j] >= arg.limit ? arg.heightMap[i, j] : arg.heightMap[i, j] + differ * deltaHeight * opacity;
                }
                else
                {
                    arg.heightMap[i, j] = arg.heightMap[i, j] <= arg.limit ? arg.heightMap[i, j] : arg.heightMap[i, j] + differ * deltaHeight * opacity;
                }
            }
        }
        // 重新设置高度图
        SetHeightMap(terrain, arg.heightMap, arg.startMapIndex.x, arg.startMapIndex.y, false);
    }

    /// <summary>
    /// 通过自定义笔刷编辑地形
    /// </summary>
    /// <param name="terrain"></param>
    public static async void ChangeHeightWithBrush(Vector3 center, float radius, float opacity, int brushIndex = 0, bool isRise = true)
    {
        HMArg arg;
        Terrain terrain = InitHMArg(center, radius, out arg);
        if (terrain == null) return;

        // 是否反转透明度
        if (!isRise) opacity = -opacity;

        //修改高度图
        //float[,] deltaMap = await Utility.BilinearInterp(brushDic[brushIndex], 2 * mapRadius, 2 * mapRadius);
        float[,] deltaMap = await Math2d.ZoomBilinearInterpAsync(brushDic[brushIndex], 2 * arg.mapRadiusX, 2 * arg.mapRadiusX);

        for (int i = 0; i < 2 * arg.mapRadiusX; i++)
        {
            for (int j = 0; j < 2 * arg.mapRadiusX; j++)
            {
                arg.heightMap[i, j] += deltaMap[i, j] * deltaHeight * opacity;
            }
        }

        // 重新设置高度图
        SetHeightMap(terrain, arg.heightMap, arg.startMapIndex.x, arg.startMapIndex.y);
    }

    /// <summary>
    /// 平滑地形
    /// </summary>
    /// <param name="center"></param>
    /// <param name="radius"></param>
    /// <param name="dev"></param>
    /// <param name="level"></param>
    public static void Smooth(Vector3 center, float radius, float dev, int level = 3)
    {
        center.x -= terrainSize.x / (heightMapRes - 1) * level;
        center.z -= terrainSize.z / (heightMapRes - 1) * level;
        radius += terrainSize.x / (heightMapRes - 1) * level;
        HMArg arg;
        Terrain terrain = InitHMArg(center, radius, out arg);
        if (terrain == null) return;
        Math2d.GaussianBlur(arg.heightMap, dev, level, false);
        SetHeightMap(terrain, arg.heightMap, arg.startMapIndex.x, arg.startMapIndex.y, false);
    }

    /// <summary>
    /// 批量平滑操作
    /// </summary>
    public static void BatchSmooth(Vector3[] centers, float radius, float dev, int level = 1)
    {
        float differx = terrainSize.x / (heightMapRes - 1) * level;
        float differz = terrainSize.z / (heightMapRes - 1) * level;
        int arrayLength = centers.Length;
        for (int i = 0; i < arrayLength; i++)
        {
            centers[i].x -= differx;
            centers[i].z -= differz;
        }
        Terrain[] terrains = new Terrain[arrayLength];

        HMArg[] args = new HMArg[arrayLength];
        LoomA.Initialize();
        for (int i = 0; i < arrayLength; i++)
        {
            terrains[i] = InitHMArg(centers[i], radius, out args[i]);
            BatchSmooth(terrains[i], args[i].heightMap, args[i].startMapIndex, dev, level);
            //BatchSmoothAsync(terrains[i], heightMaps[i], mapIndexs[i], dev, level);
        }
    }

    private static void BatchSmooth(Terrain terrain, float[,] heightMap, Vector2Int mapIndex, float dev, int level = 1)
    {
        if (terrain != null)
        {
            LoomA.RunAsync(() =>
            {
                for (int i = 0; i < 10; i++)
                {
                    Math2d.GaussianBlur(heightMap, dev, level);
                }
                LoomA.QueueOnMainThread(() =>
                {
                    SetHeightMap(terrain, heightMap, mapIndex.x, mapIndex.y, false);
                });
            });
        }
    }

    /// <summary>
    /// 异步效率测试代码，后期删掉
    /// </summary>
    /// <param name="terrain"></param>
    /// <param name="heightMap"></param>
    /// <param name="mapIndex"></param>
    /// <param name="dev"></param>
    /// <param name="level"></param>
    private async static void BatchSmoothAsync(Terrain terrain, float[,] heightMap, Vector2Int mapIndex, float dev, int level = 1)
    {
        if (terrain != null)
        {
            await Task.Run(() =>
            {
                Math2d.GaussianBlur(heightMap, dev, level);
                //Debug.Log(Test.TimeStr);
            });

            //Debug.Log(System.DateTime.Now.Millisecond + " :: " + Test.time);
            SetHeightMap(terrain, heightMap, mapIndex.x, mapIndex.y, false);
        }
    }

    /// <summary>
    /// 压平Terrain并提升到指定高度。
    /// </summary>
    /// <param name="terrain">Terrain</param>
    /// <param name="height">高度</param>
    public static void Flatten(Terrain terrain, float height)
    {
        float scaledHeight = height * deltaHeight;

        float[,] heights = new float[heightMapRes, heightMapRes];
        for (int i = 0; i < heightMapRes; i++)
        {
            for (int j = 0; j < heightMapRes; j++)
            {
                heights[i, j] = scaledHeight;
            }
        }

        terrain.terrainData.SetHeights(0, 0, heights);
    }

    #endregion

    #region 道路系统

    ///// <summary>
    ///// 压平路面
    ///// </summary>
    ///// <param name="point_0">线段起点</param>
    ///// <param name="point_1">线段末点</param>
    ///// <param name="halfLength"></param>
    //public static void FlattenRoad(Vector3 pos_0, Vector3 pos_1, Vector3 pos_2, Vector3 pos_3, RoadsOrBridges road)
    //{
    //    Vector3 point_0 = (pos_0 + pos_1) / 2;
    //    Vector3 point_1 = (pos_2 + pos_3) / 2;

    //    float maxX = Mathf.Max(pos_0.x, pos_1.x, pos_2.x, pos_3.x);
    //    float minX = Mathf.Min(pos_0.x, pos_1.x, pos_2.x, pos_3.x);
    //    float maxZ = Mathf.Max(pos_0.z, pos_1.z, pos_2.z, pos_3.z);
    //    float minZ = Mathf.Min(pos_0.z, pos_1.z, pos_2.z, pos_3.z);
    //    Vector3 startPos = new Vector3(minX, 0, minZ);
    //    Terrain terrain = GetTerrain(startPos);

    //    int mapWidth = (int)((maxX - minX) / pieceWidth);
    //    int mapHeight = (int)((maxZ - minZ) / pieceHeight);

    //    if (terrain != null)
    //    {
    //        // 构造坐标系
    //        GameCoordinate localCor = new GameCoordinate(pos_0, pos_1 - pos_0);
    //        Vector3 localPos_0 = localCor.World2Loacal3(pos_0);
    //        Vector3 localPos_1 = localCor.World2Loacal3(pos_1);
    //        Vector3 localPos_2 = localCor.World2Loacal3(pos_2);
    //        Vector3 localPos_3 = localCor.World2Loacal3(pos_3);
    //        float maxLX = Mathf.Max(localPos_0.x, localPos_1.x, localPos_2.x, localPos_3.x);
    //        float minLX = Mathf.Min(localPos_0.x, localPos_1.x, localPos_2.x, localPos_3.x);
    //        float maxLZ = Mathf.Max(localPos_0.z, localPos_1.z, localPos_2.z, localPos_3.z);
    //        float minLZ = Mathf.Min(localPos_0.z, localPos_1.z, localPos_2.z, localPos_3.z)/* - localPos_3.y / 2*/;

    //        Vector2Int mapIndex = GetHeightmapIndex(terrain, startPos);
    //        startPos = GetPositionInWorild(terrain, mapIndex.x, mapIndex.y);
    //        startPos.y = 0;

    //        float[,] heightMap = GetHeightMap(terrain, mapIndex.x, mapIndex.y, mapWidth + 1, mapHeight + 1);

    //        for (int i = 0; i < heightMap.GetLength(0); i++)
    //        {
    //            for (int j = 0; j < heightMap.GetLength(1); j++)
    //            {
    //                Vector2Int newMapIndex = new Vector2Int(mapIndex.x + j, mapIndex.y + i);
    //                if (newMapIndex.x > (heightMapRes - 1))
    //                {
    //                    terrain = terrain.Right() ?? terrain;
    //                    newMapIndex.x -= heightMapRes;
    //                }
    //                if (newMapIndex.y > (heightMapRes - 1))
    //                {
    //                    terrain = terrain.Top() ?? terrain;
    //                    newMapIndex.y -= heightMapRes;
    //                }
    //                Vector3 worldPos = GetPositionInWorild(terrain, newMapIndex.x, newMapIndex.y);
    //                Vector3 loacalPos = localCor.World2Loacal3(worldPos);
    //                if (loacalPos.x < minLX || loacalPos.x > maxLX || loacalPos.z < minLZ || loacalPos.z > maxLZ)
    //                {
    //                    //continue;
    //                }

    //                Vector3 pos = startPos + new Vector3(j * pieceWidth, 0, i * pieceHeight);
    //                Vector3 intersection;
    //                // 判断相交
    //                if (Math3d.LineLineIntersection(out intersection, point_0, point_1 - point_0, pos, pos_3 - pos_2))
    //                {
    //                    // 存储路面撤回相关的数据
    //                    if (!road.terrains.Contains(terrain))
    //                    {
    //                        float[,] oldHM = GetHeightMap(terrain);
    //                        road.terrains.Add(terrain);
    //                        road.oldHeigtMaps.Add(oldHM);
    //                        road.isChanges.Add(new bool[heightMapRes, heightMapRes]);
    //                    }

    //                    road.isChanges[road.terrains.IndexOf(terrain)][newMapIndex.y, newMapIndex.x] = true;

    //                    // 修改HeightMap
    //                    heightMap[i, j] = GetPointHeight(GetTerrain(intersection), intersection) * deltaHeight;
    //                }
    //            }
    //        }

    //        SetHeightMap(terrain, heightMap, mapIndex.x, mapIndex.y, false);
    //    }
    //}

    ///// <summary>
    ///// 删除路的时候调用
    ///// </summary>
    ///// <param name="roadId"></param>
    //public static void RecoverRoadTerrainData(RoadsOrBridges road)
    //{
    //    if (road != null)
    //    {
    //        for (int i = 0, lenghth = road.terrains.Count; i < lenghth; i++)
    //        {
    //            bool[,] isChange = road.isChanges[i];
    //            float[,] HM = GetHeightMap(road.terrains[i]);
    //            float[,] oldHM = road.oldHeigtMaps[i];
    //            for (int j = 0; j < HM.GetLength(0); j++)
    //            {
    //                for (int k = 0; k < HM.GetLength(1); k++)
    //                {
    //                    if (isChange[j, k])
    //                        HM[j, k] = oldHM[j, k];
    //                }
    //            }
    //            SetSingleHeightMapDelayLOD(road.terrains[i], 0, 0, HM);
    //        }
    //        Refresh();
    //    }
    //}

    #endregion

    #region 树木

    /// <summary>
    /// 初始化树木原型组
    /// </summary>
    private static void InitTreePrototype()
    {
        GameObject[] objs = Resources.LoadAll<GameObject>("Terrain/SpeedTree");
        TreePrototype[] trees = new TreePrototype[objs.Length];
        for (int i = 0, length = objs.Length; i < length; i++)
        {
            trees[i] = new TreePrototype();
            trees[i].prefab = objs[i];
        }
        Terrain[] terrains = Terrain.activeTerrains;
        for (int i = 0, length = terrains.Length; i < length; i++)
        {
            terrains[i].terrainData.treePrototypes = terrains[i].terrainData.treePrototypes.Concat(trees).ToArray();
        }
    }

    /// <summary>
    /// 创建树木
    /// </summary>
    /// <param name="terrain"></param>
    /// <param name="pos"></param>
    public static void CreatTree(Terrain terrain, Vector3 pos, int count, int radius, int index = 0)
    {
        TerrainData terrainData = terrain.terrainData;
        Vector3 relativePosition;
        Vector3 position;

        for (int i = 0; i < count; i++)
        {
            // 获取世界坐标系的位置和相对位置
            position = pos + new Vector3(UnityEngine.Random.Range(-radius, radius), 0, UnityEngine.Random.Range(-radius, radius));
            relativePosition = position - terrain.GetPosition();

            if (Mathf.Pow(pos.x - position.x, 2) + Mathf.Pow(pos.z - position.z, 2) > radius * radius)
            {
                i--; // 没有创建的数不计入
                continue;
            }

            // 设置新添加的树的参数
            TreeInstance instance = new TreeInstance();
            instance.prototypeIndex = index;
            instance.color = Color.white;
            instance.lightmapColor = Color.white;
            instance.widthScale = 1;
            instance.heightScale = 1;

            Vector3 p = new Vector3(relativePosition.x / terrainData.size.x, 0, relativePosition.z / terrainData.size.z);
            if (p.x > 1 || p.z > 1)
            {
                if (p.x > 1)
                    p.x = p.x - 1;
                if (p.z > 1)
                    p.z = p.z - 1;
                instance.position = p;
                GetTerrain(position)?.AddTreeInstance(instance);
            }
            else if (p.x < 0 || p.z < 0)
            {
                if (p.x < 0)
                    p.x = p.x + 1;
                if (p.z < 0)
                    p.z = p.z + 1;
                instance.position = p;
                GetTerrain(position)?.AddTreeInstance(instance);
            }
            else
            {
                instance.position = p;
                terrain.AddTreeInstance(instance);
            }
        }
    }

    /// <summary>
    /// 移除地形上的树，没有做多地图的处理
    /// </summary>
    /// <param name="terrain">目标地形</param>
    /// <param name="center">中心点</param>
    /// <param name="radius">半径</param>
    /// <param name="index">树模板的索引</param> 
    public static void RemoveTree(Terrain terrain, Vector3 center, float radius, int index = 0)
    {
        center -= terrain.GetPosition();     // 转为相对位置
        Vector2 v2 = new Vector2(center.x, center.z);
        v2.x /= Terrain.activeTerrain.terrainData.size.x;
        v2.y /= Terrain.activeTerrain.terrainData.size.z;

        terrain.Invoke("RemoveTrees", v2, radius / Terrain.activeTerrain.terrainData.size.x, index);
    }

    #endregion

    #region 细节纹理 草

    /// <summary>
    /// 初始化细节原型组
    /// </summary>
    private static void InitDetailPrototype()
    {
        Texture2D[] textures = Resources.LoadAll<Texture2D>("Terrain/Details");
        DetailPrototype[] details = new DetailPrototype[textures.Length];

        for (int i = 0, length = details.Length; i < length; i++)
        {
            details[i] = new DetailPrototype();
            details[i].prototypeTexture = textures[i];
            details[i].minWidth = 1f;
            details[i].maxWidth = 2f;
            details[i].maxHeight = 0.2f;
            details[i].maxHeight = 0.8f;
            details[i].noiseSpread = 1f;
            details[i].healthyColor = Color.green;
            details[i].dryColor = Color.green;
            details[i].renderMode = DetailRenderMode.GrassBillboard;
        }

        Terrain[] terrains = Terrain.activeTerrains;
        for (int i = 0, length = terrains.Length; i < length; i++)
        {
            terrains[i].terrainData.detailPrototypes = terrains[i].terrainData.detailPrototypes.Concat(details).ToArray();
        }
    }

    /// <summary>
    /// 返回Terrain上某一点的DetialMap索引。
    /// </summary>
    /// <param name="terrain">Terrain</param>
    /// <param name="point">Terrain上的某点</param>
    /// <returns>该点在DetialMap中的位置索引</returns>
    private static Vector2Int GetDetialMapIndex(Terrain terrain, Vector3 point)
    {
        TerrainData tData = terrain.terrainData;
        float width = tData.size.x;
        float length = tData.size.z;

        // 根据相对位置计算索引
        int x = (int)((point.x - terrain.GetPosition().x) / width * tData.detailWidth);
        int z = (int)((point.z - terrain.GetPosition().z) / length * tData.detailHeight);

        return new Vector2Int(x, z);
    }

    /// <summary>
    /// 添加细节
    /// </summary>
    /// <param name="terrain">目标地形</param>
    /// <param name="center">目标中心点</param>
    /// <param name="radius">半径</param>
    /// <param name="layer">层级</param>
    public static void AddDetial(Terrain terrain, Vector3 center, float radius, int count, int layer = 0)
    {
        //SetDetail(terrain, center, radius, layer, count);
        NewSetDetail(center, radius, layer, count);
    }

    /// <summary>
    /// 移除细节
    /// </summary>
    /// <param name="terrain">目标地形</param>
    /// <param name="center">目标中心点</param>
    /// <param name="radius">半径</param>
    /// <param name="layer">层级</param>
    public static void RemoveDetial(Terrain terrain, Vector3 point, float radius, int layer = 0)
    {
        //SetDetail(terrain, point, radius, layer, 0);
        NewSetDetail(point, radius, layer, 0);
    }

    /// <summary>
    /// 不考虑跨地形的细节修改
    /// </summary>
    public static void SetDetail(Terrain terrain, Vector3 point, float radius, int layer, int count)
    {
        TerrainData terrainData = terrain.terrainData;

        // 将位置转为细节图索引，半径转为索引半径
        Vector2Int index = GetDetialMapIndex(terrain, point);
        int mapRadius = (int)(radius / terrainData.size.x * terrainData.detailResolution);

        int[,] map = terrainData.GetDetailLayer(index.x - mapRadius, index.y - mapRadius, 2 * mapRadius, 2 * mapRadius, layer);

        for (int i = 0, length_0 = map.GetLength(0); i < length_0; i++)
        {
            for (int j = 0, length_1 = map.GetLength(1); j < length_1; j++)
            {
                // 限定圆
                if ((i - mapRadius) * (i - mapRadius) + (j - mapRadius) * (j - mapRadius) > mapRadius * mapRadius)
                    continue;
                map[i, j] = count;
            }
        }

        // 设置细节图层
        terrainData.SetDetailLayer(index.x - mapRadius, index.y - mapRadius, layer, map);
    }

    /// <summary>
    /// 获取细节数据
    /// </summary>
    private static int[,] GetDetailLayer(Terrain terrain, int xBase = 0, int yBase = 0, int width = 0, int height = 0, int layer = 0)
    {
        if (xBase + yBase + width + height == 0)
        {
            width = height = terrain.terrainData.detailResolution;
            return terrain.terrainData.GetDetailLayer(xBase, yBase, width, height, layer);
        }

        TerrainData terrainData = terrain.terrainData;
        int differX = xBase + width - terrainData.detailResolution;
        int differY = yBase + height - terrainData.detailResolution;
        int[,] ret;
        if (differX <= 0 && differY <= 0)  // 无溢出
        {
            ret = terrain.terrainData.GetDetailLayer(xBase, yBase, width, height, layer);
        }
        else if (differX > 0 && differY <= 0) // 右边溢出
        {
            ret = terrain.terrainData.GetDetailLayer(xBase, yBase, width - differX, height, layer);
            int[,] right = terrain.Right()?.terrainData.GetDetailLayer(0, yBase, differX, height, layer);
            if (right != null)
                ret = ret.Concat0(right);
        }
        else if (differX <= 0 && differY > 0)  // 上边溢出
        {
            ret = terrain.terrainData.GetDetailLayer(xBase, yBase, width, height - differY, layer);
            int[,] up = terrain.Top()?.terrainData.GetDetailLayer(xBase, 0, width, differY, layer);
            if (up != null)
                ret = ret.Concat1(up);
        }
        else // 上右均溢出
        {
            ret = terrain.terrainData.GetDetailLayer(xBase, yBase, width - differX, height - differY, layer);
            int[,] right = terrain.Right()?.terrainData.GetDetailLayer(0, yBase, differX, height - differY, layer);
            int[,] up = terrain.Top()?.terrainData.GetDetailLayer(xBase, 0, width - differX, differY, layer);
            int[,] upRight = terrain.Right()?.Top()?.terrainData.GetDetailLayer(0, 0, differX, differY, layer);

            if (right != null)
                ret = ret.Concat0(right);
            if (upRight != null)
                ret = ret.Concat1(up.Concat0(upRight));
        }

        return ret;
    }

    /// <summary>
    /// 设置细节数据
    /// </summary>
    /// <param name="terrain"></param>
    /// <param name="detailMap"></param>
    /// <param name="xBase"></param>
    /// <param name="yBase"></param>
    /// <param name="layer"></param>
    private static void SetDetailLayer(Terrain terrain, int[,] detailMap, int xBase, int yBase, int layer)
    {
        TerrainData terrainData = terrain.terrainData;
        int length_1 = detailMap.GetLength(1);
        int length_0 = detailMap.GetLength(0);

        int differX = xBase + length_1 - (terrainData.detailResolution);
        int differY = yBase + length_0 - (terrainData.detailResolution);

        if (differX <= 0 && differY <= 0) // 无溢出
        {
            terrain.terrainData.SetDetailLayer(xBase, yBase, layer, detailMap);
        }
        else if (differX > 0 && differY <= 0) // 右溢出
        {
            terrain.terrainData.SetDetailLayer(xBase, yBase, layer, detailMap.GetPart(0, 0, length_0, length_1 - differX));
            terrain.Right()?.terrainData.SetDetailLayer(0, yBase, layer, detailMap.GetPart(0, length_1 - differX, length_0, differX));
        }
        else if (differX <= 0 && differY > 0) // 上溢出
        {
            terrain.terrainData.SetDetailLayer(xBase, yBase, layer, detailMap.GetPart(0, 0, length_0 - differY, length_1));
            terrain.Top()?.terrainData.SetDetailLayer(xBase, 0, layer, detailMap.GetPart(length_0 - differY, 0, differY, length_1));
        }
        else  // 右上均溢出
        {
            terrain.terrainData.SetDetailLayer(xBase, yBase, layer, detailMap.GetPart(0, 0, length_0 - differY, length_1 - differX));
            terrain.Right()?.terrainData.SetDetailLayer(0, yBase, layer, detailMap.GetPart(0, length_1 - differX, length_0 - differY, differX));
            terrain.Top()?.terrainData.SetDetailLayer(xBase, 0, layer, detailMap.GetPart(length_0 - differY, 0, differY, length_1 - differX));
            terrain.Top()?.Right().terrainData.SetDetailLayer(0, 0, layer, detailMap.GetPart(length_0 - differY, length_1 - differX, differY, differX));
        }
    }

    /// <summary>
    /// 修改细节数据
    /// </summary>
    /// <param name="detailMap"></param>
    /// <param name="count"></param>
    private static void ChangeDetailMap(int[,] detailMap, int count)
    {
        int mapRadius = detailMap.GetLength(0) / 2;
        // 修改数据
        for (int i = 0, length_0 = detailMap.GetLength(0); i < length_0; i++)
        {
            for (int j = 0, length_1 = detailMap.GetLength(1); j < length_1; j++)
            {
                // 限定圆
                if ((i - mapRadius) * (i - mapRadius) + (j - mapRadius) * (j - mapRadius) > mapRadius * mapRadius)
                    continue;
                detailMap[i, j] = count;
            }
        }
    }

    /// <summary>
    /// 可跨多块地形的细节修改
    /// </summary>
    /// <param name="terrain"></param>
    /// <param name="center"></param>
    /// <param name="radius"></param>
    /// <param name="layer"></param>
    /// <param name="count"></param>
    public static void NewSetDetail(Vector3 center, float radius, int layer, int count)
    {
        Vector3 leftDown = new Vector3(center.x - radius, 0, center.z - radius);
        Terrain terrain = Utility.SendRayDown(leftDown, LayerMask.GetMask("Terrain")).collider?.GetComponent<Terrain>();
        if (terrain != null)
        {
            // 获取数据
            TerrainData terrainData = terrain.terrainData;
            int mapRadius = (int)(radius / terrainData.size.x * terrainData.detailResolution);
            Vector2Int mapIndex = GetDetialMapIndex(terrain, leftDown);
            int[,] detailMap = GetDetailLayer(terrain, mapIndex.x, mapIndex.y, 2 * mapRadius, 2 * mapRadius, layer);

            // 修改数据
            ChangeDetailMap(detailMap, count);

            // 设置数据
            SetDetailLayer(terrain, detailMap, mapIndex.x, mapIndex.y, layer);
        }
    }

    #endregion

    #region 贴图

    /// <summary>
    /// 初始化贴图原型
    /// </summary>
    private static void InitTextures()
    {
#if UNITY_2018
        Texture2D[] textures = Resources.LoadAll<Texture2D>("Terrain/Textures");
        TerrainLayer[] splats = new TerrainLayer[textures.Length / 2];

        for (int i = 0, length = splats.Length; i < length; i++)
        {
            TerrainLayer splat = new TerrainLayer
            {
                diffuseTexture = textures[2 * i],
                normalMapTexture = textures[2 * i + 1]
            };
            splats[i] = splat;
        }

        Terrain[] terrains = Terrain.activeTerrains;
        for (int i = 0, length = terrains.Length; i < length; i++)
        {
            terrains[i].terrainData.terrainLayers = terrains[i].terrainData.terrainLayers.Concat(splats).ToArray();
        }
#else

        Texture2D[] textures = Resources.LoadAll<Texture2D>("Terrain/Textures");
        SplatPrototype[] splats = new SplatPrototype[textures.Length / 2];

        for (int i = 0, length = splats.Length; i < length; i++)
        {
            SplatPrototype splat = new SplatPrototype
            {
                texture = textures[2 * i],
                normalMap = textures[2 * i + 1]
            };
            splats[i] = splat;
        }

        Terrain[] terrains = Terrain.activeTerrains;
        for (int i = 0, length = terrains.Length; i < length; i++)
        {
            terrains[i].terrainData.splatPrototypes = terrains[i].terrainData.splatPrototypes.Concat(splats).ToArray();
        }
#endif
    }

    /// <summary>
    /// 返回Terrain上某一点的AlphalMap索引。
    /// </summary>
    /// <param name="terrain">Terrain</param>
    /// <param name="point">Terrain上的某点</param>
    /// <returns>该点在DetialMap中的位置索引</returns>
    private static int[] GetAlphaMapIndex(Terrain terrain, Vector3 point)
    {
        TerrainData tData = terrain.terrainData;
        float width = tData.size.x;
        float length = tData.size.z;

        // 根据相对位置计算索引
        int x = (int)((point.x - terrain.GetPosition().x) / width * tData.alphamapWidth);
        int z = (int)((point.z - terrain.GetPosition().z) / length * tData.alphamapHeight);

        return new int[2] { x, z };
    }

    /// <summary>
    /// 设置贴图
    /// </summary>
    /// <param name="point"></param>
    /// <param name="index"></param>
    public static void SetTexture(Vector3 point, int index)
    {
        Terrain terrain = Utility.SendRayDown(point, LayerMask.GetMask("Terrain")).collider?.GetComponent<Terrain>();

        if (terrain != null)
        {
            int[] mapIndex = GetAlphaMapIndex(terrain, point);
            float[,,] map = terrain.terrainData.GetAlphamaps(mapIndex[0], mapIndex[1], 1, 1);
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    map[i, j, index] = 0.5f;
                    map[i, j, 0] = 0.5f;
                }
            }
            terrain.terrainData.SetAlphamaps(mapIndex[0], mapIndex[1], map);
        }
    }

    #endregion

    #region 修改与恢复

    /// <summary>
    /// 添加一个记录点
    /// </summary>
    /// <param name="terrain"></param>
    public static void AddOldData()
    {
        terrainDataStack.Push(new TerrainCmdData(terrainDataDic));
        terrainDataDic.Clear();
    }

    /// <summary>
    /// 恢复一次数据
    /// </summary>
    /// <param name="terrain"></param>
    public static void Recover()
    {
        if (!(terrainDataStack.Count > 0))
            return;
        TerrainCmdData terrainCmdData = terrainDataStack.Pop();
        terrainCmdData.Recover();
    }

    #endregion

    #region  工具

    /// <summary>
    /// 获取当前位置所对应的地图块
    /// </summary>
    public static Terrain GetTerrain(Vector3 pos)
    {
        RaycastHit hitInfo;
        Physics.Raycast(pos + Vector3.up * 10000, Vector3.down, out hitInfo, float.MaxValue, LayerMask.GetMask("Terrain"));
        return hitInfo.collider?.GetComponent<Terrain>();
    }

    /// <summary>
    /// 获取当前位置在地图上的对应位置
    /// </summary>
    /// <returns></returns>
    public static Vector3 GetPositionOnTerrain(Vector3 pos)
    {
        return new Vector3(pos.x, GetTerrain(pos).SampleHeight(pos), pos.z);
    }

    /// <summary>
    /// 获取当前Index对应世界空间的坐标
    /// </summary>
    /// <param name="terrain"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static Vector3 GetPositionInWorild(Terrain terrain, int x, int y)
    {
        Vector3 pos = terrain.GetPosition();
        pos.x += x * pieceWidth;
        pos.z += y * pieceHeight;
        pos.y = terrain.SampleHeight(pos);
        return pos;
    }

    /// <summary>
    /// 返回GameObject在Terrain上的相对（于Terrain的）位置。
    /// </summary>
    /// <param name="terrain">Terrain</param>
    /// <param name="go">GameObject</param>
    /// <returns>相对位置</returns>
    public static Vector3 GetRelativePosition(Terrain terrain, GameObject go)
    {
        return go.transform.position - terrain.GetPosition();
    }

    /// <summary>
    /// 返回Terrain上指定点在世界坐标系下的高度。
    /// </summary>
    /// <param name="terrain">Terrain</param>
    /// <param name="point">Terrain上的某点</param>
    /// <param name="vertex">true: 获取最近顶点高度  false: 获取实际高度</param>
    /// <returns>点在世界坐标系下的高度</returns>
    public static float GetPointHeight(Terrain terrain, Vector3 point, bool vertex = false)
    {
        // 对于水平面上的点来说，vertex参数没有影响
        if (vertex)
        {
            // GetHeight得到的是离点最近的顶点的高度
            Vector2Int index = GetHeightmapIndex(terrain, point);
            return terrain.terrainData.GetHeight(index.x, index.y);
        }
        else
        {
            // SampleHeight得到的是点在斜面上的实际高度
            return terrain.SampleHeight(point);
        }
    }

    #endregion

    struct TerrainCmdData
    {
        public Dictionary<Terrain, float[,]> terrainDataDic;

        public TerrainCmdData(Dictionary<Terrain, float[,]> dic)
        {
            terrainDataDic = new Dictionary<Terrain, float[,]>();
            foreach (var item in dic)
            {
                terrainDataDic.Add(item.Key, item.Value);
            }
        }

        public void Recover()
        {
            foreach (var item in terrainDataDic)
            {
                item.Key.terrainData.SetHeights(0, 0, item.Value);
            }
        }
    }

    /// <summary>
    /// 高度图修改的初始化参数
    /// </summary>
    struct HMArg
    {
        public int mapRadiusX;
        public int mapRadiusZ;
        public Vector2Int centerMapIndex;
        public Vector2Int startMapIndex;
        public float[,] heightMap;
        public float limit;
    }
}