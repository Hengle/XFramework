using System;

public class EventArgs //: EventArgs //循环继承？
{
    public object[] data;
    public EventDispatchType eventType;

    public EventArgs(EventDispatchType eventType, params object[] data)
    {
        this.data = data;
        this.eventType = eventType;
    }
}
