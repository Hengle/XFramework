using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XDEDZL;

public class CallTest : MonoBehaviour {

    BattleOb bat = new BattleOb();
    BattleData battleData = new BattleData();

	void Start () {
        DataSubjectManager.Instance.AddOnChangedCallback(DataType.BATTLE, bat);
        DataSubjectManager.Instance.Notify(DataSubjectManager.GetData<BattleData>(), (int)BattleDataType.Win);
	}
	
	void Update () {
		
	}
}
