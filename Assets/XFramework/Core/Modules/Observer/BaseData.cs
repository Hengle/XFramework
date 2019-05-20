///<summary>
///数据类基类
///<summary>
namespace XFramework
{
    //数据类型枚举
    public enum DataType
    {
        BATTLE = 0,
    }

    /// <summary>
    /// 观察者数据基类
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
    }
}