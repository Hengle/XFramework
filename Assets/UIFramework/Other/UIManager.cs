using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using DG.Tweening;

public enum UIPanelType
{
    Main,            // 主界面
    Create,          // 创建单位 
    Group,           // 五群
    Team,            // 十八队
    CommandPost,     // 三所
    ShowPower,       // 能力展示
    AdjustPosture,   // 态势调整
    Setting,         // 设置
}

/// <summary>
/// 单例UI管理类
/// </summary>
public class UIManager {

    private Transform canvasTransform;
    private Transform CanvasTransform
    {
        get
        {
            if (canvasTransform == null)
            {
                canvasTransform = GameObject.Find("Canvas").transform;
            }
            return canvasTransform;
        }
    }
    private Dictionary<UIPanelType, string> panelPathDict;  //存储所有面板Prefab的路径
    private Dictionary<UIPanelType, BasePanel> panelDict;   //保存所有实例化面板的游戏物体身上的BasePanel组件
    private Stack<BasePanel> panelStack;

    private RectTransform describeRect;
    private Image describeImage;
    private Text headText;
    private Text describeText;
    private Color32 headTextCol = new Color32(255, 170, 0, 255);

    public UIManager()
    {
        InitPathDic();
        InitDescribe();
    }

    /// <summary>
    /// 把某个页面入栈，  把某个页面显示在界面上
    /// </summary>
    public void PushPanel(UIPanelType panelType)
    {
        if (panelStack == null)
            panelStack = new Stack<BasePanel>();

        BasePanel nextPanel = GetPanel(panelType); // 计划打开的页面
        BasePanel currentPanel = null;             // 最近一次关闭的界面
        //判断一下栈里面是否有页面
        if (panelStack.Count > 0)
        {
            BasePanel topPanel = panelStack.Peek(); // 获取栈顶页面

            // 如果即将打开的页面是栈顶页面，即关闭栈顶页面
            if (topPanel == nextPanel)
            {
                PopPanel();
                return;
            }
            // 当栈内有面板时，进行判断
            while(panelStack.Count > 0)
            {
                if (topPanel.level < nextPanel.level)
                {
                    break;
                }
                // 如果栈顶页面的层级不小于要打开的页面层级，关闭它并保存
                currentPanel = PopPanel();
                if (panelStack.Count > 0)
                {
                    topPanel = panelStack.Peek();
                }
            }
        }
        // 如果最后关闭的界面和要打开的是同一个，就不打开了
        if (currentPanel != nextPanel)
        {
            nextPanel.OnEnter();
            panelStack.Push(nextPanel); // 将打开的面板入栈
        }
    }
    /// <summary>
    /// 出栈 ，把页面从界面上移除
    /// </summary>
    public BasePanel PopPanel()
    {
        if (panelStack == null)
            panelStack = new Stack<BasePanel>();

        if (panelStack.Count <= 0) return null;

        BasePanel topPanel = panelStack.Pop(); // 获取并移除栈顶面板
        topPanel.OnExit();                     // 关闭面板
        return topPanel;
    }

    /// <summary>
    /// 根据面板类型 得到实例化的面板
    /// </summary>
    /// <returns></returns>
    public BasePanel GetPanel(UIPanelType panelType)
    {
        if (panelDict == null)
        {
            panelDict = new Dictionary<UIPanelType, BasePanel>();
        }

        BasePanel panel;
        panelDict.TryGetValue(panelType, out panel);

        if (panel == null)
        {
            //如果找不到，那么就找这个面板的prefab的路径，然后去根据prefab去实例化面板
            string path;
            panelPathDict.TryGetValue(panelType, out path);

            GameObject instPanel = GameObject.Instantiate(Resources.Load(path)) as GameObject;
            instPanel.transform.SetParent(CanvasTransform,false);
            BasePanel basePanel = instPanel.GetComponent<BasePanel>();
            basePanel.Init();
            if(basePanel == null)
            {
                Debug.Log(instPanel.name + "没有挂载对应的basePanel派生");
            }
            panelDict.Add(panelType, basePanel);
            return basePanel;
        }
        else
        {
            return panel;
        }
    }

    /// <summary>
    /// 关闭所有面板
    /// </summary>
    public void CloseAll()
    {
        while(panelStack.Count > 0)
        {
            panelStack.Pop().OnExit();
        }
    }

    /// <summary>
    /// 初始化面板预制体路径字典
    /// </summary>
    private void InitPathDic()
    {
        panelPathDict = new Dictionary<UIPanelType, string>
        {
            [UIPanelType.Main] = "UIPanelPrefabs/One_MainPanel",
            [UIPanelType.Create] = "UIPanelPrefabs/Two_CreatePanel",
            [UIPanelType.ShowPower] = "UIPanelPrefabs/Two_ShowPowerPanel",
            [UIPanelType.AdjustPosture] = "UIPanelPrefabs/Two_AdjustPanel",
            [UIPanelType.Group] = "UIPanelPrefabs/Three_GroupPanel",
            [UIPanelType.Team] = "UIPanelPrefabs/Three_TeamPanel",
            [UIPanelType.CommandPost] = "UIPanelPrefabs/Three_CommandPostPanel",
            [UIPanelType.Setting] = "UIPanelPrefabs/Top_SettingPanel",
        };
    }
       
    /// <summary>   
    /// 初始化描述图片
    /// </summary>
    private void InitDescribe()
    {
        describeRect = CanvasTransform.Find("Describe").GetComponent<RectTransform>();
        describeImage = CanvasTransform.Find("Describe").GetComponent<Image>();
        headText = CanvasTransform.Find("Describe/HeadText").GetComponent<Text>();
        describeText = CanvasTransform.Find("Describe/DescribeText").GetComponent<Text>();
        
        CloseDescribe();
    }

    /// <summary>
    /// 显示描述图片
    /// </summary>
    public void ShowDescribe(float y,string headStr, string describeStr)
    {
        Vector3 xyz = describeRect.position;
        xyz.y = y;
        describeRect.position = xyz;
        //describeRect.rect.size.
        describeImage.DOColor(Color.white, 0.3f);
        describeText.DOColor(Color.white, 0.3f);
        headText.DOColor(headTextCol, 0.3f);
        headText.text = headStr;
        describeText.text = describeStr;
    }

    /// <summary>
    /// 关闭描述图片
    /// </summary>
    public void CloseDescribe()
    {
        describeImage.DOColor(Color.clear, 0.2f);
        describeText.DOColor(Color.clear, 0.2f);
        headText.DOColor(Color.clear, 0.2f);
    }
}
