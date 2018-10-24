using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandManager : MonoBehaviour
{
    /// <summary>
    /// 命令集
    /// </summary>
    private List<BaseCommand> commandSet;

    public CommandManager()
    {
        commandSet = new List<BaseCommand>();
    }

    /// <summary>
    /// 执行新的命令
    /// </summary>
    public void ExecutiveCommand(BaseCommand command)
    {
        commandSet.Add(command);
        command.ExecuteCommand();

        Debug.Log("执行命令：" + command.CommandDescribe);
    }

    /// <summary>
    /// 撤销上一个命令
    /// </summary>
    public void RevocationCommand()
    {
        if(commandSet.Count > 0)
        {
            BaseCommand command = commandSet[commandSet.Count - 1];
            commandSet.Remove(command);
            command.RevocationCommand();

            Debug.Log("撤销命令：" + command.CommandDescribe);
        }
    }
}
