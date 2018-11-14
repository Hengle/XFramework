using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 载具基类
/// </summary>
public class ArmoredCar : MonoBehaviour {
    protected new Transform transform;
    public Transform enemy;                 // 攻击目标 protected
    public Transform tracksRoot;            // 履带
    public Transform wheelsRoot;            // 轮子的父物体
    protected float moveSpeed;              // 载具移动速度
    public float rotateSpeed;               // 载具转向速度
    protected Vector3 targetPos;            // 运动目标点
    protected Vector3 currentTargetPos;     // 当前运动目标点
    protected Transform[] wheels;           // 所有轮子
    protected GameObject noGameobject;      // 用于旋转履带不在场景中出现的物体，有别的方法后删除
    protected Rigidbody rig;

    private bool isWheelRotate = false;     // 轮子的旋转比较占用性能，默认关闭，需要时打开

    private AudioSource engineSource;

    protected UnityAction TrackRotateDel;

    protected MyPath myPath = new MyPath();

    public UnitInfo info = new UnitInfo();                   // 单位的拓展属性

    protected void Start()
    {
        transform = GetComponent<Transform>();
        currentTargetPos = transform.position + transform.forward * 0.1f;
        if (tracksRoot != null)
        {
            noGameobject = new GameObject("empty");
            noGameobject.transform.SetParent(transform);
            TrackRotateDel += TrackRotate;
        }
        engineSource = gameObject.GetComponent<AudioSource>();
    }

    protected virtual void FixedUpdate()
    {
        RotateObj();
        LimitX_Z();
        MoveObj();
    }

    /// <summary>
    /// 自身载具移动
    /// </summary>
    protected virtual void MoveObj()
    {
        if(currentTargetPos == Vector3.zero)
        {
            Debug.LogError("cnm");
        }
        // 通过寻路系逐点到达
        if (!myPath.isFinish)
        {
            //currentTargetPos = path.WayPoint;
            if (myPath.IsReach(transform))
            {
                myPath.NextWaypoint();
                currentTargetPos = myPath.WayPoint;
            }

            transform.Translate(Vector3.forward * moveSpeed * Time.fixedDeltaTime);
            if (isWheelRotate)
                WheelRotate();      // 轮子的旋转
        }
        else if(engineSource != null && engineSource.isPlaying)
        {
            engineSource.Stop();    // 停止音效
        }
    }

    /// <summary>
    /// 自身载具旋转
    /// </summary>
    protected virtual void RotateObj()
    {
        // 获取朝向目标的四元数
        Vector3 dir = currentTargetPos - transform.position;

        if(Vector3.SqrMagnitude(dir.normalized - transform.forward) < 0.1)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(dir);   

        // 控制偏航的目标差值，并控制在 0-360 之间
        float angle = transform.eulerAngles.y - targetRotation.eulerAngles.y;
        if (angle < 0) angle += 360;

        // 根据方向控制旋转角小于180
        if (angle > rotateSpeed && angle < 180)
            transform.Rotate(0f, -rotateSpeed, 0f);
        else if (angle < 360 - rotateSpeed && angle > 180)
            transform.Rotate(0f, rotateSpeed, 0f);
    }

    /// <summary>
    /// 将载具X Z轴的角度控制在一定范围内
    /// </summary>
    protected virtual void LimitX_Z()
    {
        Vector3 euler = transform.eulerAngles;
        if (euler.x > 31 && euler.x <= 180)
        {
            euler.x = 31;
            transform.eulerAngles = euler;
        }
        else if(euler.x > 180 && euler.x < 329)
        {
            euler.x = 329;
            transform.eulerAngles = euler;
        }
        if (euler.z > 31 && euler.z <= 180)
        {
            euler.z = 31;
            transform.eulerAngles = euler;
        }
        else if (euler.z > 180 && euler.z < 329)
        {
            euler.z = 329;
            transform.eulerAngles = euler;
        }
    }

    /// <summary>
    /// 旋转轮子
    /// </summary>
    private void WheelRotate()
    {
        for (int i = 1; i < wheels.Length; i++)
        {
            wheels[i].Rotate(moveSpeed / 0.7f, 0, 0);
        }
        if (TrackRotateDel != null)
        {
            // 为避免万向锁问题，将x轴的旋转用空物体转为y轴，以此来控制履带旋转
            noGameobject.transform.Rotate(0, moveSpeed / 0.7f, 0);
            TrackRotateDel();
        }
    }

    /// <summary>
    /// 旋转履带(委托控制)
    /// </summary>
    private void TrackRotate()
    {
        if (tracksRoot == null)
            return;
        float offset = 0;
        // 根据空物体的旋转来计算履带贴图的偏移
        if (noGameobject != null)
        {
            offset = -noGameobject.transform.localEulerAngles.y / 90f;
        }

        // 获取两条履带，并对其贴图进行偏移处理
        for (int i = 0; i < 2; i++)
        {
            MeshRenderer mr = tracksRoot.GetChild(i).GetComponent<MeshRenderer>();
            if (mr == null) return;
            Material mtl = mr.material;
            mtl.mainTextureOffset = new Vector2(0, offset);
        }
    }

    #region 外部调用接口
    /// <summary>
    /// 设置目标位置与移动速度
    /// </summary>
    /// <param name="_targetPos"></param>
    /// <param name="_speed"></param>
    public virtual void SetPosAndSpeed(Vector3 _targetPos, float _speed)
    {
        myPath.InitByAStar(transform.position, _targetPos, OnCompletePath);  // 计算路径
        moveSpeed = _speed;
        if (engineSource != null)
        {
            engineSource.Play();
        }
    }
    /// <summary>
    /// 路径计算完成后调用
    /// </summary>
    private void OnCompletePath(Pathfinding.Path path)
    {
        List<Vector3> p = new List<Vector3>(); 

        // 路径计算成功
        if (!path.error)
        {
            for (int i = 0; i < path.vectorPath.Count; i++)
            {
                // 根据不同地图的NodeSize决定隔几个点取一个路径点
                //if (i % 3 == 0)
                //{
                p.Add(path.vectorPath[i]);
                //}
            }
            myPath.ResetPath(p.ToArray());
            currentTargetPos = myPath.WayPoint;
        }
        else
        {
            p.Add(((Pathfinding.ABPath)path).originalEndPoint);
            myPath.ResetPath(p.ToArray());
            currentTargetPos = myPath.WayPoint;
            Debug.Log("A*路径计算错误");
        }
        
    }

    /// <summary>
    /// 设置攻击目标
    /// </summary>
    /// <param name="_enemy">敌人</param>
    public virtual void SetEnemy(Transform _enemy, int _fireNum) { }

    /// <summary>
    /// 设置攻击目标地点
    /// </summary>
    /// <param name="targetPosition">攻击目标地点</param>
    public virtual void SetEnemy(Vector3 targetPosition, int _fireNum) { }

    #endregion
}
