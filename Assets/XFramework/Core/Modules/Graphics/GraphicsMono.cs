using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicsMono : MonoBehaviour
{
    private System.Action m_Action;

    private void OnPostRender()
    {
        m_Action?.Invoke();
    }

    public void AddGraphics(System.Action _action)
    {
        m_Action += _action;
    }

    public void RemoveGraphics(System.Action _action)
    {
        m_Action -= _action;
    }
}