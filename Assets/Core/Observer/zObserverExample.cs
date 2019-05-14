﻿using UnityEngine;
using XDEDZL;

/*
 * 以战斗系统为例
 */


/// <summary>
/// 挂在场景中
/// </summary>
public class zObserverExample : MonoBehaviour
{
    BattleSystem bat;
    PlayerDataMgr pd;

    void Start()
    {
        // 正常使用时观察者和被观察者都不应当是直接New
        bat = new BattleSystem();
        pd = new PlayerDataMgr();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            bat.BattleWin();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            bat.BattleLose();
        }
    } 
}

#region 战斗模块  
/*
 * 一个可被观察者观察的主题分为三个部分
 * 1.主题管理类
 * 2.派生数据类
 * 3.可被观察的事件类型
 */

public class BattleSystem
{
    /// <summary>
    /// data由各个主题各自管理，可能存在多分，也可能全局只有一个
    /// </summary>
    BattleData btData = new BattleData();

    public void BattleWin()
    {
        // 处理战斗胜利要做的事情
        // 。。。。。。。。。。。。。

        // 通知所有观察者并传递数据
        Game.ObserverModel.Notify(btData, (int)BattleDataType.Win);
    }

    public void BattleLose()
    {
        // 处理战斗失败要做的事情
        // 。。。。。。。。。。。。。

        Game.ObserverModel.Notify(btData, (int)BattleDataType.Lose);
    }
}

public class BattleData : BaseData
{
    public int battleCount;
    public int loseCount;
    public int winCount;

    public BattleData() : base()
    {
        battleCount = 5;
        loseCount = 3;
        winCount = 2;
    }

    public override DataType dataType
    {
        get
        {
            return DataType.BATTLE;
        }
    }
}

public enum BattleDataType
{
    Win = 0,
    Lose = 1,
}

#endregion

/// <summary>
/// 观察者
/// </summary>
public class PlayerDataMgr : IObserver
{
    // 此类可能是要继承Mono的，如果是，则在start中注册
    public PlayerDataMgr()
    {
        Game.ObserverModel.AddListener(DataType.BATTLE, this);
    }

    // 在合适的时候移除观察者
    ~PlayerDataMgr()
    {
        Game.ObserverModel.RemoverListener(DataType.BATTLE, this);
    }

    // 固定写法
    public void OnDataChange(BaseData eventData, int type, object obj)
    {
        switch (eventData.dataType)
        {
            case DataType.BATTLE:
                BattleData data = eventData as BattleData;
                switch (type)
                {
                    case (int)BattleDataType.Win:
                        Debug.Log("total" + data.battleCount + "   win" + data.winCount);
                        break;
                    case (int)BattleDataType.Lose:
                        Debug.Log("total" + data.battleCount + "   lose" + data.loseCount);
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }
}