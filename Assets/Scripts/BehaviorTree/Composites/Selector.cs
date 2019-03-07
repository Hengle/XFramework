namespace BehaviorTree.ChainedMode
{
    /// <summary>
    /// 选择性任务
    /// </summary>
    public class Selector : CompositeTask
    {
        private int taskIndex = 0;

        public Selector(string name) : base(name) { }

        override public ReturnCode Update()
        {
            var returnCode = tasks[taskIndex].Update();
            if (returnCode == ReturnCode.Fail)
            {
                taskIndex++;
                if (taskIndex > tasks.Count)
                    return ReturnCode.Fail;
                else
                    return ReturnCode.Running;
            }
            else
            {
                return returnCode;
            }
        }
    }
}