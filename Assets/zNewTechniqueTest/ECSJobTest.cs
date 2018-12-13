using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using System;
using Unity.Collections;
using Random = UnityEngine.Random;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;

public class ECSJobTest : MonoBehaviour
{
    EntityManager manager;
    public static ECSJobTest GM;

    public int enemyShipIncremement;
    public int leftBound;
    public int rightBound;
    public int topBound;
    public int bottomBound;
    public int enemySpeed;

    private GameObject enemyShipPrefab;

    // Start is called before the first frame update
    void Start()
    {
        GM = this;
        enemyShipPrefab = Resources.Load("Prefabs/Cube") as GameObject;
        manager = World.Active.GetOrCreateManager<EntityManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            Debug.Log(1);
            AddShips(enemyShipIncremement);
            Debug.Log(2);
        }
            

    }

    void AddShips(int amount)
    {
        NativeArray<Entity> entities = new NativeArray<Entity>(amount, Allocator.Temp);
        manager.Instantiate(enemyShipPrefab, entities);

        for (int i = 0; i < amount; i++)
        {
            float xVal = Random.Range(leftBound, rightBound);
            float zVal = Random.Range(0f, 10f);
            manager.SetComponentData(entities[i], new Position { Value = new float3(xVal, 0f, topBound + zVal) });
            manager.SetComponentData(entities[i], new Rotation { Value = new quaternion(0, 1, 0, 0) });
            manager.SetComponentData(entities[i], new MoveSpeed { Value = enemySpeed });
        }
        entities.Dispose();
    }
}

public class MoveMentSystem : JobComponentSystem
{
    [BurstCompile]
    struct MovementJob : IJobProcessComponentData<Position, Rotation, MoveSpeed>
    {
        public float topBound;
        public float bottomBound;
        public float deltaTime;

        public void Execute(ref Position position,[ReadOnly] ref Rotation rotation,[ReadOnly] ref MoveSpeed speed)
        {
            float3 value = position.Value;
            value += deltaTime * speed.Value * math.forward(rotation.Value);
            if (value.z < bottomBound)
                value.z = topBound;
            position.Value = value;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        MovementJob moveJob = new MovementJob
        {
            topBound = ECSJobTest.GM.topBound,
            bottomBound = ECSJobTest.GM.bottomBound,
            deltaTime = Time.deltaTime,
        };

        JobHandle moveHandle = moveJob.Schedule(this, inputDeps);
        return moveHandle;
    }
}