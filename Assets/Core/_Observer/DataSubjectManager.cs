///<summary>
///主题 有多少个basedata类型 就会有多少主题
///<summary>
namespace XDEDZL
{
    using System.Collections.Generic;

    /// <summary>
    /// 数据主题管理类
    /// </summary>
    public class DataSubjectManager : Singleton<DataSubjectManager>
    {
        /// <summary>
        /// 每一个Subject都是一个被观察的对象
        /// </summary>
        protected class Subject : ObservableSubjectTemplate<BaseData, int, object>
        {
            // 继承泛型模板定义一个非泛型主题模板
        }

        /// <summary>
        /// 存储数据类型和对应主题的字典
        /// </summary>
        private Dictionary<DataType, Subject> m_subjectDic = new Dictionary<DataType, Subject>();

        /// <summary>
        /// 增加数据监听
        /// </summary>
        /// <param name="dataType">数据类型</param>
        /// <param name="observer">监听这个数据的观察者</param>
        public void AddListener(DataType dataType, IObserver observer)
        {
            Subject subject = null;
            if (!m_subjectDic.ContainsKey(dataType))
            {
                subject = new Subject();
                m_subjectDic[dataType] = subject;
            }
            m_subjectDic[dataType].Attach(observer.OnDataChange);
        }

        /// <summary>
        /// 删除数据监听
        /// </summary>
        /// <param name="dataType">数据类型</param>
        /// <param name="observer">监听这个数据的观察者</param>
        public void RemoverListener(DataType dataType, IObserver observer)
        {
            if (m_subjectDic.ContainsKey(dataType))
            {
                m_subjectDic[dataType].Detach(observer.OnDataChange);
            }
        }

        /// <summary>
        /// 通知事件
        /// </summary>
        /// <param name="data">data主题</param>
        /// <param name="type">事件类型</param>
        /// <param name="obj">映射参数</param>
        public void Notify(BaseData data, int type = 0, object obj = null)
        {
            if (m_subjectDic.ContainsKey(data.dataType))
                m_subjectDic[data.dataType].Notify(data, type, obj);
        }
    }
}