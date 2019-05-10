namespace XDEDZL
{
    /// <summary>
    /// 模块基类
    /// </summary>
    public abstract class GameModule
    {
        /// <summary>
        /// 模块优先级
        /// </summary>
        /// <remarks>优先级较高的模块会优先轮询</remarks>
        internal virtual int Priority
        {
            get
            {
                return 0;
            }
        }
        /// <summary>
        /// 启用一个游戏模块
        /// </summary>
        internal abstract void Init();
        /// <summary>
        /// 游戏框架模块轮询
        /// </summary>
        /// <param name="elapseSeconds">逻辑运行时间，以秒为单位</param>
        /// <param name="realElapseSeconds">真实运行时间，以秒为单位</param>
        internal abstract void Update(float elapseSeconds, float realElapseSeconds);
        /// <summary>
        /// 关闭模块
        /// </summary>
        internal abstract void Shutdown();
    }
}