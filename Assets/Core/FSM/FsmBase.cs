using System.Collections.Generic;
using XDEDZL.Utility;

/// <summary>
/// 状态机基类
/// </summary>
/// <typeparam name="TState"></typeparam>
public class FsmBase
{
    private readonly Dictionary<string, FsmState> stateDic;

    private FsmState currentState;

    public FsmBase()
    {
        stateDic = new Dictionary<string, FsmState>();
        IsActive = true;
    }

    public virtual bool IsActive { get; private set; }
    public virtual void OnUpdate()
    {
        if (currentState != null)
        {
            currentState.OnUpdate();
        }
    }

    public virtual void ChangeState<T>() where T : FsmState
    {
        FsmState tempstate;
        if (stateDic.ContainsKey(typeof(T).Name))
        {
            tempstate = stateDic[typeof(T).Name];
        }
        else
        {
            tempstate = CreateState<T>();
            stateDic.Add(typeof(T).Name, tempstate);
        }

        if (currentState != tempstate)
        {
            currentState?.OnExit();
            currentState = tempstate;
            currentState.OnEnter();
        }
    }

    private FsmState CreateState<T>() where T : FsmState
    {
        FsmState state = ReflectionUtility.CreateInstance<T>();
        state.Init();
        return state;
    }
}