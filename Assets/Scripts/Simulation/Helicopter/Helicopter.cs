using System.Collections;
using UnityEngine;
using XDEDZL.Pool;
using XDEDZL.OldFsm;

/// <summary>
/// 直升机状态
/// </summary>
public enum HState
{
    /// <summary>
    /// 飞行
    /// </summary>
    Flying,
    /// <summary>
    /// 上升
    /// </summary>
    Up,
    /// <summary>
    /// 下降
    /// </summary>
    Down,
    /// <summary>
    /// 停止
    /// </summary>
    Halt,
    /// <summary>
    /// 悬停
    /// </summary>
    Hover,
    /// <summary>
    /// 瞄准
    /// </summary>
    Aim,    
}

public class Helicopter : FSM
{

    /// <summary>
    /// 主旋翼
    /// </summary>
    public Transform mainRotor;
    /// <summary>
    /// 尾旋翼
    /// </summary>
    public Transform tailRotor;
    /// <summary>
    /// 发射器组
    /// </summary>
    public Transform[] launchs;
    /// <summary>
    /// 导弹
    /// </summary>
    public GameObject missile;
    /// <summary>
    /// 火箭弹
    /// </summary>
    public GameObject rocketSheel;


    /// <summary>
    /// 最大俯仰角
    /// </summary>
    private readonly float maxPitchAngle = 10;
    /// <summary>
    /// 最大横滚角
    /// </summary>
    private readonly float maxRollAngle = 30;
    /// <summary>
    /// 最大升降速度
    /// </summary>
    private readonly float maxLiftSpeed = 15;
    /// <summary>
    /// 旋转速度
    /// </summary>
    private readonly float rotateSpeed = 1;
    /// <summary>
    /// 最低起飞高度
    /// </summary>
    public float MinFlyHeght { get; private set; } = 80f;

    /// <summary>
    /// 主旋翼旋转速度
    /// </summary>
    private float mainRatorSpeed;
    /// <summary>
    /// 尾翼旋转速度
    /// </summary>
    private float tailRatorSpeed;
    /// <summary>
    /// 最大移动速度
    /// </summary>
    private float maxMoveSpeed;
    /// <summary>
    /// 当前移动速度
    /// </summary>
    private float currentMoveSpeed;
    /// <summary>
    /// 旋翼声音
    /// </summary>
    private AudioSource ratorSound;

    /// <summary>
    /// 移动后是否着陆
    /// </summary>
    private bool isLanding = false;
    public bool IsAim { get; private set; } = false;
    /// <summary>
    /// 武器种类
    /// </summary>
    private bool isMissile = false;
    /// <summary>
    /// 携弹量
    /// </summary>
    [SerializeField]
    private float weaponNumber = 8;
    /// <summary>
    /// 直升机当前状态
    /// </summary>
    private HState currentFlyState = HState.Halt;
    private bool isNeedAttack = false;

    public UnitInfo info = new UnitInfo();                   // 单位的拓展属性

    /// <summary>
    /// 初始化
    /// </summary>
    protected override void Initialize()
    {
        //targetPos = transform.position + transform.forward * 0.1f;
        ratorSound = gameObject.GetComponent<AudioSource>();
    }

    protected override void FSMUpdate()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, float.MaxValue, LayerMask.GetMask("Terrain")))
            {
                float x = Random.Range(-100f, 100f);
                float z = Random.Range(-100f, 100f);
                Vector3 pos = new Vector3(hit.point.x + x, hit.point.y, hit.point.z + z);
                pos.y = Terrain.activeTerrain.SampleHeight(pos);
                StartCoroutine(Delay(pos));
                //SetTargetPos(pos + Vector3.up * 50, 30, true);
            }
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            if(IsAim == true)
            {
                StartCoroutine(Attack());
            }
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            StartCoroutine(Attack());
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            SetEnemy(enemy,false);
        }
    }

    private IEnumerator Delay(Vector3 pos)
    {
        yield return new WaitForSeconds(Random.Range(0f, 0.5f));
        SetTargetPos(pos + Vector3.up * 50, 30, false);
    }

    protected override void FSMFixedUpdate()
    {
        RotateRotor();
        SwitchMove();
    }

    /// <summary>
    /// 旋翼旋转
    /// </summary>
    private void RotateRotor()
    {
        if (currentFlyState != HState.Halt)
        {
            mainRatorSpeed = Mathf.Lerp(mainRatorSpeed, 15, Time.deltaTime);
            tailRatorSpeed = Mathf.Lerp(tailRatorSpeed, 20, Time.deltaTime);
        }
        else
        {
            mainRatorSpeed = Mathf.Lerp(mainRatorSpeed, 0, Time.deltaTime);
            tailRatorSpeed = Mathf.Lerp(tailRatorSpeed, 0, Time.deltaTime);
        }
        mainRotor.Rotate(0, mainRatorSpeed, 0);
        tailRotor.Rotate(tailRatorSpeed, 0, 0);
    }

    /// <summary>
    /// 选择飞行状态
    /// </summary>
    private void SwitchMove()
    {
        switch (currentFlyState)
        {
            case HState.Flying:
                FlyingState();
                break;
            case HState.Up:
                UpState();
                break;
            case HState.Down:
                DownState();
                break;
            case HState.Hover:
                HoverState();
                break;
            case HState.Halt:
                break;
            case HState.Aim:
                AimState();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 飞行状态的移动
    /// </summary>
    private void FlyingState()
    {
        YawRotate(targetPos, maxRollAngle); // 偏航

        float y = (targetPos - transform.position).normalized.y; // 获取y轴移动方向
        currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, maxMoveSpeed, Time.deltaTime);
        transform.Translate(new Vector3(transform.forward.x, y, transform.forward.z) * currentMoveSpeed * Time.deltaTime, Space.World); // x，z轴方向用直升机自身方向

        PitchRotate(maxPitchAngle);   // 俯仰 飞行状态必俯仰

        // 抵达目标点后更改状态
        if (Vector3.Distance(transform.position, targetPos) < 5)
        {
            currentMoveSpeed = 0;
            if (isLanding)
            {
                currentFlyState = HState.Down;
            }
            else
            {
                currentFlyState = HState.Hover;
            }
        }
    }

    /// <summary>
    /// 悬停状态
    /// </summary>
    private void HoverState()
    {
        PitchRotate(0);
        RollRotate(0);
    }

    /// <summary>
    /// 降落状态
    /// </summary>
    private void DownState()
    {
        PitchRotate(0);  // 俯仰修正
        RollRotate(0);   // 横滚修正
        float height = transform.position.y - Terrain.activeTerrain.SampleHeight(transform.position);
        if (height < 0.1)
        {
            ratorSound.Stop();
            currentFlyState = HState.Halt;
            return;
        }

        transform.Translate(Vector3.down * Time.deltaTime * Mathf.Lerp(Mathf.Min(height, maxLiftSpeed), maxLiftSpeed, Time.deltaTime));// 起飞速度以离地高度与飞行速度做插值

        // 控制旋翼声音
        ratorSound.volume = height / 10;
    }

    /// <summary>
    /// 起飞状态
    /// </summary>
    private void UpState()
    {
        float height = transform.position.y - Terrain.activeTerrain.SampleHeight(transform.position);
        if (height > MinFlyHeght)
        {
            currentFlyState = HState.Flying;
            return;
        }
        //YawRotate(targetPos, maxRollAngle);

        // 起飞速度以离地高度与飞行速度做插值
        transform.Translate(Vector3.up * Time.deltaTime * Mathf.Lerp(height, maxLiftSpeed, Time.deltaTime));

        if (!ratorSound.isPlaying)
        {
            ratorSound.Play();
        }
        // 控制旋翼声音
        ratorSound.volume = height / 10;
    }

    /// <summary>
    /// 攻击状态
    /// </summary>
    private void AimState()
    {
        if (enemy != null || attackPos != default(Vector3))
        {
            // 俯仰和偏航到指定角度
            Vector3 dir = enemy ? enemy.position - (launchs[0].position + launchs[1].position) / 2 :
                attackPos - (launchs[0].position + launchs[1].position) / 2;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime);
            if (Vector3.Angle(dir, transform.forward) < 0.2 && isNeedAttack)
            {
                StartAttack();      // 角度完成偏转，开始攻击
                isNeedAttack = false;
            }
        }
    }

    /// <summary>
    /// 偏航 机身旋转
    /// </summary>
    private void YawRotate(Vector3 _targetPos, float rollAngle)
    {
        float height = transform.position.y - Terrain.activeTerrain.SampleHeight(transform.position); // 获取当前物体相对地面高度
        if (height < 5)
        {
            return;
        }

        Vector3 dir = _targetPos - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(dir);

        float angle = transform.eulerAngles.y - targetRotation.eulerAngles.y;
        if (angle < 0) angle += 360;

        if (angle > rotateSpeed && angle < 180)
        {
            transform.Rotate(0f, -rotateSpeed, 0f, Space.World);
            RollRotate(rollAngle);
        }
        else if (angle < 360 - rotateSpeed && angle > 180)
        {
            transform.Rotate(0f, rotateSpeed, 0f, Space.World);
            RollRotate(-rollAngle);
        }
        else
        {
            // 偏航完成
            if (currentFlyState == HState.Flying)
            {
                RollRotate(0);
            }
        }
    }

    /// <summary>
    /// 横滚 左倾右倾
    /// </summary>
    /// <param name="target_z">横滚目标角度</param>
    private void RollRotate(float target_z)
    {
        Vector3 currentEuler = transform.eulerAngles;
        Quaternion from = Quaternion.Euler(transform.eulerAngles);
        Quaternion to = Quaternion.Euler(new Vector3(currentEuler.x, currentEuler.y, target_z));
        transform.rotation = Quaternion.Lerp(from, to, Time.deltaTime);
    }

    /// <summary>
    /// 俯仰 前倾
    /// </summary>
    /// <param name="target_x">俯仰目标角度</param>
    private void PitchRotate(float target_x)
    {
        Vector3 currentEuler = transform.eulerAngles;
        Quaternion from = Quaternion.Euler(transform.eulerAngles);
        Quaternion to = Quaternion.Euler(new Vector3(target_x, currentEuler.y, currentEuler.z));
        transform.rotation = Quaternion.Lerp(from, to, Time.deltaTime);
    }

    /// <summary>
    /// 初始化火力
    /// </summary>
    /// <param name="_launch">发射架</param>
    /// <param name="_target">打击目标</param>
    /// <param name="_Missile">导弹预制</param>
    /// <param name="_velocity">导弹速度</param>
    private void InitFirePower(Transform _launch, float _velocity = 400f)
    {
        int index;
        if (isMissile)
        {
            //实例化导弹实体
            index = Random.Range(2, 4);
            GameObject _missile = Singleton<GameObjectFactory>.Instance.Instantiate(missile.name, launchs[index].position, _launch.rotation);
            if (enemy)
            {
                //_missile.GetComponent<Missiles>().OnMissileInit(enemy);
            }
            else
            {
                //_missile.GetComponent<Missiles>().OnMissileInit(attackPos);
            }
        }
        else
        {
            //实例化火箭弹
            index = Random.Range(0, 2);
            //GameObject _rocketSheel = Instantiate(rocketSheel, launchs[index].position, _launch.rotation);
            GameObject _rocketSheel = Singleton<GameObjectFactory>.Instance.Instantiate(rocketSheel.name, launchs[index].position, _launch.rotation);
            //_rocketSheel.GetComponent<Bullet>();
        }

        launchs[index].Find("FireEffect").GetComponent<ParticleSystem>().Play();
        launchs[index].GetComponent<AudioSource>().Play();
    }

    /// <summary>
    /// 开始攻击
    /// </summary>
    private IEnumerator Attack()
    {
        if (launchs.Length > 0)
        {
            for (int i = 0; i < weaponNumber; i++)
            {
                InitFirePower(launchs[0]);
                yield return new WaitForSeconds(0.2f);
            }
            yield return new WaitForSeconds(0.2f);
            enemy = null;
            attackPos = default(Vector3);
            currentFlyState = HState.Hover;
        }
    }
    public void StartAttack()
    {
        StartCoroutine(Attack());
    }

    /// <summary>
    /// 设置目标点
    /// </summary>
    /// <param name="_pos"></param>
    /// <param name="_speed"></param>
    /// <param name="_isLanding"></param>
    public void SetTargetPos(Vector3 _pos, float _speed, bool _isLanding)
    {
        targetPos = _pos;
        maxMoveSpeed = _speed;
        isLanding = _isLanding;
        IsAim = false;
        switch (currentFlyState)
        {
            case HState.Down:
            case HState.Halt:
                currentFlyState = HState.Up;
                break;
            case HState.Hover:
            case HState.Aim:
                currentFlyState = HState.Flying;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 设置攻击目标
    /// </summary>
    /// <param name="_enemy"></param>
    /// <param name="isMissile">武器是否是导弹</param>
    public void SetEnemy(Transform _enemy, bool _isMissile = true)
    {
        if(launchs.Length <= 0)
        {
            return;
        }
        enemy = _enemy;
        isMissile = _isMissile;
        if (currentFlyState == HState.Hover || currentFlyState == HState.Flying)
        {
            isNeedAttack = true;
            currentFlyState = HState.Aim;
        }
    }

    /// <summary>
    /// 设置攻击目标点
    /// </summary>
    /// <param name="_enemy"></param>
    /// <param name="isMissile">武器是否是导弹</param>
    public void SetEnemy(Vector3 _attackPos, bool _isMissile = true)
    {
        if (launchs.Length <= 0)
        {
            return;
        }
        attackPos = _attackPos;
        isMissile = _isMissile;
        switch (currentFlyState)
        {
            case HState.Flying:
            case HState.Hover:
            case HState.Aim:
                isNeedAttack = true;
                currentFlyState = HState.Aim;
                break;
            default:
                break;
        }
    }
}
