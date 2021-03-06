﻿using System;
using System.Collections.Generic;

namespace XFramework
{
    public static class GameEntry
    {
        private static LinkedList<IGameModule> m_GameModules = new LinkedList<IGameModule>();

        /// <summary>
        /// 每帧运行
        /// </summary>
        /// <param name="elapseSeconds">逻辑运行时间</param>
        /// <param name="realElapseSeconds">真实运行时间</param>
        public static void ModelUpdate(float elapseSeconds, float realElapseSeconds)
        {
            foreach (var module in m_GameModules)
            {
                module.Update(elapseSeconds, realElapseSeconds);
            }
        }

        /// <summary>
        /// 获取一个模块
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetModule<T>() where T : IGameModule
        {
            Type moduleType = typeof(T);
            foreach (var module in m_GameModules)
            {
                if(module.GetType() == moduleType)
                {
                    return (T)module;
                }
            }

            return (T)CreateModule(moduleType);
        }

        /// <summary>
        /// 创建一个模块
        /// </summary>
        /// <param name="moduleType"></param>
        /// <returns></returns>
        private static IGameModule CreateModule(Type moduleType)
        {
            IGameModule module = (IGameModule)Activator.CreateInstance(moduleType);
            if (module == null)
            {
                throw new Exception(moduleType.Name + " is not a module");
            }

            LinkedListNode<IGameModule> current = m_GameModules.First;
            while (current != null)
            {
                if (module.Priority > current.Value.Priority)
                {
                    break;
                }

                current = current.Next;
            }

            if (current != null)
            {
                m_GameModules.AddBefore(current, module);
            }
            else
            {
                m_GameModules.AddLast(module);
            }

            return module;
        }
    }
}