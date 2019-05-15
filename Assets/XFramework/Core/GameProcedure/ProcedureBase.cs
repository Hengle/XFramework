namespace XDEDZL
{
    public abstract class ProcedureBase : FsmState
    {


        protected void ChangeState<T>() where T : ProcedureBase
        {
            Game.FsmModel.ChangeState<ProcedureFsm, T>();
        }
    }
}