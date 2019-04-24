﻿using System.Collections;
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
    private bool mouseDonw = false;    // 鼠标左键是否按下

    private RuntimeHandleAxis selectedAxis = RuntimeHandleAxis.None; // 当前有碰撞的轴
    private TransformMode transformMode = TransformMode.Position;    // 当前控制类型
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

    public static Matrix4x4 localToWorld { get; private set; }
    public static float screenScale { get; private set; }
    public static Transform target { get; private set; }
    public Transform testTarget;
    public static new Camera camera { get; private set; }

    public static Vector3[] circlePosX;
    public static Vector3[] circlePosY;
    public static Vector3[] circlePosZ;

    private void Awake()
    {

        lineMaterial = new Material(Shader.Find("RunTimeHandles/VertexColor"));
        lineMaterial.color = Color.white;
        quadeMaterial = new Material(Shader.Find("RunTimeHandles/VertexColor"));
        quadeMaterial.color = Color.white;
        shapesMaterial = new Material(Shader.Find("RunTimeHandles/Shape"));
        shapesMaterial.color = Color.white;

        camera = GetComponent<Camera>();
        currentHandle = positionHandle;

        // 测试用
        if (testTarget)
        {
            SetTarget(testTarget);
        }
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

        DrawCircle(Vector3.right, circleRadius, selectedAxis == RuntimeHandleAxis.X ? selectedColor : Color.red);
        DrawCircle(Vector3.up, circleRadius, selectedAxis == RuntimeHandleAxis.Y ? selectedColor : Color.green);
        DrawCircle(Vector3.forward, circleRadius, selectedAxis == RuntimeHandleAxis.Z ? selectedColor : Color.blue);

        GL.End();
        GL.PopMatrix();

        DrawScreenCircle(Color.white, target.position, 60);
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
    private void DrawCircle(Vector3 axis, float radius, Color color)
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
        // 画坐标轴的箭头 (箭头的锥顶不是它自身坐标的forword)
        Mesh meshX = GLDraw.CreateArrow(selectedAxis == RuntimeHandleAxis.X ? selectedColor : Color.red, arrowScale * screenScale);
        Graphics.DrawMeshNow(meshX, position + target.right * handleScale * screenScale, target.rotation * Quaternion.Euler(0, 0, -90));
        Mesh meshY = GLDraw.CreateArrow(selectedAxis == RuntimeHandleAxis.Y ? selectedColor : Color.green, arrowScale * screenScale);
        Graphics.DrawMeshNow(meshY, position + target.up * handleScale * screenScale, target.rotation);
        Mesh meshZ = GLDraw.CreateArrow(selectedAxis == RuntimeHandleAxis.Z ? selectedColor : Color.blue, arrowScale * screenScale);
        Graphics.DrawMeshNow(meshZ, position + target.forward * handleScale * screenScale, target.rotation * Quaternion.Euler(90, 0, 0));
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
        Mesh meshX = GLDraw.CreateCubeMesh(selectedAxis == RuntimeHandleAxis.X ? selectedColor : Color.red, Vector3.zero, cubeScale * screenScale);
        Graphics.DrawMeshNow(meshX, position + target.right * handleScale * screenScale, target.rotation * Quaternion.Euler(0, 0, -90));
        Mesh meshY = GLDraw.CreateCubeMesh(selectedAxis == RuntimeHandleAxis.Y ? selectedColor : Color.green, Vector3.zero, cubeScale * screenScale);
        Graphics.DrawMeshNow(meshY, position + target.up * handleScale * screenScale, target.rotation);
        Mesh meshZ = GLDraw.CreateCubeMesh(selectedAxis == RuntimeHandleAxis.Z ? selectedColor : Color.blue, Vector3.zero, cubeScale * screenScale);
        Graphics.DrawMeshNow(meshZ, position + target.forward * handleScale * screenScale, target.rotation * Quaternion.Euler(90, 0, 0));

        Mesh meshO = GLDraw.CreateCubeMesh(selectedAxis == RuntimeHandleAxis.XYZ ? selectedColor : Color.white, Vector3.zero, cubeScale * screenScale);
        Graphics.DrawMeshNow(meshO, position, target.rotation);
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

    private void DrawScreenCircle(Color color,Vector3 position,int pixel)
    {
        Vector2 temp = camera.WorldToScreenPoint(position);
        Vector3 offset = new Vector3(temp.x, temp.y, 0);

        lineMaterial.SetPass(0);
        GL.PushMatrix();
        GL.LoadPixelMatrix();
        GL.Begin(GL.LINES);
        GL.Color(color);

        int detlaAngle = 10;
        float x;
        float y;

        GL.Vertex(new Vector3(1, 0, 0) * pixel + offset);
        for (int i = 1; i < 360 / detlaAngle; i++)
        {
            x = Mathf.Cos(i * detlaAngle * Mathf.Deg2Rad) * pixel;
            y = Mathf.Sin(i * detlaAngle * Mathf.Deg2Rad) * pixel;

            GL.Vertex(new Vector3(x, y, 0) + offset);
            GL.Vertex(new Vector3(x, y, 0) + offset);
        }
        GL.Vertex(new Vector3(1, 0, 0) * pixel + offset);
        GL.End();
        GL.PopMatrix();

    }

    private void DoRotationNew()
    {
        Quaternion rotation = target.rotation;
        Vector3 scale = new Vector3(screenScale, screenScale, screenScale);
        Matrix4x4 xTranform = Matrix4x4.TRS(Vector3.zero, rotation * Quaternion.AngleAxis(-90, Vector3.up), Vector3.one);
        Matrix4x4 yTranform = Matrix4x4.TRS(Vector3.zero, rotation * Quaternion.AngleAxis(90, Vector3.right), Vector3.one);
        Matrix4x4 zTranform = Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one);
        Matrix4x4 objToWorld = Matrix4x4.TRS(target.position, Quaternion.identity, screenScale * Vector3.one);

        lineMaterial.SetPass(0);
        GL.PushMatrix();
        GL.MultMatrix(objToWorld);
        GL.Begin(GL.LINES);

        DrawCircleNew(xTranform, Color.red, circleRadius);

        GL.End();
        GL.PopMatrix();
    }

    private void DrawCircleNew(Matrix4x4 transform,Color color,float radius)
    {
        int detlaAngle = 10;
        float x;
        float z;
        GL.Color(color);

        Vector3 start;
        start = transform.MultiplyPoint(Vector3.right * radius);

        GL.Vertex(start);
        for (int i = 1; i < 180 / detlaAngle; i++)
        {
            x = Mathf.Cos(i * detlaAngle * Mathf.Deg2Rad) * radius;
            z = Mathf.Sin(i * detlaAngle * Mathf.Deg2Rad) * radius;
            Vector3 temp = transform.MultiplyPoint(new Vector3(x, 0, z));
            GL.Vertex(temp);
            GL.Vertex(temp);
        }

        GL.Vertex(transform.MultiplyPoint(Vector3.left * radius));
    }


    // ------------- Tools -------------- //


    /// <summary>
    /// 通过一个世界坐标和相机获取比例
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

/// <summary>
/// 鼠标选择轴的类型
/// </summary>
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

/// <summary>
/// 控制模式
/// </summary>
public enum TransformMode
{
    Position,
    Rotation,
    Scale,
}