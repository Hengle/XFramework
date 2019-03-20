using System.Collections.Generic;
using UnityEngine;

public enum UILevel
{
    One,
    Two,
    Three,
    Ten,
}

public class BasePanel : MonoBehaviour {

    /// <summary>
    /// UI层级
    /// </summary>
    [HideInInspector] public UILevel level = UILevel.One;
    protected CanvasGroup canvasGroup;

    protected RectTransform rect;

    private Dictionary<string, BaseGUI> mUIDic;

    protected virtual void Awake()
    {
        InitGUIDic();
        canvasGroup = GetComponent<CanvasGroup>();
        rect = GetComponent<RectTransform>();
    }

    /// <summary>
    /// 面板初始化，只会执行一次，在Awake后start前执行
    /// </summary>
    public virtual void Init()
    {
        Vector3 rectSize = rect.localScale;
        //rectSize.y = 0;
        rect.localScale = rectSize;
    }

    /// <summary>
    /// 界面被显示出来
    /// </summary>
    public virtual void OnEnter()
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
    }

    /// <summary>
    /// 界面不显示,退出这个界面，界面被关闭
    /// </summary>
    public virtual void OnExit()
    {
        gameObject.SetActive(false);
    }

    private void InitGUIDic()
    {
        mUIDic = new Dictionary<string, BaseGUI>();
        BaseGUI[] uis = transform.GetComponentsInChildren<BaseGUI>();
        for (int i = 0; i < uis.Length; i++)
        {
            mUIDic.Add(uis[i].name, uis[i]);
        }
    }

    public BaseGUI this[string key]
    {
        get
        {
            if (mUIDic.ContainsKey(key))
                return mUIDic[key];
            else
            {
                throw new System.Exception(this + " : 没有名为" + key + "的UI组件");
            }
        }
    }
}
