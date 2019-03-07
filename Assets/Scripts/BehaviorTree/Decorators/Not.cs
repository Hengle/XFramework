
namespace BehaviorTree.ChainedMode
{
    public class Not : DecoratorTask
    {
        public Not(string name) : base(name)
        {
        }

        public override ReturnCode Update()
        {
            var returnCode = task.Update();
            if (returnCode == ReturnCode.Fail)
                return ReturnCode.Succeed;
            else if (returnCode == ReturnCode.Succeed)
                return ReturnCode.Fail;
            else
                return ReturnCode.Running;
        }
    }
}