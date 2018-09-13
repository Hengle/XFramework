using System;
using System.Collections.Generic;
using UnityEngine;
using OnEventDelegate = System.Action<object, EventArgs>;

/// <summary>
/// 计时器管理
/// </summary>
public class TimerManager : Singleton<TimerManager>
{
    private readonly List<Timer> _timers = new List<Timer>();

    public TimerManager()
    {
        MonoEvent.Instance.LATEUPDATE += LateUpdate;
    }

    private void LateUpdate()
    {
        for (var i = 0; i < _timers.Count; i++)
        {
            if (_timers[i].IsRunning)
            {
                _timers[i].Update(Time.unscaledDeltaTime);
            }
        }
    }

    public void AddTimer(Timer timer)
    {
        if (_timers.Contains(timer) == false)
        {
            _timers.Add(timer);
        }
    }

    public void RemoveTimer(Timer timer)
    {
        if (_timers.Contains(timer))
        {
            _timers.Remove(timer);
        }
    }
}

/// <summary>
/// 计时器
/// 最小处理间隔1毫秒
/// </summary>
public class Timer : EventDispatcher
{
    private readonly Dictionary<EventDispatchType, List<OnEventDelegate>> events = new Dictionary<EventDispatchType, List<OnEventDelegate>>();

    private bool _isRunning;
    private float _useTime; //已执行时间（每次满足运行间隔就会加这个）

    /// <summary>
    /// <param name="delay">时间间隔</param>
    /// <param name="repeatCount">运行次数</param>
    /// </summary>
    public Timer(float delay, int repeatCount = int.MaxValue)
    {
        RunTime = 0f;
        Delay = Mathf.Max(delay, 1);
        RepeatCount = repeatCount;
        RegistEvent(eventAction);
    }

    /// <summary>
    /// 是否运行中
    /// </summary>
    public bool IsRunning
    {
        get { return _isRunning; }
        set
        {
            if (_isRunning != value)
            {
                _isRunning = value;
                if (_isRunning)
                {
                    TimerManager.Instance.AddTimer(this);
                }
                else
                {
                    TimerManager.Instance.RemoveTimer(this);
                }
                DispatchEvent(EventDispatchType.EVENT_TIME_RUNCHANGE, _isRunning);
            }
        }
    }

    /// <summary>
    /// 运行时间
    /// </summary>
    public float RunTime { get; private set; }

    /// <summary>
    /// 已运行次数
    /// </summary>
    public int UseCount { get; private set; }

    /// <summary>
    /// 运行间隔
    /// </summary>
    public float Delay { get; set; }

    /// <summary>
    /// 设置的运行次数
    /// </summary>
    public int RepeatCount { get; set; }

    /// <summary>
    /// 开始
    /// </summary>
    public void Start()
    {
        IsRunning = true;
    }

    public void Update(float deltaTime)
    {
        if (IsRunning && UseCount < RepeatCount)
        {
            RunTime += deltaTime;
            var f = Delay/1000;
            while (RunTime - _useTime > f && UseCount < RepeatCount)
            {
                UseCount++;
                _useTime += f;
                DispatchEvent(EventDispatchType.EVENT_TIMER);
            }
        }
        if (UseCount >= RepeatCount)
        {
            IsRunning = false;
        }
    }

    /// <summary>
    /// 停止
    /// </summary>
    public void Stop()
    {
        IsRunning = false;
    }

    /// <summary>
    /// 重置
    /// </summary>
    public void ReSet()
    {
        IsRunning = false;
        RunTime = 0f;
        _useTime = 0f;
        UseCount = 0;
    }

    /// <summary>
    /// 添加事件
    /// </summary>
    /// <param name="type"></param>
    /// <param name="fun"></param>
    public void addEventListener(EventDispatchType type, OnEventDelegate fun)
    {
        if (!events.ContainsKey(type))
        {
            events[type] = new List<OnEventDelegate>();
        }
        events[type].Add(fun);
    }

    /// <summary>
    /// 移除事件
    /// </summary>
    /// <param name="type"></param>
    /// <param name="fun"></param>
    public void removeEventListener(EventDispatchType type, OnEventDelegate fun)
    {
        if (!events.ContainsKey(type))
        {
            return;
            //events[type] = new List<OnEventDelegate>();
        }
        events[type].Remove(fun);
    }

    private void eventAction(object sender, EventArgs data)
    {
        var arr = Enum.GetValues(typeof (EventDispatchType));
        for (var i = 0; i < arr.Length; i++)
        {
            var e = (EventDispatchType) arr.GetValue(i);
            if (data.eventType == e)
            {
                if (events.ContainsKey(e) && events[e].Count > 0)
                {
                    events[e].ForEach(fun =>
                    {
                        try
                        {
                            fun(sender, data);
                        }
                        catch
                        {
                            Debug.LogWarning(fun.Target.ToString());
                        }
                    });
                }
            }
        }
    }

    /// <summary>
    /// 释放对象
    /// </summary>
    public void Dispose()
    {
        Stop();
        ReSet();
        events.Clear();
    }
}