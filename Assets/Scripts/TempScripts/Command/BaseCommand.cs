using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class BaseCommand
{
    /// <summary>
    /// 命令模式
    /// </summary>
    public string CommandDescribe { get; set; }

    /// <summary>
    /// 执行命令
    /// </summary>
    public virtual void ExecuteCommand()
    {

    }

    /// <summary>
    /// 撤销命令
    /// </summary>
    public virtual void RevocationCommand()
    {

    }
}


/// <summary>
/// InputField操作的命令
/// </summary>
public class InputFieldCommand : BaseCommand
{
    /// <summary>
    /// 目标
    /// </summary>
    private InputField commandTarget;
    /// <summary>
    /// 目标的值
    /// </summary>
    private string commandValue;

    public InputFieldCommand(InputField _commandTarget, string _commandValue, string _commandDescribe)
    {
        commandTarget = _commandTarget;
        commandValue = _commandValue;
        CommandDescribe = _commandDescribe;
    }

    /// <summary>
    /// 执行命令
    /// </summary>
    public override void ExecuteCommand()
    {
        base.ExecuteCommand();
        commandTarget.text = commandValue;
    }

    public override void RevocationCommand()
    {
        base.RevocationCommand();
        commandTarget.text = commandValue;
    }
} 

/// <summary>
/// Player操作的命令
/// </summary>
public class PlayerCommand : BaseCommand
{
    /// <summary>
    /// 目标
    /// </summary>
    private Player commandTarget;
    /// <summary>
    /// 目标的位置
    /// </summary>
    private Vector3 commandPosition;
    private Vector3 lastPosition;

    public PlayerCommand(Player _commandTarget, Vector3 _commandPosition, string _commandDescribe)
    {
        commandTarget = _commandTarget;
        commandPosition = _commandPosition;
        CommandDescribe = _commandDescribe;
    }

    /// <summary>
    /// 执行命令
    /// </summary>
    public override void ExecuteCommand()
    {
        base.ExecuteCommand();

        commandTarget.transform.position = commandPosition;
    }

    /// <summary>
    /// 撤销命令
    /// </summary>
    public override void RevocationCommand()
    {
        base.RevocationCommand();

        commandTarget.transform.position = commandPosition;
    }
}
