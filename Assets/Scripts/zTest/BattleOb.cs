using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XDEDZL;

public class BattleOb : UIObserver {

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
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
