/// <summary>
/// 想要拥有撤回功能的模块都要实现这个接口
/// </summary>
public interface ICommand
{
    void SaveData();          // 存储数据
    void RevocationCommand(); // 撤销上一次的命令
}
