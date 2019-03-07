using System.Collections;
using UnityEngine;

namespace BehaviorTree.ChainedMode
{
    /// <summary>
    /// 随机任务选择
    /// </summary>
    public class RandomSelector : Selector
    {
        private int taskIndex = 0;

        public RandomSelector(string name) : base(name) { }

        override public ReturnCode Update()
        {
            if (taskIndex >= tasks.Count)
                return ReturnCode.Fail;

            var returnCode = tasks[taskIndex].Update();
            if (returnCode == ReturnCode.Fail)
            {
                taskIndex = GetRandomIndex();
                return ReturnCode.Running;
            }
            else
            {
                return returnCode;
            }
        }

        public override void Restart()
        {
            base.Restart();

            taskIndex = GetRandomIndex();
        }

        private int GetRandomIndex()
        {
            var randomIndex = Random.Range(0, tasks.Count);
            if (randomIndex == tasks.Count)
                randomIndex = tasks.Count - 1;

            return randomIndex;
        }
    }
}