using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree.ChainedMode
{
    public class Parallel : CompositeTask
    {
        private int taskIndex = 0;
        private int maxFailCount = 0;
        private int maxSuccessCount = 0;

        public Parallel(string name, int maxSuccessCount = 0, int maxFailCount = 0) : base(name)
        {
            this.maxFailCount = maxFailCount;
            this.maxSuccessCount = maxSuccessCount;
        }

        override public ReturnCode Update()
        {
            var returnCode = ReturnCode.Running;

            var failCount = 0;
            var successCount = 0;

            foreach (Task task in tasks)
            {
                returnCode = task.Update();

                if (returnCode == ReturnCode.Fail)
                {
                    failCount++;
                }
                else if (returnCode == ReturnCode.Succeed)
                {
                    successCount++;
                }
            }

            if (maxFailCount == 0)
                maxFailCount = tasks.Count;

            if (maxSuccessCount == 0)
                maxSuccessCount = tasks.Count;

            if (failCount > maxFailCount)
                return ReturnCode.Fail;

            if (successCount > maxSuccessCount)
                return ReturnCode.Succeed;

            return ReturnCode.Running;
        }

        public override void Restart()
        {
            taskIndex = 0;
            base.Restart();
        }
    }
}