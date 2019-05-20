// ==========================================
// 描述： 
// 作者： HAK
// 时间： 2018-10-25 09:07:06
// 版本： V 1.0
// ==========================================
using System.Collections.Generic;
using XFramework;

/// <summary>
/// 命令管理类，暂时只用于撤销命令
/// 单例
/// </summary>
public class CommandManager
{
    /// <summary>
    /// 数据集
    /// </summary>
    private Dictionary<CmdType, Stack<BaseCommandData>> dataDic;
    /// <summary>
    /// 命令集
    /// </summary>
    private Dictionary<CmdType, Stack<ICommand>> cmdDic;

    public CommandManager()
    {
        dataDic = new Dictionary<CmdType, Stack<BaseCommandData>>();
        cmdDic = new Dictionary<CmdType, Stack<ICommand>>();
    }

    public void AddCommand(CmdType type, ICommand cmd,BaseCommandData data)
    {
        if (!cmdDic.ContainsKey(type))
        {
            cmdDic.Add(type, new Stack<ICommand>());
            dataDic.Add(type, new Stack<BaseCommandData>());
        }
        cmdDic[type].Push(cmd);
        dataDic[type].Push(data);
    }

    /// <summary>
    /// 撤销上一个命令
    /// </summary>
    public void RevocationCommand(CmdType type)
    {
        Stack<ICommand> cmds = cmdDic.GetValue(type);
        if (cmds != null && cmds.Count > 0)
        {
            cmds.Pop().RevocationCommand(dataDic[type].Pop());
            if(cmds.Count == 0)
            {
                cmdDic.Remove(type);
                dataDic.Remove(type);
            }
        }
    }
}

public class BaseCommandData { }

public enum CmdType
{
    Range,
    Terrain,
}