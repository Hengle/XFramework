using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour {
    //Image

    float temp;
	void Update () {
        temp = Input.GetAxis("Mouse ScrollWheel");
        Debug.Log(temp);
    }

}
