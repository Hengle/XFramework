using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XDEDZL.Pool;
using XDEDZL;

public class PoolTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Game.PoolModel.CreateObjectPool<TestPoolObj>();
        TestPoolObj aa = Game.PoolModel.Allocate<TestPoolObj>();
        Debug.Log(aa.GetHashCode());
        Game.PoolModel.Recycle(aa);
        aa = Game.PoolModel.Allocate<TestPoolObj>();
        Game.PoolModel.Recycle(aa);
        Debug.Log(aa.GetHashCode());
        aa = Game.PoolModel.Allocate<TestPoolObj>();
        Game.PoolModel.Recycle(aa);
        Debug.Log(aa.GetHashCode());
        aa = Game.PoolModel.Allocate<TestPoolObj>();
        Game.PoolModel.Recycle(aa);
        Debug.Log(aa.GetHashCode());
        aa = Game.PoolModel.Allocate<TestPoolObj>();
        Game.PoolModel.Recycle(aa);
        Debug.Log(aa.GetHashCode());
        aa = Game.PoolModel.Allocate<TestPoolObj>();
        Game.PoolModel.Recycle(aa);
        Debug.Log(aa.GetHashCode());
        aa = Game.PoolModel.Allocate<TestPoolObj>();
        Game.PoolModel.Recycle(aa);
        Debug.Log(aa.GetHashCode());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public class TestPoolObj : IPoolable
    {
        public bool IsRecycled { get; set; }
        public bool IsLocked { get; set; }

        public void OnRecycled()
        {
            Debug.Log("我被回收了");
        }
    }
}
