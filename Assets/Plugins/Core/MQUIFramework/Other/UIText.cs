using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIText{

    /// <summary>
    /// UI描述枚举
    /// </summary>
    public enum UIDes
    {
        GroupDes,
        TeamDes,
        CommandDes
    }

    // 描述文字组
    private static string[] strDes = new string[2];

    /// <summary>
    /// 获取UI描述
    /// </summary>
    /// <param name="uiDes">描述类型</param>
    /// <returns>描述文字</returns>
    public static string[] GetUIDes(UIDes uiDes)
    {
        switch (uiDes)
        {
            case UIDes.GroupDes:
                strDes[0] = "五群";
                strDes[1] = "我是五群，我骄傲！";
                break;
            case UIDes.TeamDes:
                strDes[0] = "十八队";
                strDes[1] = "我是十八队，我自豪！";
                break;
            case UIDes.CommandDes:
                strDes[0] = "三所";
                strDes[1] = "我是三所，我傲娇！";
                break;
            default:
                break;
        }
        return strDes;
    }
}
