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
        BATTLE = 0,

        Test = 1,
    }

    /// <summary>
    /// 观察者数据积累，通过一些静态变量和函数进行管理
    /// </summary>
    public abstract class BaseData
    {
        protected BaseData() { }


        /// <summary>
        /// 这个在派生类中要重写，返回对应的类型
        /// </summary>
        public abstract DataType dataType
        {
            get;
        }

        /// <summary>
        /// 存储数据类和其type的字典
        /// </summary>
        private static Dictionary<Type, BaseData> s_uiDataByInstanceTypeDic = new Dictionary<Type, BaseData>();//类型和数据类对应的字典

        /// <summary>
        /// T应该是继承自BaseData，转化成T返回
        /// </summary>
        public static T DataConvert<T>(BaseData data) where T : class
        {
            T tDate = data as T;
            if (tDate == null)
                throw new ArgumentException(string.Format("类型转换失败:源类型: {0} 目标类型:{1}", data.GetType(), typeof(T)));
            return tDate;
        }

        /// <summary>
        /// 判断是否包含type类型
        /// </summary>
        private static bool ContainsKey(Type type)
        {
            return s_uiDataByInstanceTypeDic.ContainsKey(type);
        }

        /// <summary>
        /// 为字典添加类
        /// </summary>
        private static void AddData(BaseData data)
        {
            s_uiDataByInstanceTypeDic[data.GetType()] = data;
        }

        /// <summary>
        /// 从字典中取出一个BaseData的派生类
        /// </summary>
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
    }
}