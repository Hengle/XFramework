///<summary>
///数据类基类,只有一份
///<summary>
namespace XDEDZL
{
    using System;
    using System.Collections.Generic;

    //数据类型枚举
    public enum DataType
    {
        /// <summary>
        /// 任务
        /// </summary>
        TASK_LIST = 1,
        /// <summary>
        /// 背包列表
        /// </summary>
        ITEM = 2,

        EVERYDAYTASK_LIST = 3,
        RANKING_LIST = 5,
        PUB = 6,
        ROLE = 7,
        ACTIVITY = 8,

        /// <summary>
        /// 通用RoleData散值
        /// </summary>
        COMMONROLEDATAHASHVALUE = 9,
    }



    public abstract class BaseData
    {
        private static Dictionary<Type, BaseData> s_uiDataByInstanceTypeDic = new Dictionary<Type, BaseData>();//类型和数据类对应的字典

        protected BaseData()
        { }

        //T应该是继承自BaseData，转话成T返回
        public static T DataConvert<T>(BaseData data) where T : class
        {
            T tDate = data as T;
            if (tDate == null)
                throw new ArgumentException(string.Format("类型转换失败:源类型: {0} 目标类型:{1}", data.GetType(), typeof(T)));
            return tDate;
        }

        //判断是否包含type类型
        private static bool ContainsKey(Type type)
        {
            return s_uiDataByInstanceTypeDic.ContainsKey(type);
        }

        //为字典添加类
        private static void AddData(BaseData data)
        {
            s_uiDataByInstanceTypeDic[data.GetType()] = data;
        }

        //new()限定T必须有一个无参构造函数
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