using System.Collections.Generic;

namespace BehaviorTree.ChainedMode
{
    /// <summary>
    /// 拥有多个子任务的复合任务
    /// </summary>
    public class CompositeTask : Task
    {
        public List<Task> tasks;

        public CompositeTask(string name) : base(name)
        {
            tasks = new List<Task>();
        }

        public void AddTask(Task task)
        {
            tasks.Add(task);
        }

        public override void Restart()
        {
            foreach (Task task in tasks)
            {
                task.Restart();
            }
            base.Restart();
        }
    }
}