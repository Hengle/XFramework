// ==========================================
// 描述： 
// 作者： HAK
// 时间： 2018-11-14 08:41:39
// 版本： V 1.0
// ==========================================
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Events;

public class ButtonListPanel : BasePanel {

    /// <summary>
    /// 内容
    /// </summary>
    private RectTransform content;
    /// <summary>
    /// 按钮列表
    /// </summary>
    private List<Transform> btnList;
    /// <summary>
    /// 显示文字
    /// </summary>
    private Text showText;
    private UnityAction<string> operateAction;

    private void OnEnable()
    {

    }

    protected override void Awake()
    {
        base.Awake();
        level = UILevel.Ten;

        content = transform.FindRecursive("Content").GetComponent<RectTransform>();
        showText = transform.Find("ShowText").GetComponent<Text>();

        // 操作按钮注册
        transform.Find("LoadBtn").GetComponent<Button>().onClick.AddListener(Opreate);
        // 界面关闭按钮
        transform.Find("Close").GetComponent<Button>().onClick.AddListener(() =>
        {
            UIManager.Instance.PopPanel();
        });

        btnList = new List<Transform>();
    }

    public override void Init()
    {
        base.Init();
    }

    public override void OnEnter()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
        rect.DOScaleY(1.0f, 0.1f);
        canvasGroup.interactable = true;
        transform.SetAsLastSibling();
    }
    
    public override void OnExit()
    {
        base.OnExit();
    }

    /// <summary>
    /// 适配按钮的数量
    /// </summary>
    private void AutoSetButtonCount(string[] btnStr)
    {
        GameObject btnObj = Resources.Load("UIPanelPrefabs/Button") as GameObject;
        int differ = btnStr.Length - btnList.Count;
        // 根据现有按钮数量补齐或删除
        if (differ >= 0)
        {
            for (int i = btnList.Count, length = btnStr.Length; i < length; i++)
            {
                Transform btn = Instantiate(btnObj).transform;
                btn.SetParent(content, false);
                btnList.Add(btn);
                btn.GetComponent<Button>().onClick.AddListener(() => { ChangeShowText(btnStr[btnList.IndexOf(btn)]); });
            }
        }
        else
        {
            for (int i = 0; i < -differ; i++)
            {
                Transform btn = btnList[btnList.Count - 1];
                btnList.Remove(btn);
                Destroy(btn.gameObject);
            }
        }

        // 更新按钮名
        for (int i = 0; i < btnList.Count; i++)
        {
            btnList[i].transform.FindRecursive("Text").GetComponent<Text>().text = btnStr[i];
        }

        // 更新显示文字
        if (btnStr.Length > 0)
        {
            ChangeShowText(btnStr[0]);
        }
        else
        {
            ChangeShowText("空");
        }
    }

    /// <summary>
    /// 改变显示字符串
    /// </summary>
    private void ChangeShowText(string str)
    {
        showText.text = str;
    }

    /// <summary>
    /// 面板的操作按钮
    /// </summary>
    private void Opreate()
    {
        operateAction(showText.text);
    }

    public void AddAction(string[] strs, UnityAction<string> action)
    {
        AutoSetButtonCount(strs);
        operateAction = action;
    }
}