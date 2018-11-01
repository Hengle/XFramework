using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 炮台状态机
/// </summary>
public enum TurretState
{
    AimAt,      // 瞄准
    Reset,      // 复位
    Fire,       // 开火
    Idle        // 闲置
}

/// <summary>
/// 坦克控制
/// </summary>
public class Tank : ArmoredCar
{
    public float speed;                 // 临时变量
    /// <summary>
    /// 炮塔
    /// </summary>
    public Transform turret;
    /// <summary>
    /// 炮管
    /// </summary>
    public Transform gun;
    /// <summary>
    /// 发射器组
    /// </summary>
    public Transform[] launch;
    /// <summary>
    /// 弹药
    /// </summary>
    public GameObject ammunition;

    /// <summary>
    /// 炮塔的目标角度
    /// </summary>
    private float turretRollTarget;
    /// <summary>
    /// 炮管的目标角度
    /// </summary>
    private float gunRollTarget;
    
    /// <summary>
    /// 炮塔转动速度
    /// </summary>
    public float turretRotSpeed = 1;
    /// <summary>
    /// 炮管转动速度
    /// </summary>
    public float gunRotSpeed = 1;

    /// <summary>
    /// 炮管俯仰下限
    /// </summary>
    public float downRoll = 10;
    /// <summary>
    /// 炮管俯仰上限
    /// </summary>
    public float upRoll = 15;

    /// <summary>
    /// 载具炮塔旋转委托
    /// </summary>
    protected UnityAction TurretRotationDel;
    /// <summary>
    /// 载具炮管旋转委托
    /// </summary>
    protected UnityAction GunRotationDel;

    /// <summary>
    /// 炮塔是否需要瞄准
    /// </summary>
    private bool isTurretMove = false;
    /// <summary>
    /// 炮管是否需要瞄准
    /// </summary>
    private bool isGunMove = false;
    /// <summary>
    /// 弹药发射轮数
    /// </summary>
    private int fireNum = 0;
    /// <summary>
    /// 攻击目标点
    /// </summary>
    private Vector3 enemyPos;

    /// <summary>
    /// 炮塔当前状态
    /// </summary>
    private TurretState currentTurState = TurretState.Idle;

    private void Awake()
    {
        if (turret != null)
        {
            TurretRotationDel += TurretRotation;
        }
        if (gun != null)
        {
            GunRotationDel += GunRotation;
        }

        wheels = wheelsRoot.GetComponentsInChildren<Transform>(); // 获取所有轮子的Transform

        rig = GetComponent<Rigidbody>();
        ammunition = Resources.Load("SpecialEffectPrefabs/Missile1") as GameObject;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, float.MaxValue, LayerMask.GetMask("Terrain")))
            {
                float x = Random.Range(-50, 50);
                float z = Random.Range(-50, 50);
                Vector3 pos = new Vector3(hit.point.x + x, hit.point.y, hit.point.z + z);
                pos.y = Terrain.activeTerrain.SampleHeight(pos);
                SetPosAndSpeed(pos, speed);
            }
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            StartCoroutine("AttackIE", 1);
        }
    }

    protected override void FixedUpdate()
    {
        // 瞄准与击发控制
        SwitchTurret();

        // 移动控制
        if (currentTurState != TurretState.Fire)
        {
            base.FixedUpdate();
        }
    }

    private void SwitchTurret()
    {
        switch (currentTurState)
        {
            case TurretState.AimAt:
                StateChange();
                break;
            case TurretState.Reset:
                StateChange();
                break;
            case TurretState.Fire:
                FireState();
                break;
            case TurretState.Idle:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 瞄准与复位的状态切换
    /// </summary>
    private void StateChange()
    {
        TurretRotationDel?.Invoke();
        GunRotationDel?.Invoke();
        if (!isGunMove && !isTurretMove)
        {
            if (currentTurState == TurretState.AimAt)
            {
                currentTurState = TurretState.Fire;
            }
            else if (currentTurState == TurretState.Reset)
            {
                currentTurState = TurretState.Idle;
            }
        }
    }

    /// <summary>
    /// 复位炮塔炮管
    /// </summary>
    private void ResetState()
    {
        turretRollTarget = transform.eulerAngles.y;
        gunRollTarget = transform.eulerAngles.x;
        isTurretMove = true;
        isGunMove = true;
    }

    /// <summary>
    /// 开火状态
    /// </summary>
    private void FireState()
    {
        if(fireNum != 0)
        {
            StartCoroutine("AttackIE", fireNum);
            fireNum = 0;
        }
    }

    /// <summary>
    /// 炮塔旋转
    /// </summary>
    private void TurretRotation()
    {
        if (!isTurretMove || turret == null)
            return;

        isGunMove = true; // 炮塔旋转时要持续调整炮管，避免在斜坡上的误差

        //归一化角度
        float angle = turret.eulerAngles.y - turretRollTarget;
        if (angle < 0) angle += 360;

        if (angle > turretRotSpeed && angle < 180)
            turret.Rotate(0f, -turretRotSpeed, 0f);
        else if (angle < 360 - turretRotSpeed && angle > 180)
            turret.Rotate(0f, turretRotSpeed, 0f);
        else
            isTurretMove = false;
    }

    /// <summary>
    /// 炮管旋转
    /// </summary>
    private void GunRotation()
    {
        if (!isGunMove || gun == null)
            return;

        //归一化角度
        float angle = gun.eulerAngles.x - gunRollTarget;

        if (angle < -180) angle += 360;
        if (angle > 180) angle -= 360;

        // 往上 angle为+
        if (angle > gunRotSpeed)
        {
            float localx = gun.localEulerAngles.x;
            if (localx - gunRotSpeed > downRoll && localx - gunRotSpeed < 360 - upRoll)
            {
                isGunMove = false;
                return;
            }
            gun.Rotate(-gunRotSpeed, 0, 0);  
        }
        // 往下 angle为-
        else if (angle < -gunRotSpeed)
        {
            float localx = gun.localEulerAngles.x;
            if (localx + gunRotSpeed > downRoll && localx + gunRotSpeed < 360 - upRoll)
            {
                isGunMove = false;
                return;
            }
            gun.Rotate(gunRotSpeed, 0, 0);
        }
        else
            isGunMove = false;
    }

    /// <summary>
    /// 初始化炮弹
    /// </summary>
    /// <param name="_fireNum">弹药发射轮数</param>
    /// <returns></returns>
    IEnumerator AttackIE(int _fireNum)
    {
        if (ammunition == null)
        {
            StopCoroutine("AttackIE");
            yield return null;
        }

        for (int i = 0; i < _fireNum; i++)
        {
            for (int j = 0; j < launch.Length; j++)
            {
                InitFirePower(launch[j], ammunition);
                yield return new WaitForSeconds(0.2f);
            }
            yield return new WaitForSeconds(2);
        }
        enemy = null;
        currentTurState = TurretState.Reset;
        ResetState();

        yield return null;
    }

    /// <summary>
    /// 发射炮弹
    /// </summary>
    /// <param name="_launch">发射器</param>
    /// <param name="_target">目标</param>
    /// <param name="_Missile">弹药</param>
    /// <param name="_velocity">速度</param>
    protected void InitFirePower(Transform _launch, GameObject _Missile)
    {
        if (enemy)
        {
            //Missiles missile = Singleton<GameObjectFactory>.Instance.Instantiate(_Missile.name, _launch.position, _launch.rotation).GetComponent<Missiles>();
            // missile.selfCamp = info.camp;        // 设置弹药阵营（当前不设，为了能攻击）
            //missile.OnMissileInit(enemy);            
        }
        else
        {

            //Missiles missile = Singleton<GameObjectFactory>.Instance.Instantiate(_Missile.name, _launch.position, _launch.rotation).GetComponent<Missiles>();
            //missile.selfCamp = info.camp;
            //missile.OnMissileInit(enemyPos);
        }
        // 播放击发特效
        _launch.Find("FireEffect").GetComponent<ParticleSystem>().Play();
        _launch.GetComponent<AudioSource>().Play();
    }

    /// <summary>
    /// 设置目标
    /// </summary>
    /// <param name="target">目标</param>
    /// <param name="_fireNum">发射轮数</param>
    public override void SetEnemy(Transform target, int _fireNum = 1)
    {
        if (gun == null) return;
        // 获取朝向向量
        Vector3 rot = Quaternion.LookRotation(target.position - gun.position).eulerAngles;
        turretRollTarget = rot.y;
        gunRollTarget = rot.x;
        isTurretMove = true;
        isGunMove = true;
        enemy = target;

        if (currentTurState == TurretState.Fire)
        {
            StopCoroutine("AttackIE");      // 停止当前的开火
        }
        fireNum = _fireNum;
        currentTurState = TurretState.AimAt;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="target"></param>
    /// <param name="_fireNum"></param>
    public override void SetEnemy(Vector3 targetPosition, int _fireNum = 1)
    {
        if (gun == null) return;
        // 获取朝向向量
        Vector3 rot = Quaternion.LookRotation(targetPosition - gun.position).eulerAngles;
        turretRollTarget = rot.y;
        gunRollTarget = rot.x;
        isTurretMove = true;
        isGunMove = true;
        enemyPos = targetPosition;

        if (currentTurState == TurretState.Fire)
        {
            StopCoroutine("AttackIE");      // 停止当前的开火
        }
        fireNum = _fireNum;
        currentTurState = TurretState.AimAt;
    }

    void OnDrawGizmos()
    {
        myPath.DrawWaypoints();
    }
}