using UnityEngine;

namespace XFramework.UI
{
    public class Tree : MonoBehaviour
    {
        /// <summary>
        /// 模板
        /// </summary>
        private GameObject m_Template;
        /// <summary>
        /// 树的跟节点
        /// </summary>
        private TreeNode m_RootTreeItem;

        private void Start()
        {
            m_Template = transform.Find("ItemTemplate").gameObject;

            GenerateTree();
        }

        /// <summary>
        /// 创建的树的实体
        /// </summary>
        private void CreateTreeEntity()
        {
            m_RootTreeItem.CreateTree(null, m_Template);
        }

        /// <summary>
        /// 构造父子关系
        /// </summary>
        public void GenerateTree()
        {
            TreeNode tree = new TreeNode("1");
            m_RootTreeItem = tree;

            tree.AddItem(new TreeNode("11")
                    .AddItem(new TreeNode("111")
                        .AddItem(new TreeNode("1111")
                            .AddItem(new TreeNode("11111"))
                            .AddItem(new TreeNode("11112")))
                        .AddItem(new TreeNode("1112")))
                    .AddItem(new TreeNode("112")))
                .AddItem(new TreeNode("12")
                    .AddItem(new TreeNode("121"))
                    .AddItem(new TreeNode("122")
                        .AddItem(new TreeNode("1221"))
                        .AddItem(new TreeNode("1222")))
                    .AddItem(new TreeNode("123")))
                .AddItem(new TreeNode("13")
                    .AddItem(new TreeNode("131")));

            CreateTreeEntity();
            //m_RootTreeItem.RefreshPos();
        }
    }
}