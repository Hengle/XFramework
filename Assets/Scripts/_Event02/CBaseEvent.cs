using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CEventType
{

}

public class CBaseEvent {

    protected Hashtable arguments;
    protected CEventType type;
    protected object sender;

    public CEventType Type
    {
        get { return type; }
        set { type = value; }
    }

    public IDictionary Params
    {
        get { return arguments; }
        set { arguments = value as Hashtable; }
    }

    public object Sender
    {
        get { return sender; }
        set { sender = value; }
    }

    public override string ToString()
    {
        return this.type + "[" + ((this.sender == null) ? "null" : this.sender.ToString()) + "]";
    }

    public CBaseEvent Clone()
    {
        return new CBaseEvent(this.type, this.arguments, this.sender);
    }

    public CBaseEvent(CEventType type,object sender)
    {
        this.type = type;
        this.sender = sender;
        if(this.arguments == null)
        {
            this.arguments = new Hashtable();
        }
    }

    public CBaseEvent(CEventType type, Hashtable args, object sender)
    {
        this.type = type;
        this.arguments = args;
        this.sender = sender;
        if (this.arguments == null)
        {
            this.arguments = new Hashtable();
        }
    }
}
