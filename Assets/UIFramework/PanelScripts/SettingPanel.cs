using UnityEngine;
using DG.Tweening;

public class SettingPanel : BasePanel {

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
        (this["Esc"] as GUButton).button.onClick.AddListener(OnEscClick);
        (this["Back"] as GUButton).button.onClick.AddListener(OnBackClick);
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
