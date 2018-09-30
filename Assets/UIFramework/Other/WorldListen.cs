using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldListen : MonoBehaviour {

	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Singleton<UIManager>.Instance.PushPanel(UIPanelType.Setting);
            Time.timeScale = 1;
        }
	}
}
