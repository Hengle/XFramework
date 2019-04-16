using System;

namespace XDEDZL.Utility
{
    /// <summary>
    /// 反射相关的工具
    /// </summary>
    public static class ReflectionUtility
    {
        public static T CreateInstance<T>() where T : class
        {
            T instance = Activator.CreateInstance(typeof(T)) as T;
            return instance;
        }
    }
}