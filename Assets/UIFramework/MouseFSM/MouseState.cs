using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum MouseStateType
{
    /// <summary>
    /// 默认空状态
    /// </summary>
    DefaultState,
    /// <summary>
    /// 创建单位状态
    /// </summary>
    CreateArmyState,
    /// <summary>
    /// 防空火力范围
    /// </summary>
    AttackRangeState,
    /// <summary>
    /// 对地攻击范围
    /// </summary>
    AirDefenceState,
    /// <summary>
    /// 炮兵火力打击范围
    /// </summary>
    ArtilleryRangeState,
    /// <summary>
    /// 移动
    /// </summary>
    MoveState,
    /// <summary>
    /// 攻击
    /// </summary>
    AttackState
}

public class MouseState
{
    /// <summary>
    /// 检测射线信息
    /// </summary>
    protected RaycastHit hitInfo;

    /// <summary>
    /// 状态激活时
    /// </summary>
    /// <param name="para"></param>
    public virtual void OnActive(object para = null) { }
    /// <summary>
    /// 状态结束时
    /// </summary>
    public virtual void OnDisactive() { }
    /// <summary>
    /// 左键按下
    /// </summary>
    public virtual void OnLeftButtonDown() { }
    /// <summary>
    /// 左键抬起
    /// </summary>
    public virtual void OnLeftButtonUp() { }
    /// <summary>
    /// 右键按下
    /// </summary>
    public virtual void OnRightButtonDown() { }
    /// <summary>
    /// 右键抬起
    /// </summary>
    public virtual void OnRightButtonUp() { }
    /// <summary>
    /// 每帧调用
    /// </summary>
    public virtual void Update() { }

    /// <summary>
    /// 发射一次射线更新hitInfo并返回当前鼠标接触到的物体
    /// </summary>
    /// <param name="layer">射线层级</param>
    /// <returns></returns>
    protected GameObject SendRay(int layer)
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, float.MaxValue, layer))
        {
            return hitInfo.collider.gameObject;
        }
        else
        {
            return null;
        }
    }
}

/// <summary>
/// 默认鼠标状态
/// </summary>
public class MouseDefaultState : MouseState
{

}

/// <summary>
/// 创建单位的鼠标状态
/// </summary>
public class MouseCreateArmyState : MouseState
{
    public GameObject gameObj = null;
    private Vector3 normalDir;
    private bool allowCreate = false;           // 是否允许
    private bool oldAllowCreate = false;              // 显示状态是否改便

    /// <summary>
    /// 初始化需要创建的单位
    /// </summary>
    /// <param name="para"></param>
    public override void OnActive(object para)
    {
        gameObj = (GameObject)para;
        hitInfo.point = Vector3.down * 1000;
        gameObj.GetComponent<Rigidbody>().isKinematic = true;
    }

    /// <summary>
    /// 实时更新鼠标位置的单位状态
    /// </summary>
    public override void Update()
    {
        if (null != gameObj)
        {
            // 鼠标位置显示单位的放置位置
            SendRay(LayerMask.GetMask("Terrain"));
            gameObj.transform.position = hitInfo.point + Vector3.up * 0.5f;

            normalDir = hitInfo.normal;
            gameObj.transform.up = normalDir;
#if false
            // 根据地图的点的法线方向判断是否可以创建单位
            //if (normalDir.y / normalDir.magnitude < 0.866f) // 夹角大于30度
            //{
            //    allowCreate = false;
            //}
            //else
            //{
            //    allowCreate = true;
            //}
#endif
            // 根据NavMesh判断是否可以创建
            NavMeshHit navHit;
            allowCreate = NavMesh.FindClosestEdge(hitInfo.point, out navHit, NavMesh.AllAreas);

            // 判断状态是否改变
            if (oldAllowCreate != allowCreate)
            {
                oldAllowCreate = allowCreate;
            }

            //NavMesh.
        }
    }

    /// <summary>
    /// 状态结束时制空ganeObj
    /// </summary>
    public override void OnDisactive()
    {
        if (null != gameObj)
        {
            gameObj = null;
        }
    }

    /// <summary>
    /// 左键放置单位
    /// </summary>
    public override void OnLeftButtonDown()
    {
        if (gameObj != null && SendRay(LayerMask.GetMask("Terrain")) != null && allowCreate)
        {
            gameObj.transform.position = hitInfo.point + Vector3.up * 0.2f;
            gameObj.GetComponent<Rigidbody>().isKinematic = false;
            gameObj = null;

        }
    }

    /// <summary>
    /// 右键取消单位放置
    /// </summary>
    public override void OnRightButtonDown()
    {
        if(gameObj != null)
        {
            gameObj.SetActive(false);
            gameObj = null;
        }
    }
}

/// <summary>
/// 用于创建装甲车攻击范围的状态
/// </summary>
public class MouseAttackRangeState : MouseState
{
    public override void OnLeftButtonDown()
    {
        if (SendRay(LayerMask.GetMask("RayCollider")) != null) { }
    }
}

/// <summary>
/// 用于创建防空火力范围的状态
/// </summary>
public class MouseAirDefenceState : MouseState
{
    public override void OnLeftButtonDown()
    {
        if (SendRay(LayerMask.GetMask("RayCollider")) != null) { }
    }
}

/// <summary>
/// 用于创建炮兵火力范围的状态
/// </summary>
public class MouseArtilleryRangeState : MouseState
{
    public override void OnLeftButtonDown()
    {
        if (SendRay(LayerMask.GetMask("RayCollider")) != null) { }
    }
}

/// <summary>
/// 用于控制单位攻击的状态
/// </summary>
public class MouseAttackState : MouseState
{
    /// <summary>
    /// 要进行攻击的单位
    /// </summary>
    /// <summary>
    /// 高亮代码
    /// </summary>
    private Transform seleObj;

    public override void OnActive(object para = null)
    {
        seleObj = null;
    }

    /// <summary>
    /// 左键点击控制单位攻击
    /// </summary>
    public override void OnLeftButtonDown()
    {
        // 选中需要发动攻击的单位
        if (seleObj == null && SendRay(LayerMask.GetMask("RayCollider")) != null)
        {
            seleObj = hitInfo.collider.transform.parent;

            return;
        }
        else if (seleObj != null)
        {
            // 攻击目标不为空时进行攻击判断
            if (SendRay(LayerMask.GetMask("RayCollider", "Terrain")))
            {
                if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Terrain"))
                {
                }
                else if (hitInfo.collider.transform.parent != seleObj)
                {
                }

                // 置空选中
                OnDisactive();
            }
        }
    }

    /// <summary>
    /// 右键取消选中单位
    /// </summary>
    public override void OnRightButtonDown()
    {
        OnDisactive();
    }

    /// <summary>
    /// 退出状态时,置空选中
    /// </summary>
    public override void OnDisactive()
    {
        seleObj = null;
    }
}

/// <summary>
/// 用于控制单位移动的状态
/// </summary>
public class MouseMoveState : MouseState
{
    /// <summary>
    /// 要进行移动的单位
    /// </summary>
    /// <summary>
    /// 高亮代码
    /// </summary>
    private Transform seleObj;

    public override void OnActive(object para = null)
    {
        seleObj = null;
    }

    /// <summary>
    /// 左键点击移动单位
    /// </summary>
    public override void OnLeftButtonDown()
    {
        // 点中单位
        if (SendRay(LayerMask.GetMask("RayCollider")) != null)
        {
            // 当前有选中单位
            if (seleObj != null)
            {
            }
            // 选中单位赋值
            seleObj = hitInfo.collider.transform.parent;
            
            return;
        }

        // 未击中单位
        if(seleObj != null && SendRay(LayerMask.GetMask("Terrain")) != null)
        {

            // 置空选中
            OnDisactive();
        }
    }

    /// <summary>
    /// 右键取消选中单位
    /// </summary>
    public override void OnRightButtonDown()
    {
        OnDisactive();
    }

    /// <summary>
    /// 退出状态时,置空选中
    /// </summary>
    public override void OnDisactive()
    {
        seleObj = null;
    }
}
