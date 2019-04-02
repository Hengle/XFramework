using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class UIMgr : IUIManager
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
    /// 出于打开状态的面板列表
    /// TODO ,列表数据结构要重写
    /// </summary>
    private List<BasePanel> panelList;

    public UIMgr()
    {
        panelList = new List<BasePanel>();
        InitPathDic();
        MonoEvent.Instance.UPDATE += OnUpdate;
    }

    private void OnUpdate()
    {
        foreach (var item in panelList)
        {
            item.OnUpdate();
        }
    }

    public void OpenPanel(string uiname)
    {
        BasePanel panel = GetPanel(uiname);
        if (null == panel)
            return;

        if (panelList.Contains(panel))
        {
            panel.OnResume();
            panelList.Remove(panel);
        }
        else
        {
            if (panelList.Count > 0)
            {
                panelList[panelList.Count - 1].OnPause();
            }
            panel.OnEnter();
        }
        panelList.Add(panel);
    }

    public void ClosePanel(string uiname)
    {
        BasePanel panel = GetPanel(uiname);
        if (panelList.Contains(panel))
        {
            panel.OnExit();
            panelList.Remove(panel);
        }
    }

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
#else
            asmb = Assembly.LoadFrom(Application.dataPath + "/Managed/Assembly-CSharp.dll");
#endif
            Type type = asmb.GetType(uiname + "Panel");
            BasePanel basePanel = (BasePanel)Activator.CreateInstance(type);
            basePanel.Init(instPanel, uiname);
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

    public void CloseTopPanel()
    {
        if(panelList.Count > 0)
        {
            panelList[panelList.Count - 1].OnExit();
            panelList.RemoveAt(panelList.Count - 1);
        }
    }

    /// <summary>
    /// 初始化面板预制体路径字典
    /// </summary>
    private void InitPathDic()
    {
        panelPathDict = new Dictionary<string, string>();
        string rootPath = "UIPanelPrefabs/";
        string uipaths = Resources.Load<TextAsset>("UIPath").text;
        uipaths = uipaths.Replace("\n", "");
        uipaths = uipaths.Replace("\r", "");
        uipaths = uipaths.Replace("\"", "");
        string[] data = uipaths.Split(',');
        string[] nameAndPath;
        for (int i = 0; i < data.Length; i++)
        {
            nameAndPath = data[i].Split(':');
            if (nameAndPath == null || nameAndPath.Length != 2)
                continue;
            panelPathDict.Add(nameAndPath[0], rootPath + nameAndPath[1]);
        }
    }
}