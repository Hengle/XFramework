using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 资源加载回调
/// </summary>
public interface IResLoadListener
{
    void Finish(object asset);
    void Failure();
}