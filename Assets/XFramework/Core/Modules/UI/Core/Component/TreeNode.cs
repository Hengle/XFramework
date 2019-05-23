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
        /// 当前结点所归属的树
        /// </summary>
        private Tree m_Tree;
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
        /// 层级
        /// </summary>
        private int m_level;

        /// <summary>
        /// 显示文字
        /// </summary>
        public string Text { get; private set; }

        public TreeNode()
        {
            m_IsOn = true;
            m_Childs = new List<TreeNode>();
        }

        public TreeNode(string text) : this()
        {
            this.Text = text;
        }

        /// <summary>
        /// 初始化场景中对应的实体
        /// </summary>
        /// <param name="tree"></param>
        private void InitEnity(Tree tree)
        {
            m_Tree = tree;

            // 创建自己
            if (m_Parent == null)
            {
                m_Rect = Object.Instantiate(m_Tree.NodeTemplate, m_Tree.NodeTemplate.transform.parent.Find("Root")).GetComponent<RectTransform>();
                m_level = 0;
            }
            else
            {
                m_Rect = Object.Instantiate(m_Tree.NodeTemplate, m_Parent.m_Rect.Find("Child")).GetComponent<RectTransform>();
                m_level = m_Parent.m_level + 1;
            }

            // UI组件设置
            m_Rect.Find("Toggle").GetComponent<Toggle>().onValueChanged.AddListener((value) =>
            {
                m_IsOn = value;

                Refresh(value);
                Root.RefreshPos();

                tree.onOn_Off.Invoke(value, this);
            });

            m_Rect.Find("Button").GetComponent<Button>().onClick.AddListener(() =>
            {
                tree.onSelectNode.Invoke(this);
            });

            m_Rect.Find("Button").Find("Text").GetComponent<Text>().text = this.Text;
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
        /// 添加一个父子关系
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public TreeNode AddChild(TreeNode item)
        {
            m_Childs.Add(item);
            item.m_Parent = this;
            return this;
        }

        public TreeNode AddChild(string text)
        {
            return AddChild(new TreeNode(text));
        }

        /// <summary>
        /// 根据已有的父子关系创建一颗（子）树
        /// </summary>
        /// <param name="m_Parent"></param>
        /// <param name="gameObject"></param>
        public void CreateTree(Tree tree)
        {
            InitEnity(tree);

            // 继续创建
            foreach (var child in m_Childs)
            {
                child.CreateTree(m_Tree);
            }

            if (m_Parent == null)
            {
                RefreshPos();
            }
        }

        /// <summary>
        /// 添加一个父子关系并创建实体
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public TreeNode CreateChild(TreeNode item)
        {
            AddChild(item);

            item.InitEnity(m_Tree);
            Refresh(m_IsOn);
            Root.RefreshPos();
            return this;
        }

        public TreeNode CreateChild(string text)
        {
            TreeNode item = new TreeNode(text);
            return CreateChild(item);
        }

        /// <summary>
        /// 删除自身
        /// </summary>
        public void Delete()
        {
            if(m_Parent == null)
            {
                Debug.Log("根结点不能删除");
                return;
            }
            Object.Destroy(m_Rect.gameObject);
            m_Parent.m_Childs.Remove(this);
            Root.RefreshPos();
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
        /// 根结点
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

        /// <summary>
        /// 重置父物体
        /// </summary>
        /// <param name="parent"></param>
        public void SetParent(TreeNode parent)
        {
            m_Parent.m_Childs.Remove(this);
            parent.AddChild(this);
        }

        /// <summary>
        /// 通过字符串找寻子节点
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public TreeNode Find(string path)
        {
            var temp = path.Split(new char[] { '/' }, 2);
            if (temp.Length == 1)
                return this;

            TreeNode node = null;
            foreach (var item in m_Childs)
            {
                if (item.Text == temp[0])
                {
                    node = item;
                    break;
                }
            }

            if (node == null)
                return node;
            else
                return node.Find(temp[1]);
        }

        /// <summary>
        /// 根据索引获取子节点
        /// </summary>
        public TreeNode GetChild(int index)
        {
            return m_Childs[index];
        }
    }
}