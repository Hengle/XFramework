using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏时间管理
/// </summary>
public class GameTime : Singleton<GameTime>
{
    /// <summary>
    /// 当前时域
    /// </summary>
    public TimeDomain CurrentDomain { get; private set; }
    /// <summary>
    /// 第几天
    /// </summary>
    public int DayIndex { get; private set; }
    /// <summary>
    /// 年
    /// </summary>
    public int Year { get; private set; }
    /// <summary>
    /// 月
    /// </summary>
    public int Month { get; private set; }
    /// <summary>
    /// 日
    /// </summary>
    public int Day{ get; private set; }
    /// <summary>
    /// 时
    /// </summary>
    public int Hour { get; private set; }
    /// <summary>
    /// 分
    /// </summary>
    public int Minute { get; private set; }
    /// <summary>
    /// 秒
    /// </summary>
    public int Second { get; private set; }

    /// <summary>
    /// 计时器
    /// </summary>
    private float timer = 0;

    private GameTime()
    {
        Debug.Log("hahaha");
    }

    /// <summary>
    /// 更新游戏时间
    /// </summary>
    private void UpdateTime()
    {
        timer += Time.deltaTime;
        if(timer >= 1000)
        {
            Second++;
            timer = 0;
            if(Second == 60)
            {
                Minute++;
                Second = 0;
                if(Minute == 60)
                {
                    Hour++;
                    CalculateDomain(Hour);
                    Minute = 0;
                    if(Hour == 24)
                    {
                        Day++;
                        Hour = 0;
                        if (Day == 1 + CalculateMonthDay(Year, Month))
                        {
                            Month++;
                            Day = 1;
                            if(Month == 13)
                            {
                                Year++;
                                Month = 1;
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 时间跳转
    /// </summary>
    public void SkipTime(int year, int month, int day, int hour = 0, int minute = 0)
    {
        Year = year;
        Month = month;
        Day = day;
        Hour = hour;
        Minute = minute;
        timer = 0;
    }

    /// <summary>
    /// 计算当月有多少天
    /// </summary>
    private int CalculateMonthDay(int year,int month)
    {
        switch (month)
        {
            case 2:
                    return (year % 4) == 0 ? 29 : 28;
            case 4:
            case 6:
            case 9:
            case 11:
                return 30;
            default:     //1,3,5,7,8,10,12
                return 31;
        }
    }

    /// <summary>
    /// 计算当前时域
    /// </summary>
    private void CalculateDomain(int hour)
    {
        CurrentDomain = TimeDomain.Day;
    }
}


/// <summary>
/// 时域
/// </summary>
public enum TimeDomain
{
    Morning,
    Day,
    Evening,
    Night,
}