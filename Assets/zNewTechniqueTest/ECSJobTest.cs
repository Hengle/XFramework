using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using System;
using Unity.Collections;
using Random = UnityEngine.Random;
using Unity.Transforms;
using Unity.Mathematics;

public class ECSJobTest : MonoBehaviour
{
    EntityManager manager;

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
        enemyShipPrefab = Resources.Load("Prefabs/Cube") as GameObject;
        manager = World.Active.GetOrCreateManager<EntityManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
              AddShips(enemyShipIncremement);

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

[Serializable]
public struct MoveSpeed : IComponentData
{
    public float Value;
}

public class MoveSpeedComponent : ComponentDataWrapper<MoveSpeed>
{

}
