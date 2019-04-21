public abstract class ProcedureBase : FsmState
{

    protected void ChangeState<T>() where T : FsmState
    {
        FsmManager.Instance.ChangeState<ProcedureFsm, T>();
    }
}