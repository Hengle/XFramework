// ==========================================
// 描述： 
// 作者： LYG
// 时间： 2018-11-22 13:15:42
// 版本： V 1.0
// ==========================================
using UnityEngine;
using UnityEngine.UI;

public class ToggleExt : Toggle {
    
    /// <summary>
    /// 受控制的页面
    /// </summary>
    public GameObject controledPanel;

    protected override void Start()
    {
        base.Start();

        if(controledPanel != null)
        {
            this.AddCotroledPanel(controledPanel);
        }
    }
}
