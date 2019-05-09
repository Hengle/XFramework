public abstract class ProcedureBase : FsmState
{


    protected void ChangeState<T>() where T : ProcedureBase
    {
        FsmManager.Instance.ChangeState<ProcedureFsm, T>();
    }
}