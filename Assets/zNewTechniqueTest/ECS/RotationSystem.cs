using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class RotationSystem : ComponentSystem
{
    struct Group
    {
        public RotationComponent rotation;
        public Transform transform;
    }

    protected override void OnUpdate()
    {
        Debug.Log("ECS");
        foreach (var enitites in GetEntities<Group>())
        {
            enitites.transform.Rotate(Vector3.up * enitites.rotation.speed * Time.deltaTime);
        }
    }
}
