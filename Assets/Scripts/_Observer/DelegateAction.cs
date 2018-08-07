//=====================================================================================/
///<summary>
///delegate
///<summary>
//=====================================================================================/
namespace XDEDZL
{
    public delegate void DelegateAction();
    public delegate void DelegateAction<T>(T arg);
    public delegate void DelegateAction<T, U>(T arg1, U arg2);
}
