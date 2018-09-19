using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MouseStateType
{
    Defalt,
    _A,
    _B,
}

public class MouseState
{
    /// <summary>
    /// 被激活时
    /// </summary>
    /// <param name="para"></param>
    public virtual void OnActive(object para = null) { }
    /// <summary>
    /// 被禁用时
    /// </summary>
    public virtual void OnDisactive() { }
    /// <summary>
    /// 鼠标左键按下
    /// </summary>
    public virtual void OnLeftButtonDown() { }
    /// <summary>
    /// 鼠标左键抬起
    /// </summary>
    public virtual void OnLeftButtonUp() { }
    /// <summary>
    /// 鼠标右键按下
    /// </summary>
    public virtual void OnRightButtonDown() { }
    /// <summary>
    /// 鼠标右键抬起
    /// </summary>
    public virtual void OnRightButtonUp() { }
    /// <summary>
    /// 所有状态都会执行的
    /// </summary>
    public virtual void Update() { }
}

/// <summary>
/// 默认鼠标状态
/// </summary>
public class MouseState_Defalt : MouseState
{
    public override void OnDisactive()
    {
        Debug.Log("Disactive");
    }
}

public class MouseState_A : MouseState
{

    private GameObject gameObj = null;
    string classname = null;

    public override void OnActive(object para)
    {
        Debug.Log("A_Active");
    }

    public override void Update()
    {
        Debug.Log("MouseUpdate");
    }

    public override void OnDisactive()
    {
        if (null != gameObj)
        {
            gameObj.SetActive(false);
            gameObj = null;
        }
    }

    public override void OnLeftButtonDown()
    {
        Debug.Log("OnLeftButtonDown");
    }

    public override void OnRightButtonDown()
    {
        Debug.Log("OnRightButtonDown");
    }
}

public class MouseState_B : MouseState
{
    private string classname;

    public override void OnActive(object para)
    {
        classname = para as string;
    }

    public override void OnLeftButtonDown()
    {

    }
}
