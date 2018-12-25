using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class CountSystem : ComponentSystem
{
    struct Group
    {
        public readonly int Length;
        public ComponentDataArray<CountData> countData;
    }

    [Inject] Group group;

    protected override void OnUpdate()
    {
        for (int i = 0; i < group.Length; i++)
        {
            var countData = group.countData[i];
            countData.count++;
            group.countData[i] = countData;
        }
    }
}
