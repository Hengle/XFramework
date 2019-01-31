using System;

public class SimpleObjectFactory<T> : Pool<T> where T : new ()
{
    readonly Action<T> mResetMethod;

    public SimpleObjectFactory(Func<T> factoryMethod, Action<T> resetMethod = null,int initCount = 0)
    {
        mFactory = new CustomObjectFactory<T>(factoryMethod);
        mResetMethod = resetMethod;

        for (int i = 0; i < initCount; i++)
        {
            mCacheStack.Push(mFactory.Create());
        }
    }

    public override bool Recycle(T obj)
    {
        mResetMethod.InvokeGracefully(obj);
        mCacheStack.Push(obj);
        return true;
    }
}
