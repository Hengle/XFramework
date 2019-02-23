using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 挂在相机上
/// </summary>
public class RuntimeHandle : MonoBehaviour
{
    private float handleScale = 1;
    private float quadScale = 0.2f;    // 方块长度和轴长度的比例
    private float arrowScale = 1f;
    private float cubeScale = 0.15f;
    private float circleRadius = 0.6f;

    public static Vector3 quadDir = Vector3.one;

    private static bool lockX = false;
    private static bool lockY = false;
    private static bool lockZ = false;
    private bool mouseDonw = false;

    public static Vector3[] circlePosX;
    public static Vector3[] circlePosY;
    public static Vector3[] circlePosZ;

    private RuntimeHandleAxis selectedAxis = RuntimeHandleAxis.None; // 当前有碰撞的轴
    private TransformMode transformMode = TransformMode.Position;
    private BaseHandle currentHandle;

    private readonly PositionHandle positionHandle = new PositionHandle();
    private readonly RotationHandle rotationHandle = new RotationHandle();
    private readonly ScaleHandle scaleHandle = new ScaleHandle();

    private Color selectedColor = Color.yellow;
    private Color selectedColorA = new Color(1, 0.92f, 0.016f, 0.2f);
    private Color redA = new Color(1, 0, 0, 0.2f);
    private Color greenA = new Color(0, 1, 0, 0.2f);
    private Color blueA = new Color(0, 0, 1, 0.2f);

    private Material lineMaterial;
    private Material quadeMaterial;
    private Material shapesMaterial;

    public static RuntimeHandle instance;
    public static Matrix4x4 localToWorld { get; private set; }
    public static float screenScale { get; private set; }
    public static Transform target { get; private set; }
    public static Camera camera { get; private set; }

    private void Awake()
    {
        if (instance = null)
        {
            instance = this;
        }

        lineMaterial = new Material(Shader.Find("Battlehub/RTHandles/VertexColor"));
        lineMaterial.color = Color.white;
        quadeMaterial = new Material(Shader.Find("Battlehub/RTHandles/VertexColor"));
        quadeMaterial.color = Color.white;
        shapesMaterial = new Material(Shader.Find("Battlehub/RTHandles/Shape"));
        shapesMaterial.color = Color.white;

        camera = GetComponent<Camera>();
        currentHandle = positionHandle;

        SetTarget(GameObject.Find("Cube").transform);
    }

    private void Update()
    {
        if (target)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                currentHandle = positionHandle;
                transformMode = TransformMode.Position;
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                currentHandle = rotationHandle;
                transformMode = TransformMode.Rotation;
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                currentHandle = scaleHandle;
                transformMode = TransformMode.Scale;
            }

            if (!mouseDonw)
                selectedAxis = currentHandle.SelectedAxis();

            ControlTarget();
        }
    }

    void OnPostRender()
    {
        if (target)
        {
            screenScale = GetScreenScale(target.position, camera);
            localToWorld = Matrix4x4.TRS(target.position, target.rotation, Vector3.one * screenScale);
            DrawHandle(target);
        }
    }

    /// <summary>
    /// 根据变换模式绘制不同的手柄
    /// </summary>
    private void DrawHandle(Transform target)
    {
        switch (transformMode)
        {
            case TransformMode.Position:
                DoPosition(target);
                break;
            case TransformMode.Rotation:
                DoRotation(target);
                break;
            case TransformMode.Scale:
                DoSacle(target);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 绘制位移手柄
    /// </summary>
    private void DoPosition(Transform target)
    {
        DrawCoordinate(target, true);
        DrawCoordinateArrow(target);
    }

    /// <summary>
    /// 绘制旋转手柄
    /// </summary>
    /// <param name="target"></param>
    private void DoRotation(Transform target)
    {
        Matrix4x4 transform = Matrix4x4.TRS(target.position, target.rotation, Vector3.one * screenScale);

        lineMaterial.SetPass(0);
        GL.PushMatrix();
        GL.MultMatrix(transform);
        GL.Begin(GL.LINES);

        DrawCircle(target, Vector3.right, circleRadius, selectedAxis == RuntimeHandleAxis.X ? selectedColor : Color.red);
        DrawCircle(target, Vector3.up, circleRadius, selectedAxis == RuntimeHandleAxis.Y ? selectedColor : Color.green);
        DrawCircle(target, Vector3.forward, circleRadius, selectedAxis == RuntimeHandleAxis.Z ? selectedColor : Color.blue);

        GL.End();
        GL.PopMatrix();
    }

    /// <summary>
    /// 绘制比例手柄
    /// </summary>
    private void DoSacle(Transform target)
    {
        DrawCoordinate(target, false);
        DrawCoordinateCube(target);
    }

    /// <summary>
    /// 绘制坐标系
    /// </summary>
    private void DrawCoordinate(Transform target, bool hasQuad)
    {
        Vector3 position = target.position;
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
        GL.End();

        Vector3 dir = position - camera.transform.position;
        float angleX = Vector3.Angle(target.right, dir);
        float angleY = Vector3.Angle(target.up, dir);
        float angleZ = Vector3.Angle(target.forward, dir);

        bool signX = angleX >= 90 && angleX < 270;
        bool signY = angleY >= 90 && angleY < 270;
        bool signZ = angleZ >= 90 && angleZ < 270;

        quadDir = Vector3.one;
        if (!signX)
        {
            x = -x;
            quadDir.x = -1;
        }
        if (!signY)
        {
            y = -y;
            quadDir.y = -1;
        }
        if (!signZ)
        {
            z = -z;
            quadDir.z = -1;
        }

        // 画方块的边框线
        if (hasQuad)
        {
            GL.Begin(GL.LINES);
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
        }

        GL.PopMatrix();
    }

    /// <summary>
    /// 画一个空心圆
    /// </summary>
    private void DrawCircle(Transform target, Vector3 axis, float radius, Color color)
    {
        int detlaAngle = 10;
        float x;
        float y;
        GL.Color(color);

        Vector3 start;
        if (axis.x == 1)
            start = Vector3.up * radius;
        else
            start = Vector3.right * radius;
        Vector3[] circlePos;

        circlePos = new Vector3[360 / detlaAngle];
        
        GL.Vertex(start);
        circlePos[0] = start;
        for (int i = 1; i < 360 / detlaAngle; i++)
        {
            x = Mathf.Cos(i * detlaAngle * Mathf.Deg2Rad) * radius;
            y = Mathf.Sin(i * detlaAngle * Mathf.Deg2Rad) * radius;

            Vector3 temp;
            if (axis.x == 1)
                temp = new Vector3(0, x, y);
            else if (axis.y == 1)
                temp = new Vector3(x, 0, y);
            else
                temp = new Vector3(x, y, 0);
            GL.Vertex(temp);
            GL.Vertex(temp);
            circlePos[i] = temp;
        }
        GL.Vertex(start);

        if (axis.x == 1)
            circlePosX = circlePos;
        else if (axis.y == 1)
            circlePosY = circlePos;
        else
            circlePosZ = circlePos;
    }

    /// <summary>
    /// 绘制坐标系箭头
    /// </summary>
    private void DrawCoordinateArrow(Transform target)
    {
        Vector3 position = target.position;
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
    /// 绘制坐标系小正方体
    /// </summary>
    private void DrawCoordinateCube(Transform target)
    {
        Vector3 position = target.position;
        Vector3 euler = target.eulerAngles;
        // 画坐标轴的小方块
        shapesMaterial.SetPass(0);
        Mesh meshX = CreateCubeMesh(selectedAxis == RuntimeHandleAxis.X ? selectedColor : Color.red, Vector3.zero, cubeScale * screenScale);
        Graphics.DrawMeshNow(meshX, position + target.right * handleScale * screenScale, Quaternion.identity);
        Mesh meshY = CreateCubeMesh(selectedAxis == RuntimeHandleAxis.Y ? selectedColor : Color.green, Vector3.zero, cubeScale * screenScale);
        Graphics.DrawMeshNow(meshY, position + target.up * handleScale * screenScale, Quaternion.identity);
        Mesh meshZ = CreateCubeMesh(selectedAxis == RuntimeHandleAxis.Z ? selectedColor : Color.blue, Vector3.zero, cubeScale * screenScale);
        Graphics.DrawMeshNow(meshZ, position + target.forward * handleScale * screenScale, Quaternion.identity);

        Mesh meshO = CreateCubeMesh(selectedAxis == RuntimeHandleAxis.XYZ ? selectedColor : Color.white, Vector3.zero, cubeScale * screenScale);
        Graphics.DrawMeshNow(meshO, position, Quaternion.identity);
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
    /// 控制目标
    /// </summary>
    private void ControlTarget()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            mouseDonw = true;
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            float inputX = Input.GetAxis("Mouse X");
            float inputY = Input.GetAxis("Mouse Y");

            float x = 0;
            float y = 0;
            float z = 0;

            switch (selectedAxis)
            {
                case RuntimeHandleAxis.None:
                    break;
                case RuntimeHandleAxis.X:
                    if (!lockX)
                        x = currentHandle.GetTransformAxis(new Vector2(inputX, inputY), target.right);
                    break;
                case RuntimeHandleAxis.Y:
                    if (!lockY)
                        y = currentHandle.GetTransformAxis(new Vector2(inputX, inputY), target.up);
                    break;
                case RuntimeHandleAxis.Z:
                    if (!lockZ)
                        z = currentHandle.GetTransformAxis(new Vector2(inputX, inputY), target.forward);
                    break;
                case RuntimeHandleAxis.XY:
                    if (!lockX)
                        x = currentHandle.GetTransformAxis(new Vector2(inputX, inputY), target.right);
                    if (!lockY)
                        y = currentHandle.GetTransformAxis(new Vector2(inputX, inputY), target.up);
                    break;
                case RuntimeHandleAxis.XZ:
                    if (!lockX)
                        x = currentHandle.GetTransformAxis(new Vector2(inputX, inputY), target.right);
                    if (!lockZ)
                        z = currentHandle.GetTransformAxis(new Vector2(inputX, inputY), target.forward);
                    break;
                case RuntimeHandleAxis.YZ:
                    if (!lockY)
                        y = currentHandle.GetTransformAxis(new Vector2(inputX, inputY), target.up);
                    if (!lockZ)
                        z = currentHandle.GetTransformAxis(new Vector2(inputX, inputY), target.forward);
                    break;
                case RuntimeHandleAxis.XYZ:
                    x = y = z = inputX;
                    if (lockX)
                        x = 0;
                    if (lockY)
                        y = 0;
                    if (lockZ)
                        z = 0;
                    break;
                default:
                    break;
            }

            currentHandle.Transform(new Vector3(x, y, z) * screenScale);
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            mouseDonw = false;
        }
    }


    // ------------- Tools -------------- //


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


    // ---------------- 外部调用 ------------------- //

    public static void SetTarget(Transform _target)
    {
        target = _target;
    }

    public static void ConfigFreeze(bool _lockX = false, bool _lockY = false, bool _locKZ = false)
    {
        lockX = _lockX;
        lockY = _lockY;
        lockZ = _locKZ;
    }

    public static void Disable()
    {
        target = null;
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
    XYZ,
}

public enum TransformMode
{
    Position,
    Rotation,
    Scale,
}