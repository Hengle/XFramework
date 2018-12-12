using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

public class GManager : MonoBehaviour
{
    public static GManager GM;
    public int enemyShipIncremement;
    public int leftBound;
    public int rightBound;
    public int topBound;
    public int bottomBound;
    public int enemySpeed;

    private TransformAccessArray transforms;
    private MovementJob moveJob;
    private JobHandle moveHandle;

    private GameObject enemyShipPrefab;

    private void Start()
    {
        enemyShipPrefab = Resources.Load("Prefabs/Cube") as GameObject;
        GM = this;
        AddShips(enemyShipIncremement);
    }

    void Update()
    {
        moveHandle.Complete();   //确保主线程在计划任务完成之前不会继续执行

        if (Input.GetKeyDown("space"))
        {
            //AddShips(enemyShipIncremement);
        }

        moveJob = new MovementJob()
        {
            moveSpeed = enemySpeed,
            topBound = topBound,
            bottomBound = bottomBound,
            deltaTime = Time.deltaTime
        };
        moveHandle = moveJob.Schedule(transforms);
        JobHandle.ScheduleBatchedJobs();
    }

    void AddShips(int amount)
    {
        moveHandle.Complete();
        transforms.capacity = transforms.length + amount;

        for (int i = 0; i < amount; i++)
        {
            float xVal = Random.Range(leftBound, rightBound);
            float zVal = Random.Range(0f, 10f);

            Vector3 pos = new Vector3(xVal, 0f, zVal + topBound);
            Quaternion rot = Quaternion.Euler(0f, 180f, 0f);

            var obj = Instantiate(enemyShipPrefab, pos, rot) as GameObject;
        }
    }
}
