using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XDEDZL.UI;

[DisallowMultipleComponent]
public class GameEntry : MonoBehaviour
{
    private BasePanel currentPanel;

    private void Update()
    {
        ModelUpdate(Time.deltaTime, Time.unscaledDeltaTime);
    }

    /// <summary>
    /// 每帧运行
    /// </summary>
    /// <param name="elapseSeconds">逻辑运行时间</param>
    /// <param name="realElapseSeconds">真实运行时间</param>
    private void ModelUpdate(float elapseSeconds, float realElapseSeconds)
    {

    }
}