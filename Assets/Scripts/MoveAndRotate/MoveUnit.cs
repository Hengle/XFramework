using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveUnit : MonoBehaviour {

    public Transform targetTransform;
    public Vector3 targetPosition = Vector3.zero;
    public float moveSpeed = 1.0f;
    public float angleSpeed = 1.0f;
    private Vector3 targetDir;

    private Rigidbody rigibody;

	// Use this for initialization
	void Start () {
        rigibody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        //if(targetTransform != null)
        //{
        //    SetTarget(targetTransform);
        //}

        ScreenRay();

        //SetDir();
        Rotate();
        Move();
	}

    private void Move()
    {
        if (Mathf.Abs((targetPosition - transform.position).magnitude) > 10.0f)
        {
            //transform.position += transform.forward * moveSpeed * Time.deltaTime;
            rigibody.velocity = transform.forward * moveSpeed;
        }
    }

    public void SetTarget(Vector3 _position)
    {
        targetPosition = _position;
    }
    public void SetTarget(Transform _transform)
    {
        targetPosition = _transform.position;
    }

    public void SetDir()
    {
        Vector3 currentTarget = transform.position + transform.forward * 100;
        targetDir = targetPosition - transform.position;
        Vector3 tempTarget = Vector3.Lerp(currentTarget,targetPosition,Time.deltaTime * angleSpeed);
        transform.LookAt(new Vector3(tempTarget.x,transform.position.y,tempTarget.z));
    }

    private void Rotate()
    {
        Vector3 relativePos = targetPosition - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * angleSpeed);
    }

    private void ScreenRay()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//摄像机发射射线到屏幕点。
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                targetPosition = hitInfo.point;
            }
        }
        
    }
}
