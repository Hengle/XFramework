
/// <summary>
/// 池
/// </summary>
public interface IPool<T>
{
    T Allocate();
    bool Recycle(T obj);
}

/// <summary>
/// 对象工厂
/// </summary>
public interface IObjectFactory<T>
{
    T Create();
}

/// <summary>
/// 用于泛型约束，要实现SafeObjectPool的类必须继承这个接口
/// </summary>
public interface IPoolable
{
    /// <summary>
    /// 用于表示对象是否被回收
    /// </summary>
    bool IsRecycled { get; set; }

    void OnRecycled();
}

public interface IPoolType
{
    void Recycle2Cache();
}