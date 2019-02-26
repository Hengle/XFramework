using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager GM;

    [Header("Simulation Setting")]
    public float topBound = 16.5f;
    public float bottomBound = -13.5f;
    public float leftBound = -23.5f;
    public float rightBound = -23.5f;

    [Header("Enemy Setting")]
    public GameObject enemyShipPrefab;
    public float enemySpeed = 1f;

    [Header("Spwan Setting")]
    public int enemyShipCount = 1;
    public int enemyShipIncremement = 1;

    //FPS fps;
    private int count;

    private EntityManager manager;

    // ------------------------------- //

    public Material mat;
    public Mesh mesh;

    public GameObject monoShip;

    private void Awake()
    {
        if (GM == null)
            GM = this;
        //else if (GM != this)
        //    Destroy(gameObject);
    }

    private void Start()
    {
        //fps = GetComponent<FPS>();
        manager = World.Active.GetOrCreateManager<EntityManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddShips(enemyShipCount);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            AddMonoShip(enemyShipCount);
        }
    }
    
    private void AddShips(int amount)
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

        // ---------------------------------- //


        //EntityArchetype Cube;
        //Cube = manager.CreateArchetype(typeof(Position), typeof(Rotation), typeof(MoveSpeed), typeof(MeshInstanceRenderer));

        //MeshInstanceRenderer MSI = new MeshInstanceRenderer();
        //MSI.material = mat;
        //MSI.mesh = mesh;

        //for (int i = 0; i < amount; i++)
        //{
        //    float xVal = Random.Range(leftBound, rightBound);
        //    float zVal = Random.Range(0f, 10f);
        //    Entity cubeEnity = manager.CreateEntity(Cube);
        //    manager.SetComponentData(cubeEnity, new Position { Value = new float3(xVal, 0f, topBound + zVal) });
        //    manager.SetComponentData(cubeEnity, new Rotation { Value = new quaternion(0, 1, 0, 0) });
        //    manager.SetComponentData(cubeEnity, new MoveSpeed { Value = enemySpeed });
        //    manager.SetSharedComponentData(cubeEnity, MSI);
        //}

        // ---------------------------------- //

        count += amount;

    }

    private void AddMonoShip(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            float xVal = Random.Range(GM.leftBound, GM.rightBound);
            float zVal = Random.Range(0f, 10f);
            Instantiate(monoShip, new Vector3(xVal, 0, zVal), Quaternion.identity);
        }
        count += amount;
        print(count);
    }
}
