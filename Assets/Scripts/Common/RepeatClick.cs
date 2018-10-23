using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 鼠标连续点击
/// </summary>
public class RepeatClick : Singleton<RepeatClick>
{
    // Use this for initialization
    private int count = 0;
    private float time = 0;
    private float inteval = 0.2f;

    public event UnityAction OnceClickWithDelay;
    public event UnityAction OnceClickNoDelay;
    public event UnityAction DoubleClick;
    public event UnityAction ThriceClick;

    public RepeatClick()
    {
        MonoEvent.Instance.UPDATE += Update;
    }
    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnceClickNoDelay?.Invoke();

            count = count == 0 ? 1 : count;
            if (Time.time - time <= inteval)
            {
                count++;
            }
            //else
            //{

            //}
            time = Time.time;
        }
        if (Time.time - time > inteval && count != 0)
        {
            switch (count)
            {
                case 1:
                    OnceClickWithDelay?.Invoke();
                    break;
                case 2:
                    DoubleClick?.Invoke();
                    break;
                case 3:
                    ThriceClick?.Invoke();
                    break;
                default:
                    break;
            }
            count = 0;
        }
    }
}
