using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 对应面板的固定名称
/// </summary>
public class UIName
{
    public const string Main = "Main";                       // 主界面
    public const string Create = "Create";
    public const string Group = "Group";
    public const string Team = "Team";
    public const string CommandPost = "CommandPost";
    public const string ShowPower = "ShowPower";
    public const string Adjust = "Adjust";
    public const string Setting = "Setting";
    public const string Verify = "Verify";                   // 确认面板
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
    /// <summary>
    /// 存储所有面板Prefab的路径
    /// </summary>
    private Dictionary<string, string> panelPathDict;
    /// <summary>
    /// 保存所有实例化面板的游戏物体身上的BasePanel组件
    /// </summary>
    private Dictionary<string, BasePanel> panelDict;
    /// <summary>
    /// 存储面板的栈
    /// </summary>
    private Stack<BasePanel> panelStack;

    /// <summary>
    /// 提示
    /// </summary>
    private RectTransform tipRect;
    /// <summary>
    /// 提示
    /// </summary>
    private Text tipText;

    /// <summary>
    /// 确认操作的委托
    /// </summary>
    private Action VerifyOperate;
    /// <summary>
    /// 确认面板的显示文字
    /// </summary>
    public string VerifyText { get; private set; }

    public UIManager()
    {
        InitPathDic();
        MonoEvent.Instance.UPDATE += OnUpdate;
    }

    private void OnUpdate()
    {
        foreach (var item in panelStack)
        {
            item.OnUpdate();
        }
    }

    /// <summary>
    /// 把某个页面入栈，  把某个页面显示在界面上
    /// </summary>
    public void PushPanel(string panelType)
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
            while (panelStack.Count > 0)
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
    public BasePanel GetPanel(string uiname)
    {
        if (panelDict == null)
        {
            panelDict = new Dictionary<string, BasePanel>();
        }

        BasePanel panel;
        panelDict.TryGetValue(uiname, out panel);

        if (panel == null)
        {
            // 如果找不到，那么就找这个面板的prefab的路径，然后去根据prefab去实例化面板
            string path;
            panelPathDict.TryGetValue(uiname, out path);
            GameObject instPanel = GameObject.Instantiate(Resources.Load(path)) as GameObject;
            instPanel.transform.SetParent(CanvasTransform, false);

            // UICore与派生类不一定在一个程序集类，所以不能直接用Type.GetType  TODO : 根据不同平台规定路径
            Assembly asmb;
#if UNITY_EDITOR
            asmb = Assembly.LoadFrom(System.Environment.CurrentDirectory + @"\Library\ScriptAssemblies\Assembly-CSharp.dll");
#endif
            Type type = asmb.GetType(uiname + "Panel");
            BasePanel basePanel = (BasePanel)Activator.CreateInstance(type);
            basePanel.Init(instPanel);
            if (basePanel == null)
            {
                throw new Exception("面板类名错误");
            }
            panelDict.Add(uiname, basePanel);
            return basePanel;
        }
        else
        {
            return panel;
        }
    }
    /// <summary>
    /// 返回特定类型的panel
    /// </summary>
    public T GetPanel<T>(string panelType) where T : BasePanel
    {
        return (T)GetPanel(panelType);
    }

    /// <summary>
    /// 判断栈顶界面是否为某一个类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool TopPanelEqual(string type)
    {
        return TopPanel == GetPanel(type);
    }

    /// <summary>
    /// 栈顶界面
    /// </summary>
    public BasePanel TopPanel
    {
        get
        {
            if (panelStack == null || panelStack.Count < 1)
                return null;
            else
                return panelStack?.Peek();
        }
    }
    /// <summary>
    /// 返回一个特定类型的栈顶界面，如果不匹配返回Null
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T TopPanell<T>() where T : BasePanel
    {
        return (T)TopPanel;
    }

    /// <summary>
    /// 关闭所有面板
    /// </summary>
    public void CloseAll()
    {
        while (panelStack.Count > 0)
        {
            panelStack.Pop().OnExit();
        }
    }

    /// <summary>
    /// 初始化面板预制体路径字典
    /// </summary>
    private void InitPathDic()
    {
        panelPathDict = new Dictionary<string, string>
        {
            [UIName.Main] = "UIPanelPrefabs/One_MainPanel",
            [UIName.Create] = "UIPanelPrefabs/Two_CreatePanel",
            [UIName.ShowPower] = "UIPanelPrefabs/Two_ShowPowerPanel",
            [UIName.Adjust] = "UIPanelPrefabs/Two_AdjustPanel",
            [UIName.Group] = "UIPanelPrefabs/Three_GroupPanel",
            [UIName.Team] = "UIPanelPrefabs/Three_TeamPanel",
            [UIName.CommandPost] = "UIPanelPrefabs/Three_CommandPostPanel",
            [UIName.Setting] = "UIPanelPrefabs/Top_SettingPanel",
            [UIName.Verify] = "UIPanelPrefabs/VerifyPanel",
        };
    }

    /// <summary>
    /// 初始化提示
    /// </summary>
    private void InitTip()
    {
        tipRect = canvasTransform.Find("Tip").GetComponent<RectTransform>();
        tipText = tipRect.GetComponent<Text>();
    }

    /// <summary>
    /// 显示提示
    /// </summary>
    public void ShowTip(string content)
    {
        tipRect.localPosition = Vector3.zero;
        tipText.color = Color.red;
        tipText.text = content;
    }

    /// <summary>
    /// 初始化游戏运行过程中需要的UI
    /// </summary>
    public void InitGamingUI()
    {
        InitTip();
    }

    /// <summary>
    /// 添加确认操作
    /// 在某个面板的某个操作前调用
    /// </summary>
    public void AddVerifyOperate(string showText, Action action = null)
    {
        VerifyOperate = action;
        VerifyText = showText;
    }

    /// <summary>
    /// 执行确认委托
    /// 除了VerifyPanel面板 不要调用
    /// </summary>
    public void ExecuteVerifyOperate()
    {
        VerifyOperate?.Invoke();
    }

    /// <summary>
    /// 清楚所有UI面板
    /// </summary>
    public void Clear()
    {
        //while(panelStack?.Count > 0)
        //{
        //    PopPanel();
        //}
        panelDict?.Clear();
        panelStack?.Clear();
    }
}
