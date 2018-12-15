using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class RotationSystem : ComponentSystem
{
    struct Group
    {
        public readonly int Length;
        public ComponentArray<RotationComponent> rotationCubeComponents;
        //public RotationCubeComponent  rotation;
        //public Transform transform;
    }

    [Inject] Group group;

    protected override void OnUpdate()
    {
        //foreach (var enitites in GetEntities<Group>())
        //{
        //    enitites.transform.Rotate(Vector3.up * enitites.rotation.speed * Time.deltaTime);
        //}
        for (int i = 0; i < group.rotationCubeComponents.Length; i++)
        {
            group.rotationCubeComponents[i].transform.Rotate(Vector3.up * group.rotationCubeComponents[i].speed * Time.deltaTime);
        }
    }
}
