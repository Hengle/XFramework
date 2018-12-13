using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[Serializable]
public struct MoveSpeed : IComponentData
{
    public float Value;
}

//[DisallowMultipleComponent]
public class MoveSpeedComponent : ComponentDataWrapper<MoveSpeed>
{

}