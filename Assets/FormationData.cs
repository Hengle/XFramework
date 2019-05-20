using System.IO;
using UnityEngine;

public class FormationData
{
    public string Name;
    /// <summary>
    /// 单位名称
    /// </summary>
    public string[] UnitNames;
    /// <summary>
    /// 对应一级单位数量
    /// </summary>
    public int[] UnitCounts;
    /// <summary>
    /// 能力范围类型
    /// </summary>
    public RangeType rangeType;

    public static FormationData[] GetData()
    {
        string path = Application.streamingAssetsPath + "/FormationConfig.json";
        if (!File.Exists(path))
        {
            throw new System.Exception("无预定义编组信息");
        }

        string json = File.ReadAllText(path);

        return Newtonsoft.Json.JsonConvert.DeserializeObject<FormationData[]>(json);
    }
}
public enum RangeType
{
    /// <summary>
    /// 无
    /// </summary>
    None,
    /// <summary>
    /// 攻击范围
    /// </summary>
    AttackRange,
    /// <summary>
    /// 防空火力范围
    /// </summary>
    AntiAircraftRange,
    /// <summary>
    /// 炮兵攻击范围
    /// </summary>
    AritillerRange,
    /// <summary>
    /// 空中单位范围
    /// </summary>
    AirForceRange,
}