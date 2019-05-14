using System.Collections.Generic;
using UnityEngine;

namespace XDEDZL
{
    /// <summary>
    /// GL管理器
    /// </summary>
    public class GraphicsManager : IGameModule
    {
        private Dictionary<Camera, GraphicsMono> m_GraphicsDic;

        public GraphicsManager()
        {
            m_GraphicsDic = new Dictionary<Camera, GraphicsMono>();
        }

        public int Priority { get { return 10; } }

        public void Shutdown()
        {

        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {

        }

        public void AddGraphics(Camera camera, System.Action action)
        {
            if (m_GraphicsDic.ContainsKey(camera))
            {
                camera.GetComponent<GraphicsMono>().AddGraphics(action);
            }
            else
            {
                GraphicsMono temp = camera.gameObject.AddComponent<GraphicsMono>();
                m_GraphicsDic.Add(camera, temp);
                temp.AddGraphics(action);
            }
        }

        public void RemoveGraphics(Camera camera, System.Action action)
        {
            if (m_GraphicsDic.ContainsKey(camera))
            {
                m_GraphicsDic[camera].RemoveGraphics(action);
            }
        }
    }
}