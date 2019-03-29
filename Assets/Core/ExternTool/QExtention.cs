using System;
/// <summary>
/// 在学习QFramework过程中需要用到的扩展函数，不想把所有东西都拷进来，后期根据需要选择是否留下
/// </summary>
[Obsolete]
public static class QExtention
{
    #region Action

    /// <summary>
    /// Call action
    /// </summary>
    /// <param name="selfAction"></param>
    /// <returns> call succeed</returns>
    public static bool InvokeGracefully(this Action selfAction)
    {
        if (null != selfAction)
        {
            selfAction();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Call action
    /// </summary>
    /// <param name="selfAction"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool InvokeGracefully<T>(this Action<T> selfAction, T t)
    {
        if (null != selfAction)
        {
            selfAction(t);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Call action
    /// </summary>
    /// <param name="selfAction"></param>
    /// <returns> call succeed</returns>
    public static bool InvokeGracefully<T, K>(this Action<T, K> selfAction, T t, K k)
    {
        if (null != selfAction)
        {
            selfAction(t, k);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Call delegate
    /// </summary>
    /// <param name="selfAction"></param>
    /// <returns> call suceed </returns>
    public static bool InvokeGracefully(this Delegate selfAction, params object[] args)
    {
        if (null != selfAction)
        {
            selfAction.DynamicInvoke(args);
            return true;
        }
        return false;
    }

    #endregion
}
