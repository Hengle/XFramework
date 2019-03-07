﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 行为树节点
/// </summary>
public class BehaviorNode
{
    /// <summary>
    /// 子节点列表
    /// </summary>
    private List<BehaviorNode> childNodes;
    /// <summary>
    /// 父节点
    /// </summary>
    private BehaviorNode parentNode;

    protected virtual BehaviorState OnUpdate()
    {
        return BehaviorState.Suncced;
    }

    public void AddBehavior(BehaviorNode node)
    {
        node.parentNode = this;
        childNodes.Add(node);
    }

    public void RemoveBehavior(BehaviorNode node)
    {
        childNodes.Remove(node);
    }
}