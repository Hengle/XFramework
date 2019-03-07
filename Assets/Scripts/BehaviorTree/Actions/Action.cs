using UnityEngine;

namespace BehaviorTree.ChainedMode
{
    /// <summary>
    /// 行为任务节点
    /// </summary>
    public class Action : Task
    {
        private ActionDelegate actionDelegate;

        public Action(string name, ActionDelegate actionDelegate) : base(name)
        {
            this.actionDelegate = actionDelegate ?? throw new UnassignedReferenceException();
        }

        public override ReturnCode Update()
        {
            return actionDelegate();
        }
    }

    public delegate ReturnCode ActionDelegate();
}