using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFsm : FsmBase
{

}
public class MouseState : FsmState
{

}

public class State1 : MouseState
{
    public override void Init()
    {
        Debug.Log("State1Init");
    }

    public override void OnEnter()
    {
        Debug.Log("State1Enter");
    }

    public override void OnUpdate()
    {
        Debug.Log("State1Update");
    }

    public override void OnExit()
    {
        Debug.Log("State1Exit");
    }
}

public class State2 : MouseState
{
    public override void Init()
    {
        Debug.Log("State2Init");
    }
    public override void OnEnter()
    {
        Debug.Log("State2Enter");
    }

    public override void OnUpdate()
    {
        Debug.Log("State2Update");
    }

    public override void OnExit()
    {
        Debug.Log("State2Exit");
    }
}



public class QQQFsm : FsmBase
{

}
public class QQQState : FsmState
{

}

public class QQQ1 : MouseState
{
    public override void Init()
    {
        Debug.Log("QQQ1Init");
    }
    public override void OnEnter()
    {
        Debug.Log("QQQ1Enter");
    }

    public override void OnUpdate()
    {
        Debug.Log("QQQ1Update");
    }

    public override void OnExit()
    {
        Debug.Log("QQQ1Exit");
    }
}

public class QQQ2 : MouseState
{
    public override void Init()
    {
        Debug.Log("QQQ2Init");
    }
    public override void OnEnter()
    {
        Debug.Log("QQQ2Enter");
    }

    public override void OnUpdate()
    {
        Debug.Log("QQQ2Update");
    }

    public override void OnExit()
    {
        Debug.Log("QQQ2Exit");
    }
}