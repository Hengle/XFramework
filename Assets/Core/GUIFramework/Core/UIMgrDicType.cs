using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace XDEDZL.UI
{
    /// <summary>
    /// 一个使用字典管理的UI管理器
    /// </summary>
    public class UIMgrDicType : IUIManager
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
        /// 处于打开状态的面板字典，key为层级
        /// </summary>
        private Dictionary<int, BasePanel> onDisplayPanelDic;

        public UIMgrDicType()
        {
            onDisplayPanelDic = new Dictionary<int, BasePanel>();
            InitPathDic();
            MonoEvent.Instance.UPDATE += OnUpdate;
        }

        private void OnUpdate()
        {
            foreach (var item in onDisplayPanelDic.Values)
            {
                item.OnUpdate();
            }
        }

        /// <summary>
        /// 打开面板
        /// </summary>
        public void OpenPanel(string uiname)
        {
            BasePanel panel = GetPanel(uiname);
            if (null == panel)
                return;

            if (onDisplayPanelDic.ContainsKey(panel.Level))
            {
                if (onDisplayPanelDic[panel.Level] == panel)
                    return;
                onDisplayPanelDic[panel.Level].OnClose();
            }
            onDisplayPanelDic[panel.Level] = panel;
            if (onDisplayPanelDic.ContainsKey(panel.Level - 1))
            {
                onDisplayPanelDic[panel.Level - 1].OnPause();
            }

            panel.OnOpen();
        }

        /// <summary>
        /// 关闭面板
        /// </summary>
        public void ClosePanel(string uiname)
        {
            BasePanel panel = GetPanel(uiname);
            if (onDisplayPanelDic.ContainsKey(panel.Level))
            {
                panel.OnClose();
                onDisplayPanelDic.Remove(panel.Level);
            }

            if (onDisplayPanelDic.ContainsKey(panel.Level - 1))
            {
                onDisplayPanelDic[panel.Level - 1].OnResume();
            }
        }

        /// <summary>
        /// 获取面板
        /// </summary>
        public BasePanel GetPanel(string uiname)
        {
            if (panelDict == null)
            {
                panelDict = new Dictionary<string, BasePanel>();
            }

            panelDict.TryGetValue(uiname, out BasePanel panel);

            if (panel == null)
            {
                // 根据prefab去实例化面板
                panelPathDict.TryGetValue(uiname, out string path);
                GameObject instPanel = GameObject.Instantiate(Resources.Load(path)) as GameObject;

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

                Transform uiGroup = CanvasTransform.Find("Level" + basePanel.Level);
                if(uiGroup == null)
                {
                    uiGroup = (new GameObject("Level" + basePanel.Level)).transform;
                    uiGroup.SetParent(CanvasTransform);
                    uiGroup.localPosition = new Vector3(-CanvasTransform.position.x, CanvasTransform.position.y, 0); 
                }
                instPanel.transform.SetParent(uiGroup, false);
                return basePanel;
            }
            else
            {
                return panel;
            }
        }

        /// <summary>
        /// 关闭最上层界面
        /// </summary>
        public void CloseTopPanel()
        {
            int level = 0;
            foreach (var item in onDisplayPanelDic.Keys)
            {
                if (item > level)
                    level = item;
            }
            onDisplayPanelDic[level].OnClose();
            onDisplayPanelDic.Remove(level);
        }

        /// <summary>
        /// 初始化面板预制体路径字典
        /// </summary>
        private void InitPathDic()
        {
            panelPathDict = new Dictionary<string, string>();
            string rootPath = "UIPanelPrefabs/";
            string uipaths = Resources.Load<TextAsset>("UIPath").text;
            uipaths = uipaths.Replace("\"", "");
            uipaths = uipaths.Replace("\n", "");
            uipaths = uipaths.Replace("\r", "");
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
}