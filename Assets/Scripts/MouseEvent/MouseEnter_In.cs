using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// unity自带的OnMouseEnter,Exit等鼠标事件在移出移入过于密集时不够灵敏，这种情况可以用这个类来做
/// </summary>
public class MouseEnter_In : MonoBehaviour {
    private Ray ray;
    private RaycastHit hitInfo;

    private Vector3 oldCameraPosition;
    private Vector3 curCameraPosition;
    private Vector3 oldMousePosition;
    private Vector3 curMousePosition;
    private Camera mainCamera;
    private bool cameraFlag;
    private bool mouseFlag;

    GameObject oldObj;
    GameObject curObj;

    public GameObject highLight;

    private void Start()
    {
        mainCamera = Camera.main;
        cameraFlag = false;
        mouseFlag = false;
    }

    private void Update()
    {
        //只有在鼠标移动或者是镜头移动时鼠标对应的物体才会发生改变，通过这两个判断减少射线发射次数
        MosueState();
        CameraState();

        if(mouseFlag != false || cameraFlag != false)
        {
            SenRay();
        }
    }

    private void SenRay()
    {
        ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitInfo, float.MaxValue, LayerMask.GetMask("Cylinder", "UnitRay")))
        {
            //鼠标进入
            

        }
        else if (curObj != null)
        {
            //鼠标移除
            
        }
    }

    #region 判断鼠标位置
    //获取相机状态
    private void CameraState()
    {
        curCameraPosition = mainCamera.transform.position;
        if (curCameraPosition != oldCameraPosition)
        {
            oldCameraPosition = curCameraPosition;
            cameraFlag = true;
        }
        else
        {
            cameraFlag = false;
        }
    }

    //获取相机状态
    private void MosueState()
    {
        curMousePosition = Input.mousePosition;
        if (curMousePosition != oldMousePosition)
        {
            oldMousePosition = curMousePosition;
            mouseFlag = true;
        }
        else
        {
            mouseFlag = false;
        }
    }
    #endregion

    //鼠标进入
    protected virtual void OnMovuseIn()
    {

    }
    //鼠标移出
    protected virtual void OnMovuseOut()
    {

    }
}
