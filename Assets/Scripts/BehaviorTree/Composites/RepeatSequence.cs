using BehaviorTree.ChainedMode;

/// <summary>
/// 循环队列
/// </summary>
public class RepeatSequence : CompositeTask
{
    private int taskIndex = 0;

    public RepeatSequence(string name) : base(name) { }

    public override ReturnCode Update()
    {
        if (tasks == null || tasks.Count == 0)
            return ReturnCode.Succeed;

        var returnCode = tasks[taskIndex].Update();
        if (returnCode == ReturnCode.Succeed)
        {
            taskIndex++;
            if (taskIndex >= tasks.Count)
                Restart();
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
