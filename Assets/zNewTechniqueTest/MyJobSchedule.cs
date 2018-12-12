using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;

public class MyJobScheduler : MonoBehaviour
{
    Vector3[] waypoints;
    float offsetForWaypoints;

    //我们将保存结果和句柄的列表
    List<JobResultAndHandle> resultsAndHandles = new List<JobResultAndHandle>();

    void Update()
    {
        // 我们会在需要时创建新的JobResultANdHandle（该代码不必在Update方法中，因为它只是个示例）然后我们会给ScheduleJob方法提供引用。
        JobResultAndHandle newResultAndHandle = new JobResultAndHandle();
        ScheduleJob(ref newResultAndHandle);

        // 如果ResultAndHAndles的列表非空，我们会在该列表进行循环，了解是否有需要调用的作业。
        if (resultsAndHandles.Count > 0)
        {
            for (int i = 0; i < resultsAndHandles.Count; i++)
            {
                CompleteJob(resultsAndHandles[i]);
            }
        }
    }

    // ScheduleJob会获取JobResultAndHandle的引用，初始化并调度作业。
    void ScheduleJob(ref JobResultAndHandle resultAndHandle)
    {
        //我们会填充内置数组，设置合适的分配器
        resultAndHandle.waypoints = new NativeArray<Vector3>(waypoints, Allocator.TempJob);

        //我们会初始化作业，提供需要的数据
        MyJob newJob = new MyJob
        {
            waypoints = resultAndHandle.waypoints,
            offsetToAdd = offsetForWaypoints,
        };

        //设置作业句柄并调度作业
        resultAndHandle.handle = newJob.Schedule();
        resultsAndHandles.Add(resultAndHandle);
    }

    //完成后，我们会复制作业中处理的数据，然后弃用弃用内置数组
    //这一步很有必要，因为我们需要释放内存
    void CompleteJob(JobResultAndHandle resultAndHandle)
    {
        resultsAndHandles.Remove(resultAndHandle);
        resultAndHandle.handle.Complete();
        resultAndHandle.waypoints.CopyTo(waypoints);
        resultAndHandle.waypoints.Dispose();
    }
}

struct JobResultAndHandle
{
    public NativeArray<Vector3> waypoints;
    public JobHandle handle;
}
