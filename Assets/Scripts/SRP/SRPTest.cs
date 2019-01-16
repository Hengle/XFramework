using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SRPTest : MonoBehaviour
{

    private RenderTexture rt;
    public Transform[] cubeTransforms;
    public Mesh cubeMesh;
    public Material pureColorMaterail;

    void Start()
    {
        rt = new RenderTexture(Screen.width, Screen.height, 24);
    }

    // Update is called once per frame
    void OnPostRender()
    {
        Camera cam = Camera.current;
        Graphics.SetRenderTarget(rt);
        GL.Clear(true, true, Color.gray);

        // Start DrawCall
        pureColorMaterail.color = new Color(0, .5f, 0.8f);
        pureColorMaterail.SetPass(0);
        foreach (var item in cubeTransforms)
        {
            Graphics.DrawMeshNow(cubeMesh, item.localToWorldMatrix);
        }
        // End DrawCall


        Graphics.Blit(rt, cam.targetTexture);
    }
}