// ==========================================
// 描述： 
// 作者： HAK
// 时间： 2018-10-25 09:07:20
// 版本： V 1.0
// ==========================================

using System.Collections.Generic;
/// <summary>
/// 想要拥有撤回功能的模块都要实现这个接口
/// </summary>
public interface ICommand
{
    void SaveData();          // 存储数据
    void RevocationCommand(BaseCommandData data); // 撤销上一次的命令
}

public interface ICommand<T>:ICommand
{
    void RevocationCommand(T parm);
}
