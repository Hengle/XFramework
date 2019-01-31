using System;

public class SafeObjectPool<T> : Pool<T> where T : IPoolable, new()
{
    public SafeObjectPool()
    {
        mFactory = new DefultObjectFactory<T>();
    }

    /// <summary>
    /// Init the specified maxCount and initCount.
    /// </summary>
    /// <param name="maxCount">Max Cache count.</param>
    /// <param name="initCount">Init Cache count.</param>
    public void Init(int maxCount, int initCount)
    {
        MaxCacheCount = maxCount;

        if (maxCount > 0)
        {
            initCount = Math.Min(maxCount, initCount);
        }

        if (CurCount < initCount)
        {
            for (var i = CurCount; i < initCount; ++i)
            {
                Recycle(new T());
            }
        }
    }

    /// <summary>
    /// Gets or sets the max cache count.
    /// </summary>
    /// <value>The max cache count.</value>
    public int MaxCacheCount
    {
        get { return mMaxCount; }
        set
        {
            mMaxCount = value;

            if (mCacheStack != null)
            {
                if (mMaxCount > 0)
                {
                    if (mMaxCount < mCacheStack.Count)
                    {
                        int removeCount = mMaxCount - mCacheStack.Count;
                        while (removeCount > 0)
                        {
                            mCacheStack.Pop();
                            --removeCount;
                        }
                    }
                }
            }
        }
    }

    public override T Allocate()
    {
        T result = base.Allocate();
        result.IsRecycled = false;
        return result;
    }

    public override bool Recycle(T t)
    {
        if(t == null || t.IsRecycled)
        {
            return false;
        }

        t.IsRecycled = true;
        t.OnRecycled();
        mCacheStack.Push(t);

        return true;
    }
}
