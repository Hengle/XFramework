// ==========================================
// 描述： 
// 作者： HAK
// 时间： 2018-10-16 16:00:41
// 版本： V 1.0
// ==========================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 存储单位信息
/// </summary>
public class UnitInfo {

    /// <summary>
    /// 血量
    /// </summary>
    public int ph;
    /// <summary>
    /// 通过这个hash值可以找到对应Formationinfo
    /// </summary>
    public int formationKey;
    /// <summary>
    /// 阵营
    /// </summary>
    public int camp;
    /// <summary>
    /// 单位的攻击距离
    /// </summary>
    public float attackDis;
    /// <summary>
    /// 单位名（预制体名）
    /// </summary>
    public string className;
    /// <summary>
    /// 单位的HashCode
    /// </summary>
    public int unitHashCode;

    public UnitInfo()
    {
        ph = 100;
        formationKey = 0;
        camp = 1000;
        attackDis = 1700;
        className = "";
    }
}
