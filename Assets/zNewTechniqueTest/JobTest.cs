using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine.Jobs;
using System.Threading;

public class JobTest : MonoBehaviour
{
    TransformAccessArray accessArray;

    private void Start()
    {
        accessArray = new TransformAccessArray(transform.GetComponentsInChildren<Transform>());
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.J))
        {
            TestTransformJob();
        }
    }

    private void OnDisable()
    {
        accessArray.Dispose();
    }

    private void TestJob()
    {
        // 1.创建副本
        NativeArray<float> result = new NativeArray<float>(1, Allocator.TempJob);

        // 2.初始化Job数据
        MyJob jobData = new MyJob()
        {
            a = 10,
            b = 1,
            result = result,
        };

        // 3.调度作业
        JobHandle handle = jobData.Schedule();

        // 4.等待作业完成（如果Job没有依赖关系，每一个都要执行Complete，如果有依赖关系，只有一群有依赖关系的中的最后一个job需要执行）
        handle.Complete();

        // 5.释放内存
        result.Dispose();
    }

    private void TestParalleJob()
    {
        NativeArray<Vector3> wayPoints = new NativeArray<Vector3>(3, Allocator.TempJob);

        MyJobParallelFor myJobParallelFor = new MyJobParallelFor()
        {
            waypoints = wayPoints,
        };

        JobHandle handleParallelFor = myJobParallelFor.Schedule(3, 32);  // 依赖于handle，handle结束后才会开始工作
        handleParallelFor.Complete();
        wayPoints.Dispose();
    }

    private void TestTransformJob()
    {
        MyTransformJob myTransformJob = new MyTransformJob()
        {
            time = Time.time,
        };
        JobHandle handTransform = myTransformJob.Schedule(accessArray);
        handTransform.Complete();
    }
}

public struct MyJob : IJob
{
    public float a;
    public float b;
    public NativeArray<float> result;

    // 实现接口
    public void Execute()
    {
        result[0] = a + b;
        Debug.Log(result[0]);
    }
}

public struct MyJobParallelFor : IJobParallelFor
{
    public NativeArray<Vector3> waypoints;

    public void Execute(int index)
    {
        Debug.Log(waypoints[index]);
    }
}

public struct MyTransformJob : IJobParallelForTransform
{
    public float time;

    public void Execute(int index, TransformAccess transform)
    {
        transform.position += new Vector3(0, 0, 0.01f);
        Debug.Log(transform.position);
    }
}

public struct MyBatchJob : IJobParallelForBatch
{
    public void Execute(int startIndex, int count)
    {
        throw new System.NotImplementedException();
    }
}

public struct MyFilterJob : IJobParallelForFilter
{
    public bool Execute(int index)
    {
        throw new System.NotImplementedException();
    }
}