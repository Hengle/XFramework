﻿using System;
using System.Collections.Generic;
using UnityEngine;
using OnEventDelegate = System.Action<object, EventArgs>;

/// <summary>
/// 计时器
/// 最小处理间隔1毫秒
/// </summary>
public class Timer : EventDispatcher
{
    private readonly Dictionary<EventDispatchType, List<OnEventDelegate>> events = new Dictionary<EventDispatchType, List<OnEventDelegate>>();

    /// <summary>
    /// 是否执行
    /// </summary>
    private bool _isRunning;
    /// <summary>
    /// 已执行时间（每次满足运行间隔就会加这个）
    /// </summary>
    private float _useTime;
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
    public float TimeInterval { get; set; }
    /// <summary>
    /// 设置的运行次数
    /// </summary>
    public int RepeatCount { get; set; }

    /// <summary>
    /// <param name="interval">时间间隔，单位是毫秒</param>
    /// <param name="repeatCount">运行次数，一秒一次的话MaxValue可以执行68年</param>
    /// </summary>
    public Timer(float interval, int repeatCount = int.MaxValue)
    {
        RunTime = 0f;
        TimeInterval = Mathf.Max(interval, 1);  // 最小间隔为1毫秒
        RepeatCount = repeatCount;
        RegistEvent(EventAction);
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
                DispatchEvent(EventDispatchType.TIME_RUNCHANGE, _isRunning);
            }
        }
    }

    /// <summary>
    /// 每帧执行
    /// </summary>
    /// <param name="deltaTime"></param>
    public void Update(float deltaTime)
    {
        if (IsRunning && UseCount < RepeatCount)
        {
            RunTime += deltaTime;
            var f = TimeInterval/1000;
            while (RunTime - _useTime > f && UseCount < RepeatCount)
            {
                UseCount++;
                _useTime += f;
                DispatchEvent(EventDispatchType.TIMER);
            }
        }
        if (UseCount >= RepeatCount)
        {
            IsRunning = false;
        }
    }

    /// <summary>
    /// 添加事件
    /// </summary>
    /// <param name="type"></param>
    /// <param name="fun"></param>
    public void AddEventListener(EventDispatchType type, OnEventDelegate fun)
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
    public void RemoveEventListener(EventDispatchType type, OnEventDelegate fun)
    {
        if (!events.ContainsKey(type))
        {
            return;
            //events[type] = new List<OnEventDelegate>();
        }
        events[type].Remove(fun);
    }

    private void EventAction(object sender, EventArgs data)
    {
        Array arr = Enum.GetValues(typeof(EventDispatchType));
        
        for (var i = 0; i < arr.Length; i++)
        {
            EventDispatchType e = (EventDispatchType)arr.GetValue(i);
            if (data.eventType == e)
            {
                if (events.ContainsKey(e) && events[e].Count > 0)
                {
                    // 遍历列表挨个传入sender和data并执行
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
    /// 开始
    /// </summary>
    public void Start()
    {
        IsRunning = true;
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
    /// 释放对象
    /// </summary>
    public void Dispose()
    {
        Stop();
        ReSet();
        events.Clear();
    }




    /// <summary>
    /// 计时器管理
    /// 除了计时器以外其他类暂时不需要调用，以后需要再放到外面去
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
                    // unscaledDeltaTime和deltaTime一样，但是不受TimeScale影响
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
}