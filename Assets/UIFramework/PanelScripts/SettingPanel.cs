using UnityEngine;
using DG.Tweening;
using XDEDZL.UI;

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
        UIHelper.Instance.CloseTopPanel();
        Time.timeScale = 1;
    }

    public override void Reg()
    {
        Level = 10;
        (this["Esc"] as GUButton).button.onClick.AddListener(OnEscClick);
        (this["Back"] as GUButton).button.onClick.AddListener(OnBackClick);
    }

    public override void OnOpen()
    {
        rect.DOScaleY(1.0f, 0.1f).OnComplete(() => { Time.timeScale = 0; });
        transform.SetAsLastSibling();
    }

    public override void OnClose()
    {
        rect.DOScaleY(0.0f, 0.1f);
    }
}
