//=====================================================================================/
/// <summary>
/// 所有事件DispatchType
/// </summary>
//=====================================================================================.
public enum EventDispatchType
{
    EVENT_CREATE_COMBATUNIT,    //单元创建


    //技能
    EVENT_ANIMATION_START,
    EVENT_ANIMATION_END,
    /// <summary>
    /// 计时器运行帧
    /// </summary>
    EVENT_TIMER,
    /// <summary>
    /// 计时器运行状态改变
    /// </summary>
    EVENT_TIME_RUNCHANGE,
    //Task
    EVENT_TASK_UPDATE,
    EVENT_TASK_FINSH,
    EVENT_TASK_CANCREATE,//可接任务

    //寻路
    EVENT_AUTODES_STATE,//开始寻路


    //动画帧事件部分
    EVENT_ANIMATION_CAST,
    EVENT_ANIMATION_PLAYAUDIO,
    EVENT_ANIMATION_SHADOW,

}