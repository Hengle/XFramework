using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SettingPanel : BasePanel {

    private Button escBtn;
    private Button backBtn;

    private void OnEscClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnBackClick()
    {
        UIManager.Instance.PopPanel();
        Time.timeScale = 1;
    }

    public override void Init(GameObject _gameObject)
    {
        base.Init(_gameObject);
        level = 10;
        escBtn = transform.Find("Esc").GetComponent<Button>();
        backBtn = transform.Find("Back").GetComponent<Button>();
        escBtn.onClick.AddListener(OnEscClick);
        backBtn.onClick.AddListener(OnBackClick);
    }

    public override void OnEnter()
    {
        rect.DOScaleY(1.0f, 0.1f).OnComplete(() => { Time.timeScale = 0; });
        transform.SetAsLastSibling();
    }

    public override void OnExit()
    {
        rect.DOScaleY(0.0f, 0.1f);
    }
}
