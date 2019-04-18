using System;

namespace XDEDZL.Utility
{
    /// <summary>
    /// 反射相关的工具
    /// </summary>
    public static class ReflectionUtility
    {
        public static T CreateInstance<T>(params object[] objs) where T : class
        {
            T instance;
            if (objs != null)
                instance = Activator.CreateInstance(typeof(T), objs) as T;
            else
                instance = Activator.CreateInstance(typeof(T)) as T;
            return instance;
        }

        public static T CreateInstance<T>(Type type,params object[] objs) where T : class
        {
            T instance;
            if (objs != null)
                instance = Activator.CreateInstance(type, objs) as T;
            else
                instance = Activator.CreateInstance(type) as T;
            return instance;
        }
    }
}