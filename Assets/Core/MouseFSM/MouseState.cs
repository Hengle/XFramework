using UnityEngine;

public class BaseMouseState
{
    /// <summary>
    /// 是否已经初始化
    /// </summary>
    public bool isInited { get; private set; }

    public BaseMouseState() { isInited = false; }
    /// <summary>
    /// 状态初始化，只会执行一次
    /// </summary>
    public virtual void OnInit() { isInited = true; }
    /// <summary>
    /// 状态激活时
    /// </summary>
    /// <param name="para"></param>
    public virtual void OnEnable() { }
    /// <summary>
    /// 状态结束时
    /// </summary>
    public virtual void OnDisable() { }
    /// <summary>
    /// 左键按下
    /// </summary>
    public virtual void OnLeftButtonDown() { }
    /// <summary>
    /// 左键保持按下状态
    /// </summary>
    public virtual void OnLeftButtonHold() { }
    /// <summary>
    /// 左键抬起
    /// </summary>
    public virtual void OnLeftButtonUp() { }
    /// <summary>
    /// 右键按下
    /// </summary>
    public virtual void OnRightButtonDown() { }
    /// <summary>
    /// 右键保持按下状态
    /// </summary>
    public virtual void OnRightButtonHold() { }
    /// <summary>
    /// 右键抬起
    /// </summary>
    public virtual void OnRightButtonUp() { }
    /// <summary>
    /// 每帧调用
    /// </summary>
    public virtual void Update() { }
}