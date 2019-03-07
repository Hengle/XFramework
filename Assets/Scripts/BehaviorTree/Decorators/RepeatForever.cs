namespace BehaviorTree.ChainedMode
{
    /// <summary>
    /// 循环任务节点
    /// </summary>
    public class RepeatForever : DecoratorTask
    {
        public RepeatForever(string name) : base(name)
        {
        }

        public override ReturnCode Update()
        {
            if (task == null) return base.Update();

            if (task.Update() == ReturnCode.Succeed)
            {
                task.Restart();
            }

            return base.Update();
        }
    }
}