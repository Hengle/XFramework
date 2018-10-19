//=====================================================================================/
///<summary>
///主题 有多少个basedata类型 就会有多少主题
///<summary>
//=====================================================================================/
namespace XDEDZL
{
    //using UI;
    using System.Collections.Generic;

    public class DataSubjectManager : Singleton<DataSubjectManager> //改模板应改使用XDEDZL内的 方便以后移植,暂时这样写
    {
        protected class Subject : ObservableSubjectTemplate<BaseData, int, object>
        {
            //主题，这么写应该只是为了方便看吧，否则把ObservableSubjectTemplate直接写成嵌套类会很乱
        }

        /// <summary>
        /// 存储数据类型和对应主题的字典
        /// </summary>
        private Dictionary<DataType, Subject> m_subjectDic = new Dictionary<DataType, Subject>();

        /// <summary>
        /// 把subject做一层封装，加入dataType参数以便识别,但好像又没什么必要,但本蟑螂觉得暂时没什么必要，所有去掉了
        /// </summary>
        //private class Entry
        //{
        //    public DataType dataType;
        //    public Subject subject = new Subject();

        //    public Entry(DataType type)
        //    {
        //        dataType = type;
        //    }
        //}

        /// <summary>
        /// 增加数据监听
        /// </summary>
        /// <param name="dataType">数据类型</param>
        /// <param name="observer">监听这个数据的观察者</param>
        public void AddOnChangedCallback(DataType dataType, Observer observer)
        {
            Subject en = null;
            if (!m_subjectDic.ContainsKey(dataType))
            {
                en = new Subject();
                m_subjectDic[dataType] = en;
            }
            m_subjectDic[dataType].Attach(observer.OnDataChange);
        }

        /// <summary>
        /// 删除数据监听
        /// </summary>
        /// <param name="dataType">数据类型</param>
        /// <param name="observer">监听这个数据的观察者</param>
        public void RemoveOnChangedCallback(DataType dataType, Observer observer)
        {
            if (m_subjectDic.ContainsKey(dataType))
            {
                m_subjectDic[dataType].Detach(observer.OnDataChange);
            }
        }

        /// <summary>
        /// 返回一个BaseData的派生类
        /// </summary>
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
            //if (m_subjectDic.ContainsKey(data.dataType))
                m_subjectDic[data.dataType]?.Notify(data, type, obj);
        }
    }
}