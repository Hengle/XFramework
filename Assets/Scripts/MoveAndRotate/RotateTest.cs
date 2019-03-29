using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 整合Unity中的旋转方式，有些会加上移动
/// </summary>
public class RotateTest : MonoBehaviour {

    public Transform targetTransform;
    public float angleSpeed = 5.0f;
    public Vector3 tempVector3 = Vector3.up;

	// Use this for initialization
	void Start () {
		 
	}
	
	// Update is called once per frame
	void Update () {
        //BaseRotate();
        RotateToTarget();
    }

    //基础旋转
    void BaseRotate()
    {
        //1.直接旋转，下面两个分别改变的是transform.localEulerAngles和transform.eulerAngles
        //transform.Rotate(Vector3.up * Time.deltaTime * angleSpeed);
        //transform.Rotate(Vector3.up * Time.deltaTime * angleSpeed, Space.World);

        //2.绕自身的某一轴旋转（不仅仅是x，y，z）,其中第一个所绕轴实际上是transform.up
        //transform.Rotate(Vector3.up, Time.deltaTime * angleSpeed);
        //transform.Rotate(Vector3.up, Time.deltaTime * angleSpeed, Space.World);

        //3.绕过场景中某一点的某个轴转动
        //transform.RotateAround(Vector3.zero, Vector3.up, Time.deltaTime * angleSpeed);

        //4.直接赋值位置和方向
        //transform.SetPositionAndRotation(targetTransform.position, targetTransform.rotation);
    }

    //给一个目标点，使物体朝向目标
    void RotateToTarget()
    {
        //1.LookAt
        //transform.LookAt(targetTransform);
        //transform.LookAt(targetTransform.position);

        //2.利用LookAt差值平缓旋转
        //Vector3 currentTarget = transform.position + transform.forward * 100;
        //Vector3 targetDir = targetTransform.position - transform.position;
        //Vector3 tempTarget = Vector3.Lerp(currentTarget, targetTransform.position, Time.deltaTime * angleSpeed);
        //transform.LookAt(new Vector3(tempTarget.x, transform.position.y, tempTarget.z));

        //改变transform.forward
        //Vector3 dir = targetTransform.position - transform.position;
        //transform.forward = Vector3.Lerp(transform.forward, dir, Time.deltaTime * angleSpeed);

        //利用四元数差值
        Vector3 dir2 = targetTransform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(dir2);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * angleSpeed);
    }
}
