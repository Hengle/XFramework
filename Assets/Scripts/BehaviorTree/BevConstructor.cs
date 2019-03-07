
namespace BehaviorTree.ChainedMode
{
    /// <summary>
    /// 行为树链式构造器
    /// </summary>
    public class BevConstructor
    {
        /// <summary>
        /// 父节点构造器
        /// </summary>
        public BevConstructor parentBuilder;
        /// <summary>
        /// 父节点任务
        /// </summary>
        public Task parentTask;
        /// <summary>
        /// 当前构造器任务节点
        /// </summary>
        private Task task;

        /// <summary>
        /// 添加循环任务节点
        /// </summary>
        public BevConstructor RepeatForever(string name)
        {
            var repeatForever = new RepeatForever(name);
            AddTask(repeatForever);
            return CreateChildTreeBuilder(repeatForever);
        }

        /// <summary>
        /// 添加顺序队列节点
        /// </summary>
        public BevConstructor Sequence(string name)
        {
            var sequence = new Sequence(name);
            AddTask(sequence);
            return CreateChildTreeBuilder(sequence);
        }

        public BevConstructor ReperatSequence(string name)
        {
            var repeartSequence = new RepeatSequence(name);
            AddTask(repeartSequence);
            return CreateChildTreeBuilder(repeartSequence);
        }

        /// <summary>
        /// 添加随机选择任务节点
        /// </summary>
        public BevConstructor RandomSelector(string name)
        {
            var selector = new RandomSelector(name);

            AddTask(selector);

            return CreateChildTreeBuilder(selector);
        }

        /// <summary>
        /// 添加行为任务
        /// </summary>
        public BevConstructor Action(string name, ActionDelegate action)
        {
            return Action(new Action(name, action));
        }

        /// <summary>
        /// 添加行为任务
        /// </summary>
        public BevConstructor Action(Action action)
        {
            AddTask(action);

            return this;
        }

        /// <summary>
        /// 添加条件任务节点
        /// </summary>
        public BevConstructor Conditional(string name,ConditionalDelegate conditional)
        {
            return Conditional(new Conditional(name, conditional));
        }

        /// <summary>
        /// 添加条件任务节点
        /// </summary>
        public BevConstructor Conditional(Conditional conditional)
        {
            AddTask(conditional);
            return this;
        }

        /// <summary>
        /// 给节点添加任务
        /// </summary>
        private void AddTask(Task task)
        {
            if (parentTask is DecoratorTask dt)
            {
                dt.AddTask(task);
            }
            else if (parentTask is CompositeTask ct)
            {
                ct.AddTask(task);
            }
        }

        /// <summary>
        /// 子节点任务构造器
        /// </summary>
        private BevConstructor CreateChildTreeBuilder(Task task)
        {
            var bt = new BevConstructor
            {
                parentTask = task,
                parentBuilder = this
            };

            this.task = task;

            return bt;
        }

        /// <summary>
        /// 结束一个复合任务的构造
        /// </summary>
        public BevConstructor End()
        {
            return this.parentBuilder;
        }

        /// <summary>
        /// 构造结束，返回任务根节点
        /// </summary>
        public Task Build()
        {
            return task;
        }
    }
}