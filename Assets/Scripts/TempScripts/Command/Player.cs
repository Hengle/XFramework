using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    //角色移动事件
    public delegate void MoveDelegate(Vector3 pos);
    public event MoveDelegate MoveEvent;

    /// <summary>
    /// 角色移动
    /// </summary>
    public void Move(Vector3 pos)
    {
        MoveEvent?.Invoke(pos);
    }
}
