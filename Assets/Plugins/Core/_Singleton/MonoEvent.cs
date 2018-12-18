using System;

/// <summary>
/// Mono生命周期事件
/// 一些不继承Mono的类如果想在Mono生命周期做一些事，可以往这里添加
/// </summary>
public class MonoEvent : MonoSingleton<MonoEvent>
{
    public event Action UPDATE;
    public event Action FIXEDUPDATE;
    public event Action ONGUI;
    public event Action LATEUPDATE;

    private void Update()
    {
        UPDATE?.Invoke();
    }

    private void FixedUpdate()
    {
        FIXEDUPDATE?.Invoke();
    }

    private void OnGUI()
    {
        ONGUI?.Invoke();
    }

    private void LateUpdate()
    {
        LATEUPDATE?.Invoke();
    }
}