using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XDEDZL;

/// <summary>
/// 数据观察者运用示例
/// </summary>
public class zObserverExample : MonoBehaviour
{
    BattleOb bat;
    TestOb test;

    void Start()
    {
        bat = new BattleOb();
        test = new TestOb();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            DataSubjectManager.Instance.Notify(DataSubjectManager.GetData<BattleData>(), (int)BattleDataType.Win);
        }
    } 
}


public class BattleOb : IObserver
{
    // 此类可能是要继承Mono的，如果是，则在start中注册
    public BattleOb()
    {
        DataSubjectManager.Instance.AddOnChangedCallback(DataType.BATTLE, this);
    }

    public void OnDataChange(BaseData eventData, int type, object obj)
    {
        switch (eventData.dataType)
        {
            case DataType.BATTLE:
                switch (type)
                {
                    case (int)BattleDataType.Win:
                        Debug.Log("BattleDataType.Win");
                        break;
                    case (int)BattleDataType.Lose:
                        Debug.Log("BattleDataType.Lose");
                        break;
                    default:
                        break;
                }
                break;
        }
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


public class TestOb : IObserver
{
    public TestOb()
    {
        DataSubjectManager.Instance.AddOnChangedCallback(DataType.BATTLE, this);
    }

    public void OnDataChange(BaseData eventData, int type, object obj)
    {
        switch (eventData.dataType)
        {
            case DataType.BATTLE:
                switch (type)
                {
                    case (int)BattleDataType.Win:
                        BattleData data = eventData as BattleData;
                        Debug.Log("total" + data.battleCount + "   win" + data.winCount);
                        
                        break;
                    case (int)BattleDataType.Lose:
                        Debug.Log("Test在BattleLose时做的事");
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

