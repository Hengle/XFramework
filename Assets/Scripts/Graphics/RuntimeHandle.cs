using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 挂在相机上
/// </summary>
public class RuntimeHandle : MonoBehaviour
{
    public Transform target;
    private Material lineMaterial;
    private Material quadeMaterial;
    private float handleScale = 1;
    private float quadScale = 0.2f;
    private float arrowScale = 1f;
    private Camera camera;


    private void Awake()
    {
        if (!lineMaterial)
        {
            lineMaterial = new Material(Shader.Find("Battlehub/RTHandles/VertexColor"));
            lineMaterial.color = Color.white;
            quadeMaterial = new Material(Shader.Find("Battlehub/RTHandles/VertexColor"));
            quadeMaterial.color = Color.white;
        }

        camera = GetComponent<Camera>();
    }

    private void Start()
    {
        // 创建一个矩阵，他可以将本地坐标转化为世界坐标
        Matrix4x4 mat = Matrix4x4.TRS(Vector3.up, Quaternion.identity, Vector3.one);
        //Debug.Log(mat.MultiplyPoint(Vector3.zero));

        Matrix4x4 mat2 = GameObject.Find("C").transform.worldToLocalMatrix;
        Debug.Log(mat2.MultiplyPoint(Vector3.zero)); 
        //mat.mu
    }

    void OnPostRender()
    {
        if (target)
        {
            DrawCoordinate(target);
        }
    }

    private void Update()
    {

    }

    private void DrawCoordinate(Transform target)
    {
        Vector3 position = target.position;
        float screenScale = GetScreenScale(target.position, camera);
        Matrix4x4 transform = Matrix4x4.TRS(target.position, target.rotation, Vector3.one * screenScale);

        lineMaterial.SetPass(0);
        Vector3 x = Vector3.right * handleScale;
        Vector3 y = Vector3.up * handleScale;
        Vector3 z = Vector3.forward * handleScale;
        Vector3 xy = x + y;
        Vector3 xz = x + z;
        Vector3 yz = y + z;
        Vector3 o = Vector3.zero;

        GL.PushMatrix();
        GL.MultMatrix(transform);   // 在绘制的时候GL会用这个矩阵转换坐标

        // 画三个坐标轴线段
        GL.Begin(GL.LINES);
        GL.Color(Color.red);
        GL.Vertex(o);
        GL.Vertex(x);
        GL.Color(Color.green);
        GL.Vertex(o);
        GL.Vertex(y);
        GL.Color(Color.blue);
        GL.Vertex(o);
        GL.Vertex(z);

        Vector3 dir = position - camera.transform.position;
        float angleX = Vector3.Angle(target.right, dir);
        float angleY = Vector3.Angle(target.up, dir);
        float angleZ = Vector3.Angle(target.forward, dir);


        bool signX = angleX >= 90 && angleX < 270;
        bool signY = angleY >= 90 && angleY < 270;
        bool signZ = angleZ >= 90 && angleZ < 270;
        if (!signX)
            x = -x;
        if (!signY)
            y = -y;
        if (!signZ)
            z = -z;

        // 画方块的边框线
        GL.Color(Color.red);
        GL.Vertex(y * quadScale);
        GL.Vertex((y + z) * quadScale);
        GL.Vertex((y + z) * quadScale);
        GL.Vertex(z * quadScale);
        GL.Color(Color.green);
        GL.Vertex(x * quadScale);
        GL.Vertex((x + z) * quadScale);
        GL.Vertex((x + z) * quadScale);
        GL.Vertex(z * quadScale);
        GL.Color(Color.blue);
        GL.Vertex(x * quadScale);
        GL.Vertex((x + y) * quadScale);
        GL.Vertex((x + y) * quadScale);
        GL.Vertex(y * quadScale);
        GL.End();

        // 画三个小方块
        GL.Begin(GL.QUADS);
        GL.Color(new Color(1, 0, 0, 0.2f));
        GL.Vertex(o * quadScale);
        GL.Vertex(y * quadScale);
        GL.Vertex((y + z) * quadScale);
        GL.Vertex(z * quadScale);
        GL.Color(new Color(0, 1, 0, 0.2f));
        GL.Vertex(o * quadScale);
        GL.Vertex(x * quadScale);
        GL.Vertex((x + z) * quadScale);
        GL.Vertex(z * quadScale);
        GL.Color(new Color(0, 0, 1, 0.2f));
        GL.Vertex(o * quadScale);
        GL.Vertex(x * quadScale); 
        GL.Vertex((x + y) * quadScale);
        GL.Vertex(y * quadScale);
        GL.End();

        GL.PopMatrix();

        Vector3 euler = target.eulerAngles;
        // 画坐标轴的箭头
        Mesh meshX = CreateArrow(Color.red, arrowScale * screenScale);
        Graphics.DrawMeshNow(meshX, position + target.right * handleScale * screenScale, Quaternion.Euler(new Vector3(0, 0, -90) + euler));
        Mesh meshY = CreateArrow(Color.green, arrowScale * screenScale);
        Graphics.DrawMeshNow(meshY, position + target.up * handleScale * screenScale, Quaternion.Euler(euler));
        Mesh meshZ = CreateArrow(Color.blue, arrowScale * screenScale);
        Graphics.DrawMeshNow(meshZ, position + target.forward * handleScale * screenScale, Quaternion.Euler(new Vector3(90, 0, 0) + euler));
    }

    public float GetScreenScale(Vector3 position, Camera camera)
    {
        float h = camera.pixelHeight;
        if (camera.orthographic)
        {
            return camera.orthographicSize * 2f / h * 90;
        }

        Transform transform = camera.transform;
        float distance = Vector3.Dot(position - transform.position, transform.forward);       // Position位置的深度距离
        float scale = 2.0f * distance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad); // 在Position的深度上，每个像素点对应的y轴距离
        return scale / h * 90; // 90为自定义系数
    }

    public static Mesh CreateArrow(Color color, float scale)
    {
        int segmentsCount = 12;  // 侧面三角形数量
        float size = 1.0f / 5;
        size *= scale;

        Vector3[] vertices = new Vector3[segmentsCount + 2];
        int[] triangles = new int[segmentsCount * 6];
        Color[] colors = new Color[vertices.Length];
        for (int i = 0; i < colors.Length; ++i)
        {
            // 顶点颜色
            colors[i] = color;
        }

        float radius = size / 2.6f; // 地面半径
        float height = size;        // 高
        float deltaAngle = Mathf.PI * 2.0f / segmentsCount;

        float y = -height;

        vertices[vertices.Length - 1] = new Vector3(0, -height, 0); // 圆心点
        vertices[vertices.Length - 2] = Vector3.zero;               // 锥顶

        // 地面圆上的点
        for (int i = 0; i < segmentsCount; i++)
        {
            float angle = i * deltaAngle;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;

            vertices[i] = new Vector3(x, y, z);
        }

        for (int i = 0; i < segmentsCount; i++)
        {
            // 底面三角形排序
            triangles[i * 6] = vertices.Length - 1;
            triangles[i * 6 + 1] = i;
            triangles[i * 6 + 2] = (i + 1) % segmentsCount;

            // 侧面三角形排序
            triangles[i * 6 + 3] = vertices.Length - 2;
            triangles[i * 6 + 4] = i;
            triangles[i * 6 + 5] = (i + 1) % segmentsCount;
        }

        Mesh cone = new Mesh
        {
            name = "Cone",
            vertices = vertices,
            triangles = triangles,
            colors = colors
        };

        return cone;
    }
}