using System.Collections.Generic;
using System;

namespace XDEDZL.Pool
{
    public partial class ObjectPoolManager : IGameModule
    {
        private Dictionary<string, PoolBase> m_ObjectPools;

        public ObjectPoolManager()
        {
            m_ObjectPools = new Dictionary<string, PoolBase>();
        }

        /// <summary>
        /// 是否有T类型的对象池
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public bool HasPool<T>() where T : IPoolable, new()
        {
            return HasPool(typeof(T).Name);
        }

        public bool HasPool(string poolName)
        {
            if (m_ObjectPools.ContainsKey(poolName))
                return true;
            return false;
        }

        /// <summary>
        /// 创建一个T类型的对象池
        /// </summary>
        /// <typeparam name="T">对象池类型</typeparam>
        /// <param name="initCount">初始数量</param>
        /// <param name="maxCount">最大数量</param>
        public void CreateObjectPool<T>(int initCount,int maxCount) where T : IPoolable, new()
        {
            if (HasPool<T>())
                return;

            Pool<T> pool = new Pool<T>(initCount,maxCount);
            m_ObjectPools.Add(typeof(T).Name, pool);
        }

        public bool DestoryPool<T>() where T : IPoolable, new()
        {
            if (HasPool<T>())
            {
                // 销毁这个对象池
                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取一个对象池
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public Pool<T> GetPool<T>() where T : IPoolable, new()
        {
            return (Pool<T>)GetPool(typeof(T).Name);
        }

        public PoolBase GetPool(string poolName)
        {
            if (HasPool(poolName))
            {
                return m_ObjectPools[poolName];
            }
            return null;
        }

        /// <summary>
        /// 分配一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public T Allocate<T>() where T : IPoolable, new()
        {
            if (HasPool<T>())
            {
                return ((Pool<T>)m_ObjectPools[typeof(T).Name]).Allocate();
            }

            throw new Exception("试图在对象池创建前获取对象");
        }

        #region 接口实现

        public int Priority { get { return 2; } }

        public void Init()
        {
            throw new System.NotImplementedException();
        }

        public void Shutdown()
        {
            throw new System.NotImplementedException();
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            // TODO每隔一定时间清理一次对象池
        }

        #endregion
    }
}