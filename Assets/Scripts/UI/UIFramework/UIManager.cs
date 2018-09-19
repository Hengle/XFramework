using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using XDEDZL;

public enum UIPanelType
{
    Main,            // 主界面
    Create,          // 创建单位 
    Group,           // 五群
    Team,            // 十八队
    CommandPost,     // 三所
}

/// <summary>
/// 单例UI管理类
/// </summary>
public class UIManager : Singleton<UIManager>
{

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

    public UIManager()
    {
        InitPathDic();
    }

    /// <summary>
    /// 把某个页面入栈，  把某个页面显示在界面上
    /// </summary>
    public void PushPanel(UIPanelType panelType)
    {
        if (panelStack == null)
            panelStack = new Stack<BasePanel>();

        BasePanel nextPanel = GetPanel(panelType);
        BasePanel lastClosePanel = new BasePanel();
        //判断一下栈里面是否有页面
        if (panelStack.Count > 0)
        {
            BasePanel topPanel = panelStack.Peek(); // 获取栈顶页面
            if(topPanel == nextPanel)
            {
                PopPanel();
                return;
            }
            for (int i = 0,length = panelDict.Count; i < length; i++)
            {
                if (topPanel.level < nextPanel.level)
                {
                    break;
                }
                // 如果栈顶页面的层级不小于要打开的页面层级，关闭它
                lastClosePanel = PopPanel();
                topPanel = panelStack.Peek();
            }
        }
        // 如果要打开的面板和刚刚关闭的面板是同一个，就不打开
        if(lastClosePanel != nextPanel)
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
    private BasePanel GetPanel(UIPanelType panelType)
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
    /// 初始化面板预制体路径字典
    /// </summary>
    private void InitPathDic()
    {
        panelPathDict = new Dictionary<UIPanelType, string>();
        panelPathDict.Add(UIPanelType.Main, "UIPnaelPrefabs/MainPanel");
        panelPathDict.Add(UIPanelType.Create, "UIPnaelPrefabs/CreatePanel");
        panelPathDict.Add(UIPanelType.Group, "UIPnaelPrefabs/GroupPanel");
        panelPathDict.Add(UIPanelType.Team, "UIPnaelPrefabs/TeamPanel");
        panelPathDict.Add(UIPanelType.CommandPost, "UIPnaelPrefabs/CommandPostPanel");
    }
}
