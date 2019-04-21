using System;
using System.Collections.Generic;
using System.Reflection;

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

        public static List<Type> GetInterfaceSon(Type typeBase, string assemblyName = "Assembly-CSharp")
        {
            if (!typeBase.IsInterface)
            {
                throw new Exception("参数错误，应为接口");
            }

            List<Type> types = new List<Type>();
            Assembly assembly = Assembly.Load(assemblyName);
            if (assembly == null)
            {
                throw new Exception("没有找到程序集");
            }

            Type[] allType = assembly.GetTypes();
            foreach (Type type in allType)
            {
                if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeBase))
                {
                    types.Add(type);
                }
            }
            return types;
        }
    }
}