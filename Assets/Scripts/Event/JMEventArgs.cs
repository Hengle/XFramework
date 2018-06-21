using System;

public class JMEventArgs : EventArgs 
{
    public object[] data;
    public JMEventDispatchType eventType;

    public JMEventArgs(JMEventDispatchType eventType, params object[] data)
    {
        this.data = data;
        this.eventType = eventType;
    }
}
