using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 手柄基类
/// </summary>
public class BaseHandle
{
    private float colliderPixel = 10;  // 鼠标距离轴多少时算有碰撞（单位：像素）

    public virtual float GetTransformAxis(Vector2 inputDir, Vector3 axis, Transform target, Camera camera) { return 0; }
    public virtual void Transform(Transform target, Vector3 value) { }

    /// <summary>
    /// 最基本的碰撞选择
    /// </summary>
    public virtual RuntimeHandleAxis SelectedAxis(Transform target, Camera camera, float screenScale)
    {
        float distanceX, distanceY, distanceZ;
        Matrix4x4 mat = Matrix4x4.TRS(target.position, target.rotation, Vector3.one * screenScale);
        bool hit = HitAxis(Vector3.right, mat, out distanceX, camera, target);
        hit |= HitAxis(Vector3.up, mat, out distanceY, camera, target);
        hit |= HitAxis(Vector3.forward, mat, out distanceZ, camera, target);

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
    public bool HitAxis(Vector3 axis, Matrix4x4 matrix, out float distanceToAxis,Camera camera,Transform target)
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
    /// 获取顺时针的垂直向量
    /// </summary>
    protected Vector2 PerpendicularClockwise(Vector2 vector2)
    {
        return new Vector2(-vector2.y, vector2.x);
    }

}

/// <summary>
/// 位置修改手柄
/// </summary>
public class PositionHandle : BaseHandle
{
    /// <summary>
    /// 返回鼠标和手柄的碰撞信息
    /// </summary>
    public override RuntimeHandleAxis SelectedAxis(Transform target, Camera camera, float screenScale)
    {
        float scale = screenScale/* * 0.2f*/;
        // TODO 方块的位置是会变化的
        if(HitQuad(target.position,target.right + target.up, camera))
        {
            return RuntimeHandleAxis.XY;
        }
        else if(HitQuad(target.position, target.right + target.forward, camera))
        {
            return RuntimeHandleAxis.XZ;
        }
        else if (HitQuad(target.position, target.up + target.forward, camera))
        { 
            return RuntimeHandleAxis.YZ;
        }

        return base.SelectedAxis(target, camera, screenScale);
    }

    public override float GetTransformAxis(Vector2 inputDir, Vector3 axis, Transform target, Camera camera)
    {
        Vector2 screenStart = camera.WorldToScreenPoint(target.position);
        Vector2 screenEnd = camera.WorldToScreenPoint(target.position + axis);
        Vector2 screenDir = (screenEnd - screenStart).normalized;

        return Vector2.Dot(screenDir, inputDir);
    }

    public override void Transform(Transform target, Vector3 value)
    {
        target.Translate(value * Time.deltaTime * 10, Space.Self);
    }

    /// <summary>
    /// 是否和小方块有碰撞
    /// </summary>
    /// <param name="axis"></param>
    /// <param name="matrix"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    private bool HitQuad(Vector3 origin, Vector3 offset, Camera camera)
    {
        Vector2 mousePos = Input.mousePosition;
        Vector2 screenOrigin = camera.WorldToScreenPoint(origin);
        Vector2 screenOffset = camera.WorldToScreenPoint(origin + offset);

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log(mousePos);
            Debug.Log(screenOrigin);
            Debug.Log(screenOffset);
        }

        if (mousePos.x > Mathf.Max(screenOrigin.x, screenOffset.x) ||
            mousePos.x < Mathf.Min(screenOrigin.x, screenOffset.x) ||
            mousePos.y > Mathf.Max(screenOrigin.y, screenOffset.y) ||
            mousePos.y < Mathf.Min(screenOrigin.y, screenOffset.y))
            return false;
        else
            return true;
    }
}

/// <summary>
/// 角度修改手柄
/// </summary>
public class RotationHandle : BaseHandle
{
    //public override RuntimeHandleAxis SelectedAxis(Transform target, Camera camera, float screenScale)
    //{

    //}

    public override float GetTransformAxis(Vector2 inputDir, Vector3 axis, Transform target, Camera camera)
    {
        return 0;
    }

    public override void Transform(Transform target, Vector3 value)
    {
        
    }
}

/// <summary>
/// 比例修改手柄
/// </summary>
public class ScaleHandle : BaseHandle
{
    public override RuntimeHandleAxis SelectedAxis(Transform target, Camera camera, float screenScale)
    {
        // TODO xyz的情况


        return base.SelectedAxis(target, camera, screenScale);
    }

    public override float GetTransformAxis(Vector2 inputDir, Vector3 axis, Transform target, Camera camera)
    {
        Vector2 screenStart = camera.WorldToScreenPoint(target.position);
        Vector2 screenEnd = camera.WorldToScreenPoint(target.position + axis);
        Vector2 screenDir = (screenEnd - screenStart).normalized;

        return Vector2.Dot(screenDir, inputDir);
    }

    public override void Transform(Transform target, Vector3 value)
    {
        target.localScale += value * Time.deltaTime;
    }
}