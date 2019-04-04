using UnityEngine.UI;
using UnityEngine;

namespace XDEDZL.UI
{
    public class GUVerticalLayoutGroup : BaseGUI
    {
        /// <summary>
        /// 实体回收事件
        /// </summary>
        private System.Action<GameObject> entityRecycle;
        public VerticalLayoutGroup verticalLayoutGroup;

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
        public void AddEntity(Transform entity)
        {
            entity.SetParent(verticalLayoutGroup.transform);
        }


        public void RemoveEntity(GameObject gameObject)
        {
            entityRecycle?.Invoke(gameObject);
        }

        public void RemoveEntity(int index)
        {
            RemoveEntity(verticalLayoutGroup.transform.GetChild(index).gameObject);
        }

        public void SetRecycle(System.Action<GameObject> _entityRecycle)
        {
            entityRecycle = _entityRecycle;
        }
    }
}