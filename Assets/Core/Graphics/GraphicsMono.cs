using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicsMono : MonoBehaviour
{
    private System.Action action;

    private void OnPostRender()
    {
        action?.Invoke();
    }

    public void AddGraphics(System.Action _action)
    {
        action += _action;
    }

    public void RemoveGraphics(System.Action _action)
    {
        action -= _action;
    }
}