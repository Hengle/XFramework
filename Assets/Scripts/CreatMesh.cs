using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatMesh : MonoBehaviour {
    private MeshFilter filter;
    private Mesh mesh;

    private void Start()
    {
        filter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        filter.mesh = mesh;
        InitMesh();
    }

    void InitMesh()
    {
        mesh.name = "MyMesh";

        //创建顶点数组
        Vector3[] vertices = new Vector3[4]
        {
            new Vector3 (1,1,0),
            new Vector3 (-1,1,0),
            new Vector3 (1,-1,0),
            new Vector3 (-1,-1,0)
        };
        mesh.vertices = vertices;

        //通过顶点创建三角形
        int[] triangles = new int[2 * 3] { 0, 3, 1  ,  0, 2, 3 };
        mesh.triangles = triangles;

        //设置纹理贴图坐标  uv数组顶点数组一一对应
        Vector2[] uv = new Vector2[4]{ 
            new Vector2(1, 1),
            new Vector2(0, 1),
            new Vector2(1, 0),
            new Vector2(0, 0)
        };
        mesh.uv = uv;
    }
}
