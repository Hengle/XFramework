using System.Collections;
using System.Collections.Generic;
using XDEDZL.Utility;

public class FsmBase<TState> where TState : FsmState
{
    private readonly Dictionary<string, TState> stateDic;

    private TState currentState;

    public FsmBase()
    {
        stateDic = new Dictionary<string, TState>();
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

    public virtual void ChangeState<T>() where T : TState
    {
        if (currentState != null)
        {
            currentState.OnExit();
        }

        if (stateDic.ContainsKey(typeof(T).Name))
        {
            currentState = stateDic[typeof(T).Name];
        }
        else
        {
            currentState = CreateState<T>();
            stateDic.Add(typeof(T).Name, currentState);
        }

        currentState.OnEnter();
    }

    private TState CreateState<T>() where T : TState
    {
        TState state = ReflectionUtility.CreateInstance<T>();
        state.Init();
        return state;
    }
}