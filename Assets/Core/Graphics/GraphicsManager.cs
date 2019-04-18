using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GL管理器
/// </summary>
public class GraphicsManager : Singleton<GraphicsManager>
{
    private Dictionary<Camera, GraphicsMono> graphicsDic;

    private GraphicsManager()
    {
        graphicsDic = new Dictionary<Camera, GraphicsMono>();
    }

    public void AddGraphics(Camera camera,System.Action action)
    {
        if (graphicsDic.ContainsKey(camera))
        {
            camera.GetComponent<GraphicsMono>().AddGraphics(action);
        }
        else
        {
            GraphicsMono temp = camera.gameObject.AddComponent<GraphicsMono>();
            graphicsDic.Add(camera, temp);
            temp.AddGraphics(action);
        }
    }

    public void RemoveGraphics(Camera camera,System.Action action)
    {
        if (graphicsDic.ContainsKey(camera))
        {
            graphicsDic[camera].RemoveGraphics(action);
        }
    }
}