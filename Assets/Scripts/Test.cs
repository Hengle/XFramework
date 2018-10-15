using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Test : MonoBehaviour {

    private void Start()
    {
        transform.position = Vector3.zero;
        Debug.Log(transform.position);
        transform.position = transform.position.WithX(100);
        Debug.Log(transform.position);
    }

    private void Update()
    {

    }
}
