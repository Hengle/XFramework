using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RootButton : MonoBehaviour {

    private Button startBtn;

	void Start () {
        startBtn = GetComponent<Button>();
        startBtn.onClick.AddListener(OnClick);
        GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
	}

    public void OnClick()
    {
        // 显示主界面
        Singleton<UIManager>.Instance.PushPanel(UIPanelType.Main);
    }
	
}
