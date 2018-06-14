//=====================================================================================/
///<summary>
///主题 有多少个basedata类型 就会有多少主题
///<summary>
//=====================================================================================/
namespace JM
{
    //using JMUI;
    using System.Collections.Generic;

    public class JMDataSubjectManager : SingletonTemplate<JMDataSubjectManager> //改模板应改使用JM内的 方便以后移植,暂时这样写
    {
        protected class Subject : JMObservableSubjectTemplate<JMBaseData, int, object>
        { }

        private Dictionary<JMDataType, Entry> m_subjectDic = new Dictionary<JMDataType, Entry>();

        private class Entry
        {
            public JMDataType dataType;
            public Subject subject = new Subject();

            public Entry(JMDataType type)
            {
                dataType = type;
            }
        }

        /// <summary>
        /// 增加数据监听
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="observer"></param>
        public void AddOnChangedCallback(JMDataType dataType, JMUIObserver observer)
        {
            Entry en = null;
            if (!m_subjectDic.ContainsKey(dataType))
            {
                en = new Entry(dataType);
                m_subjectDic[dataType] = en;
            }
            m_subjectDic[dataType].subject.Attach(observer.OnDataChange);
        }

        public void RemoveOnChangedCallback(JMDataType dataType, JMUIObserver observer)
        {
            if (m_subjectDic.ContainsKey(dataType))
            {
                m_subjectDic[dataType].subject.Detach(observer.OnDataChange);
            }
        }

        public static T GetData<T>() where T : JMBaseData, new()
        {
            return JMBaseData.GetData<T>();
        }

        /// <summary>
        /// 通知事件
        /// </summary>
        /// <param name="data">data主题</param>
        /// <param name="type">事件类型</param>
        /// <param name="obj">映射参数</param>
        public void Notify(JMBaseData data, int type = 0, object obj = null)
        {
            if (m_subjectDic.ContainsKey(data.dataType))
                m_subjectDic[data.dataType].subject.Notify(data, type, obj);
            //else
                //GameDebugLog.Log(data.dataType.ToString());
        }
    }
}