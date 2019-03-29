/*
 * 想在自己的框架里写几种不同的UI管理方式
 * 外部通过UIHelper调用IUIManager
 * 在UIHelper中觉得以哪一种UIManager中管理
 * 实际项目中UI管理器以接口的形式出现可以方便的进行扩展，但在大多数情况下，一种UI管理方式就够了，可以删除UIHelper和IUImanager，直接使用UIManager
 */

/// <summary>
/// 
/// </summary>
public interface IUImanager
{
    void OpenPanel(string name);
    void ClosePanel(string name);
    BasePanel GetPanel(string name);
}
