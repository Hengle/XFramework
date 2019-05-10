using System.Collections.Generic;

namespace XDEDZL.Pool
{
    public abstract class Pool<T> : Singleton<Pool<T>>, IPool<T>
    {
        public int CurCount
        {
            get { return mCacheStack.Count; }
        }

        protected IObjectFactory<T> mFactory;

        protected Stack<T> mCacheStack = new Stack<T>();

        protected int mMaxCount = 5;

        public virtual T Allocate()
        {
            return mCacheStack.Count == 0 ? mFactory.Create() : mCacheStack.Pop();
        }

        public abstract bool Recycle(T obj);
    }
}