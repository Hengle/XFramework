using System;
using System.Collections.Generic;

namespace XDEDZL
{
    public static class GameEntry
    {
        private static LinkedList<GameModule> gameModules = new LinkedList<GameModule>();

        /// <summary>
        /// 每帧运行
        /// </summary>
        /// <param name="elapseSeconds">逻辑运行时间</param>
        /// <param name="realElapseSeconds">真实运行时间</param>
        public static void ModelUpdate(float elapseSeconds, float realElapseSeconds)
        {
            foreach (var module in gameModules)
            {
                module.Update(elapseSeconds, realElapseSeconds);
            }
        }

        public static GameModule GetModule<T>()
        {
            Type moduleType = typeof(T);
            foreach (var module in gameModules)
            {
                if(module.GetType() == moduleType)
                {
                    return module;
                }
            }

            return CreateModule(moduleType);
        }

        private static GameModule CreateModule(Type moduleType)
        {
            GameModule module = (GameModule)Activator.CreateInstance(moduleType);
            if (module == null)
            {
                throw new Exception(moduleType.Name + " is not a module");
            }

            LinkedListNode<GameModule> current = gameModules.First;
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
                gameModules.AddBefore(current, module);
            }
            else
            {
                gameModules.AddLast(module);
            }

            return module;
        }
    }
}