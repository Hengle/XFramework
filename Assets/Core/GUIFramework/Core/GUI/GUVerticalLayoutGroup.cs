using UnityEngine.UI;
using UnityEngine;
using System;

namespace XDEDZL.UI
{
    [RequireComponent(typeof(VerticalLayoutGroup))]
    public class GUVerticalLayoutGroup : BaseGUI
    {
        public VerticalLayoutGroup verticalLayoutGroup;

        /// <summary>
        /// 实体回收事件
        /// </summary>
        private Action<GameObject> entityRecycle;
        /// <summary>
        /// 内容模板
        /// </summary>
        private GameObject entityTemplate;

        private void Start()
        {
            entityRecycle = (entity) => { Destroy(entity); };
        }

        private void Reset()
        {
            verticalLayoutGroup = transform.GetComponent<VerticalLayoutGroup>();
        }

        /// <summary>
        /// 添加实体
        /// </summary>
        public GameObject AddEntity()
        {
            return CreateEntity();
        }

        /// <summary>
        /// 移除实体
        /// </summary>
        public void RemoveEntity(GameObject gameObject)
        {
            entityRecycle?.Invoke(gameObject);
        }

        /// <summary>
        /// 移除实体
        /// </summary>
        /// <param name="index"></param>
        public void RemoveEntity(int index)
        {
            RemoveEntity(verticalLayoutGroup.transform.GetChild(index).gameObject);
        }

        /// <summary>
        /// 设置实体回收事件
        /// </summary>
        public void SetRecycle(Action<GameObject> _entityRecycle)
        {
            entityRecycle = _entityRecycle;
        }

        /// <summary>
        /// 配置实体数量
        /// </summary>
        public void ConfigEntity(int count)
        {
            int differ = count - transform.childCount;

            if (differ > 0)         // 当目标Item数量大于现有Item数量时补充Item
            {
                for (int i = 0; i < differ; i++)
                {
                    AddEntity();
                }
            }
            else if (differ < 0)    // 当目标Item数量小于现有Item数量时删除Item
            {
                for (int i = 0; i < -differ; i++)
                {
                    // 暂时写销毁，后期改为对象池回收
                    GameObject.Destroy(transform.GetChild(transform.childCount - 1));
                }
            }
        }

        /// <summary>
        /// 创建一个实体并返回，后期改为从对象池中获取
        /// </summary>
        private GameObject CreateEntity()
        {
            return Instantiate(entityTemplate,transform);
        }
    }
}