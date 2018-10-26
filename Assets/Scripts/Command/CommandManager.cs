// ==========================================
// 描述： 
// 作者： HAK
// 时间： 2018-10-25 09:07:06
// 版本： V 1.0
// ==========================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 命令管理类，暂时只用于撤销命令
/// 单例
/// </summary>
public class CommandManager : Singleton<CommandManager>
{
    /// <summary>
    /// 命令集
    /// </summary>
    private Stack<ICommand> commands;

    public CommandManager()
    {
        commands = new Stack<ICommand>();
    }

    public void AddCommand(ICommand cmd)
    {
        commands.Push(cmd);
    }

    /// <summary>
    /// 撤销上一个命令
    /// </summary>
    public void RevocationCommand()
    {
        if (commands.Count > 0)
        {
            ICommand command = commands.Pop();
            command.RevocationCommand();
        }
    }
}

public class BaseCommandData
{

}