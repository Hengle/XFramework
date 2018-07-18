//=====================================================================================/
///<summary>
///主题 有多少个basedata类型 就会有多少主题
///<summary>
//=====================================================================================/
namespace XDEDZL
{
    //using UI;
    using System.Collections.Generic;

    public class DataSubjectManager : SingletonTemplate<DataSubjectManager> //改模板应改使用JM内的 方便以后移植,暂时这样写
    {
        protected class Subject : ObservableSubjectTemplate<BaseData, int, object>
        { }

        private Dictionary<DataType, Entry> m_subjectDic = new Dictionary<DataType, Entry>();

        private class Entry
        {
            public DataType dataType;
            public Subject subject = new Subject();

            public Entry(DataType type)
            {
                dataType = type;
            }
        }

        /// <summary>
        /// 增加数据监听
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="observer"></param>
        public void AddOnChangedCallback(DataType dataType, UIObserver observer)
        {
            Entry en = null;
            if (!m_subjectDic.ContainsKey(dataType))
            {
                en = new Entry(dataType);
                m_subjectDic[dataType] = en;
            }
            m_subjectDic[dataType].subject.Attach(observer.OnDataChange);
        }

        public void RemoveOnChangedCallback(DataType dataType, UIObserver observer)
        {
            if (m_subjectDic.ContainsKey(dataType))
            {
                m_subjectDic[dataType].subject.Detach(observer.OnDataChange);
            }
        }

        public static T GetData<T>() where T : BaseData, new()
        {
            return BaseData.GetData<T>();
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
                m_subjectDic[data.dataType].subject.Notify(data, type, obj);
            //else
                //GameDebugLog.Log(data.dataType.ToString());
        }
    }
}