using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EasyPlayerController : MonoBehaviour
{
    public float speed;
    public float rotateSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float temp = speed * Time.deltaTime;
        transform.Translate(0, 0, temp * vertical, Space.Self);
        transform.Rotate(0, rotateSpeed * Time.deltaTime * horizontal, 0);
    }
}