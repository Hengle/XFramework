using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine.Jobs;

public class JobTest : MonoBehaviour
{
    private void Start()
    {
        // 创建单个浮点数的本地数组（NativeArray）来存储结果。为了更好说明功能，该示例会等待作业完成。
        NativeArray<float> result = new NativeArray<float>(1, Allocator.TempJob);

        // 设置作业数据
        MyJob jobData = new MyJob();
        jobData.a = 1;
        jobData.b = 1;
        jobData.result = result;

        // 调度作业
        JobHandle handle = jobData.Schedule();

        // 等待作业完成
        handle.Complete();

        //NativeArray的所有副本都指向相同内存，你可以在NativeArray的副本中访问结果。
        float aPlusB = result[0];
        Debug.Log(jobData.res);

        // 释放结果数组分配的内存
        result.Dispose();
    }
}

public struct MyJob : IJob
{

    /*在作业中，需要定义所有用于执行作业和输出结果的数据
    Unity会创建内置数组，它们大体上和普通数组差不多，但是需要自己处理分配和释放设置*/

    public NativeArray<Vector3> waypoints;
    public float offsetToAdd;
    public float a;
    public float b;
    public float res;
    public NativeArray<float> result;

    /*所有作业都需要Execute函数*/
    public void Execute()
    {
        /*该函数会保存行为。要执行的变量必须在该struct开头定义。*/
        //waypoints[i] = waypoints[i] * offsetToAdd;
        result = new NativeArray<float>();
        result[0] = a + b;
    }
}
