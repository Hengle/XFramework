//=====================================================================================/
///<summary>
///delegate
///<summary>
//=====================================================================================/
namespace JM
{
    public delegate void JMDelegateAction();
    public delegate void JMDelegateAction<T>(T arg);
    public delegate void JMDelegateAction<T, U>(T arg1, U arg2);
}
