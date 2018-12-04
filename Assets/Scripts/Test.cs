using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

namespace UnityEngine
{
    public class Test : MonoBehaviour
    {
        // Start is called before the first frame update
        void OnEnable()
        {
            Terrain.activeTerrain.terrainData.treeInstances = new TreeInstance[0];
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                //RaycastHit hitInfo = Utility.SendRay(LayerMask.GetMask("Terrain"));
                RaycastHit hitInfo = Utility.SendRay(-1);
                TerrainUtility.CreatTree(Terrain.activeTerrain, hitInfo.point, 200, 50);

                
            }

            // 利用反射删除树
            if (Input.GetMouseButtonDown(1))
            {
                RaycastHit hitInfo = Utility.SendRay(-1);
                Vector2 v2 = new Vector2(hitInfo.point.x, hitInfo.point.z);
                v2.x /= Terrain.activeTerrain.terrainData.size.x; 
                v2.y /= Terrain.activeTerrain.terrainData.size.z; 

                Terrain terrain = Terrain.activeTerrain;
                BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
                Type type = terrain.GetType();
                MethodInfo m = type.GetMethod("RemoveTrees", flags);
                object[] objs = new object[] { v2, 50/Terrain.activeTerrain.terrainData.size.x, 0 };
                m.Invoke(terrain, objs);
            }
        }
    }
}