using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework.UI
{
    /// <summary>
    /// 树节点
    /// </summary>
    public class TreeNode
    {
        private RectTransform m_Rect;
        /// <summary>
        /// 子结点
        /// </summary>
        private List<TreeNode> m_Childs;
        /// <summary>
        /// 父结点
        /// </summary>
        private TreeNode m_Parent;
        /// <summary>
        /// 开启状态
        /// </summary>
        private bool m_IsOn;
        /// <summary>
        /// 显示文字
        /// </summary>
        private string text;

        public TreeNode()
        {
            m_IsOn = true;
            m_Childs = new List<TreeNode>();
        }

        public TreeNode(string text) : this()
        {
            this.text = text;
        }

        /// <summary>
        /// 创建一颗（子）树
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="gameObject"></param>
        public void CreateTree(TreeNode parent, GameObject gameObject)
        {
            // 设置父物体
            this.m_Parent = parent;

            // 创建自己
            if(parent == null)
                m_Rect = GameObject.Instantiate(gameObject, gameObject.transform.parent.Find("Root")).GetComponent<RectTransform>();
            else
                m_Rect = GameObject.Instantiate(gameObject, parent.m_Rect.Find("Child")).GetComponent<RectTransform>();

            // UI组件设置
            m_Rect.Find("Toggle").GetComponent<Toggle>().onValueChanged.AddListener((value) =>
            {
                m_IsOn = value;
                Refresh(value);

                Root.RefreshPos();
            });

            m_Rect.Find("Button").Find("Text").GetComponent<Text>().text = this.text;

            // 继续创建
            foreach (var child in m_Childs)
            {
                child.CreateTree(this, gameObject);
            }

            if(parent == null)
            {
                RefreshPos();
            }
        }

        /// <summary>
        /// 刷新位置及显示隐藏
        /// </summary>
        private void Refresh(bool isOn)
        {
            isOn &= m_IsOn;
            if (isOn)
            {
                foreach (var item in m_Childs)
                {
                    item.Refresh(isOn);
                    item.m_Rect.localScale = new Vector3(1, 1, 1);
                }
            }
            else
            {
                foreach (var item in m_Childs)
                {
                    item.Refresh(isOn);
                    item.m_Rect.localScale = new Vector3(1, 0, 1);
                }
            }
        }

        /// <summary>
        /// 刷新位置
        /// </summary>
        public void RefreshPos()
        {
            int index = 0;

            if (m_Parent != null)
            {
                foreach (var item in m_Parent.m_Childs)
                {
                    if (item == this)
                        break;
                    index += item.GetItemCount();
                }
            }

            m_Rect.anchoredPosition = new Vector2(0, -index * 30);

            foreach (var item in m_Childs)
            {
                item.RefreshPos();
            }
        }

        /// <summary>
        /// 添加一个子物体
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public TreeNode AddItem(TreeNode item)
        {
            m_Childs.Add(item);
            return this;
        }

        public TreeNode AddItem(string text)
        {
            return AddItem(new TreeNode(text));
        }

        /// <summary>
        /// 子物体的数量 +1
        /// </summary>
        public int GetItemCount()
        {
            if (m_Childs.Count == 0 || !m_IsOn)
            {
                return 1;
            }
            else
            {
                int count = 0;
                foreach (var item in m_Childs)
                {
                    count += item.GetItemCount();
                }
                return count + 1;
            }
        }

        /// <summary>
        /// 获取自己在父物体种的索引
        /// </summary>
        public int GetSiblingIndex()
        {
            if(m_Parent != null)
            {
                int index = 0;
                foreach (var item in m_Parent.m_Childs)
                {
                    if (item == this)
                        return index;
                }
            }
            return 0;
        }

        /// <summary>
        /// 跟结点
        /// </summary>
        public TreeNode Root
        {
            get
            {
                TreeNode item = this;
                while(item.m_Parent != null)
                {
                    item = item.m_Parent;
                }
                return item;
            }
        }
    }
}