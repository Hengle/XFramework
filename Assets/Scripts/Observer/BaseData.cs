///<summary>
///���������,ֻ��һ��
///<summary>
namespace XDEDZL
{
    using System;
    using System.Collections.Generic;
    public enum DataType
    {
        /// <summary>
        /// ����
        /// </summary>
        TASK_LIST = 1,
        /// <summary>
        /// �����б�
        /// </summary>
        ITEM = 2,

        EVERYDAYTASK_LIST = 3,
        RANKING_LIST = 5,
        PUB = 6,
        ROLE = 7,
        ACTIVITY = 8,

        /// <summary>
        /// ͨ��RoleDataɢֵ
        /// </summary>
        COMMONROLEDATAHASHVALUE = 9,
    }



    public abstract class BaseData
    {
        private static Dictionary<Type, BaseData> s_uiDataByInstanceTypeDic = new Dictionary<Type, BaseData>();

        protected BaseData()
        { }

        public static T DataConvert<T>(BaseData data) where T : class
        {
            T tDate = data as T;
            if (tDate == null)
                throw new ArgumentException(string.Format("����ת��ʧ��:Դ����: {0} Ŀ������:{1}", data.GetType(), typeof(T)));
            return tDate;
        }

        private static bool ContainsKey(Type type)
        {
            return s_uiDataByInstanceTypeDic.ContainsKey(type);
        }

        private static void AddData(BaseData data)
        {
            s_uiDataByInstanceTypeDic[data.GetType()] = data;
        }

        public static T GetData<T>() where T : BaseData, new()
        {
            T data = null;
            Type type = typeof(T);
            if (ContainsKey(type))
            {
                data = s_uiDataByInstanceTypeDic[type] as T;
            }
            else
            {
                data = new T();
                s_uiDataByInstanceTypeDic[type] = data;
            }
            return data;
        }

        public abstract DataType dataType
        {
            get;
        }
    }
}