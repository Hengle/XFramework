using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree.ChainedMode
{
    /// <summary>
    /// 顺序执行的任务队列
    /// </summary>
    public class Sequence : CompositeTask
    {
        private int taskIndex = 0;

        public Sequence(string name) : base(name) { }

        public override ReturnCode Update()
        {
            if (tasks == null || tasks.Count == 0)
                return ReturnCode.Succeed;

            var returnCode = tasks[taskIndex].Update();
            if (returnCode == ReturnCode.Succeed)
            {
                taskIndex++;
                // 所有子任务节点执行完后队列执行完毕
                if (taskIndex >= tasks.Count)
                    return ReturnCode.Succeed;
                else
                    return ReturnCode.Running;
            }
            else
            {
                return returnCode;
            }
        }

        public override void Restart()
        {
            taskIndex = 0;
            base.Restart();
        }
    }
}