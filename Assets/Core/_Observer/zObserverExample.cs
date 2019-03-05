using UnityEngine;
using XDEDZL;

/*
 * 以战斗系统
 */


/// <summary>
/// 数据观察者运用示例
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


public class BattleSystem
{

    BattleData btData = new BattleData();

    public void BattleWin()
    {
        // 通知所有观察者并传递数据
        DataSubjectManager.Instance.Notify(btData, (int)BattleDataType.Win);
    }

    public void BattleLose()
    {
        DataSubjectManager.Instance.Notify(btData, (int)BattleDataType.Lose);
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


public class PlayerDataMgr : IObserver
{
    // 此类可能是要继承Mono的，如果是，则在start中注册
    public PlayerDataMgr()
    {
        DataSubjectManager.Instance.AddOnChangedCallback(DataType.BATTLE, this);
    }

    // 在合适的时候移除观察者
    ~PlayerDataMgr()
    {
        DataSubjectManager.Instance.RemoveOnChangedCallback(DataType.BATTLE, this);
    }

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

