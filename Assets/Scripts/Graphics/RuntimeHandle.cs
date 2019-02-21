using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 挂在相机上
/// </summary>
public class RuntimeHandle : MonoBehaviour
{
    public Transform target;
    private Camera camera;
    private Material lineMaterial;
    private Material quadeMaterial;

    private float handleScale = 1;
    private float quadScale = 0.2f;    // 方块长度和轴长度的比例
    private float arrowScale = 1f;
    private float screenScale = 0;

    private float colliderPixel = 10;  // 鼠标距离轴多少时算有碰撞（单位：像素）

    private bool lockX = false;
    private bool lockY = false;
    private bool lockZ = false;

    private RuntimeHandleAxis selectedAxis = RuntimeHandleAxis.None; // 当前有碰撞的轴

    private Color selectedColor = Color.yellow;
    private Color selectedColorA = new Color(1, 0.92f, 0.016f, 0.2f);
    private Color redA = new Color(1, 0, 0, 0.2f);
    private Color greenA = new Color(0, 1, 0, 0.2f);
    private Color blueA = new Color(0, 0, 1, 0.2f);

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

    void OnPostRender()
    {
        if (target)
        {
            DrawCoordinate(target);
        }
    }

    private void Update()
    {
        if (target)
        {
            selectedAxis = SelectedAxis();

            // TODO: 手柄控制目标的行为

        }
    }

    /// <summary>
    /// 绘制手柄
    /// </summary>
    private void DrawCoordinate(Transform target)
    {
        Vector3 position = target.position;
        screenScale = GetScreenScale(target.position, camera);
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
        GL.Color(selectedAxis == RuntimeHandleAxis.X ? selectedColor : Color.red);
        GL.Vertex(o);
        GL.Vertex(x);
        GL.Color(selectedAxis == RuntimeHandleAxis.Y ? selectedColor : Color.green);
        GL.Vertex(o);
        GL.Vertex(y);
        GL.Color(selectedAxis == RuntimeHandleAxis.Z ? selectedColor : Color.blue);
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
        GL.Color(selectedAxis == RuntimeHandleAxis.YZ ? selectedColorA : redA);
        GL.Vertex(o * quadScale);
        GL.Vertex(y * quadScale);
        GL.Vertex((y + z) * quadScale);
        GL.Vertex(z * quadScale);
        GL.Color(selectedAxis == RuntimeHandleAxis.XZ ? selectedColorA : greenA);
        GL.Vertex(o * quadScale);
        GL.Vertex(x * quadScale);
        GL.Vertex((x + z) * quadScale);
        GL.Vertex(z * quadScale);
        GL.Color(selectedAxis == RuntimeHandleAxis.XY ? selectedColorA : blueA);
        GL.Vertex(o * quadScale);
        GL.Vertex(x * quadScale); 
        GL.Vertex((x + y) * quadScale);
        GL.Vertex(y * quadScale);
        GL.End();

        GL.PopMatrix();

        Vector3 euler = target.eulerAngles;
        // 画坐标轴的箭头
        Mesh meshX = CreateArrow(selectedAxis == RuntimeHandleAxis.X ? selectedColor : Color.red, arrowScale * screenScale);
        Graphics.DrawMeshNow(meshX, position + target.right * handleScale * screenScale, Quaternion.Euler(new Vector3(0, 0, -90) + euler));
        Mesh meshY = CreateArrow(selectedAxis == RuntimeHandleAxis.Y ? selectedColor : Color.green, arrowScale * screenScale);
        Graphics.DrawMeshNow(meshY, position + target.up * handleScale * screenScale, Quaternion.Euler(euler));
        Mesh meshZ = CreateArrow(selectedAxis == RuntimeHandleAxis.Z ? selectedColor : Color.blue, arrowScale * screenScale);
        Graphics.DrawMeshNow(meshZ, position + target.forward * handleScale * screenScale, Quaternion.Euler(new Vector3(90, 0, 0) + euler));
    }

    /// <summary>
    /// 创建一个轴的箭头网格
    /// </summary>
    private Mesh CreateArrow(Color color, float scale)
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

    /// <summary>
    /// 创建一个方块网格 
    /// </summary>
    public Mesh CreateCubeMesh(Color color, Vector3 center, float scale, float cubeLength = 1, float cubeWidth = 1, float cubeHeight = 1)
    {
        cubeHeight *= scale;
        cubeWidth *= scale;
        cubeLength *= scale;

        Vector3 vertice_0 = center + new Vector3(-cubeLength * .5f, -cubeWidth * .5f, cubeHeight * .5f);
        Vector3 vertice_1 = center + new Vector3(cubeLength * .5f, -cubeWidth * .5f, cubeHeight * .5f);
        Vector3 vertice_2 = center + new Vector3(cubeLength * .5f, -cubeWidth * .5f, -cubeHeight * .5f);
        Vector3 vertice_3 = center + new Vector3(-cubeLength * .5f, -cubeWidth * .5f, -cubeHeight * .5f);
        Vector3 vertice_4 = center + new Vector3(-cubeLength * .5f, cubeWidth * .5f, cubeHeight * .5f);
        Vector3 vertice_5 = center + new Vector3(cubeLength * .5f, cubeWidth * .5f, cubeHeight * .5f);
        Vector3 vertice_6 = center + new Vector3(cubeLength * .5f, cubeWidth * .5f, -cubeHeight * .5f);
        Vector3 vertice_7 = center + new Vector3(-cubeLength * .5f, cubeWidth * .5f, -cubeHeight * .5f);
        Vector3[] vertices = new[]
        {
                // Bottom Polygon
                vertice_0, vertice_1, vertice_2, vertice_3,
                // Left Polygon
                vertice_7, vertice_4, vertice_0, vertice_3,
                // Front Polygon
                vertice_4, vertice_5, vertice_1, vertice_0,
                // Back Polygon
                vertice_6, vertice_7, vertice_3, vertice_2,
                // Right Polygon
                vertice_5, vertice_6, vertice_2, vertice_1,
                // Top Polygon
                vertice_7, vertice_6, vertice_5, vertice_4
            };

        int[] triangles = new[]
        {
                // Cube Bottom Side Triangles
                3, 1, 0,
                3, 2, 1,    
                // Cube Left Side Triangles
                3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
                3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,
                // Cube Front Side Triangles
                3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
                3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,
                // Cube Back Side Triangles
                3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
                3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,
                // Cube Rigth Side Triangles
                3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
                3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,
                // Cube Top Side Triangles
                3 + 4 * 5, 1 + 4 * 5, 0 + 4 * 5,
                3 + 4 * 5, 2 + 4 * 5, 1 + 4 * 5,
            };

        Color[] colors = new Color[vertices.Length];
        for (int i = 0; i < colors.Length; ++i)
        {
            colors[i] = color;
        }

        Mesh cubeMesh = new Mesh();
        cubeMesh.name = "cube";
        cubeMesh.vertices = vertices;
        cubeMesh.triangles = triangles;
        cubeMesh.colors = colors;
        cubeMesh.RecalculateNormals();
        return cubeMesh;
    }

    /// <summary>
    /// 返回鼠标和手柄的碰撞信息
    /// </summary>
    private RuntimeHandleAxis SelectedAxis()
    {
        Matrix4x4 mat = Matrix4x4.TRS(target.position, target.rotation, Vector3.one * screenScale);
        bool hit = HitAxis(Vector3.right, mat, out float distanceX);
        hit |= HitAxis(Vector3.up, mat, out float distanceY);
        hit |= HitAxis(Vector3.forward, mat, out float distanceZ);

        if (hit)
        {
            if (distanceX < distanceY && distanceX < distanceZ)
            {
                return RuntimeHandleAxis.X;
            }
            else if (distanceY < distanceZ)
            {
                return RuntimeHandleAxis.Y;
            }
            else
            {
                return RuntimeHandleAxis.Z;
            }
        }

        return RuntimeHandleAxis.None;
    }

    /// <summary>
    /// 是否和手柄有碰撞
    /// </summary>
    /// <param name="axis"></param>
    /// <param name="matrix">手柄坐标系转换矩阵</param>
    /// <param name="distanceAxis"></param>
    /// <returns></returns>
    private bool HitAxis(Vector3 axis, Matrix4x4 matrix, out float distanceToAxis)
    {
        // 把坐标轴本地坐标转为世界坐标
        axis = matrix.MultiplyPoint(axis);

        // 坐标轴转屏幕坐标(有问题)
        Vector2 screenVectorBegin = camera.WorldToScreenPoint(target.position);
        Vector2 screenVectorEnd = camera.WorldToScreenPoint(axis);
        Vector2 screenVector = screenVectorEnd - screenVectorBegin;
        float screenVectorMag = screenVector.magnitude;
        screenVector.Normalize();

        if (screenVector != Vector2.zero) 
        {
            Vector2 perp = PerpendicularClockwise(screenVector).normalized;
            Vector2 mousePosition = Input.mousePosition;
            Vector2 relMousePositon = mousePosition - screenVectorBegin;    // 鼠标相对轴远点位置
            distanceToAxis = Mathf.Abs(Vector2.Dot(perp, relMousePositon)); // 在屏幕坐标系中，鼠标到轴的距离

            Vector2 hitPoint = (relMousePositon - perp * distanceToAxis);
            float vectorSpaceCoord = Vector2.Dot(screenVector, hitPoint);

            bool result = vectorSpaceCoord >= 0 && hitPoint.magnitude <= screenVectorMag && distanceToAxis < colliderPixel;
            return result;
        }
        else  // 坐标轴正对屏幕
        {
            Vector2 mousePosition = Input.mousePosition;

            distanceToAxis = (screenVectorBegin - mousePosition).magnitude;
            bool result = distanceToAxis <= colliderPixel;
            if (!result)
            {
                distanceToAxis = float.PositiveInfinity;
            }
            else
            {
                distanceToAxis = 0.0f;
            }
            return result;
        }
    }

    /// <summary>
    /// 是否和小方块有碰撞
    /// </summary>
    /// <param name="axis"></param>
    /// <param name="matrix"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    private bool HitQuad(Vector3 axis, Matrix4x4 matrix, float size)
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(matrix.MultiplyVector(axis).normalized, matrix.MultiplyPoint(Vector3.zero));

        if (!plane.Raycast(ray, out float distance))
        {
            return false;
        }

        Vector3 point = ray.GetPoint(distance);
        point = matrix.inverse.MultiplyPoint(point);

        Vector3 toCam = matrix.inverse.MultiplyVector(camera.transform.position/* - HandlePosition*/);

        float fx = Mathf.Sign(Vector3.Dot(toCam, Vector3.right));
        float fy = Mathf.Sign(Vector3.Dot(toCam, Vector3.up));
        float fz = Mathf.Sign(Vector3.Dot(toCam, Vector3.forward));

        point.x *= fx;
        point.y *= fy;
        point.z *= fz;

        float lowBound = -0.01f;

        bool result = point.x >= lowBound && point.x <= size && point.y >= lowBound && point.y <= size && point.z >= lowBound && point.z <= size;

        if (result)
        {
            //DragPlane = GetDragPlane(matrix, axis);
        }

        return result;
    }

    /// <summary>
    /// 控制目标
    /// </summary>
    private void ControlTarget()
    {

    }



    // ------------- Tools -------------- //

    /// <summary>
    /// 获取顺时针的垂直向量
    /// </summary>
    private Vector2 PerpendicularClockwise(Vector2 vector2)
    {
        return new Vector2(-vector2.y, vector2.x);
    }

    /// <summary>
    /// 通过一个世界左边和相机获取比例
    /// </summary>
    private float GetScreenScale(Vector3 position, Camera camera)
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
}

public enum RuntimeHandleAxis
{
    None,
    X,
    Y,
    Z,
    XY,
    XZ,
    YZ,
}