//=====================================================================================/
///<summary>
///���� �ж��ٸ�basedata���� �ͻ��ж�������
///<summary>
//=====================================================================================/
namespace XDEDZL
{
    //using UI;
    using System.Collections.Generic;

    public class DataSubjectManager : SingletonTemplate<DataSubjectManager> //��ģ��Ӧ��ʹ��JM�ڵ� �����Ժ���ֲ,��ʱ����д
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
        /// �������ݼ���
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
        /// ֪ͨ�¼�
        /// </summary>
        /// <param name="data">data����</param>
        /// <param name="type">�¼�����</param>
        /// <param name="obj">ӳ�����</param>
        public void Notify(BaseData data, int type = 0, object obj = null)
        {
            if (m_subjectDic.ContainsKey(data.dataType))
                m_subjectDic[data.dataType].subject.Notify(data, type, obj);
            //else
                //GameDebugLog.Log(data.dataType.ToString());
        }
    }
}