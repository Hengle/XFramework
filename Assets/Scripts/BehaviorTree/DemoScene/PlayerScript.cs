using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

    public float speed = 1;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");

        var position = transform.position;

        if (h < 0 && position.x < -5.7 || h > 0 && position.x > 5.7)
            h = 0;

        if (v < 0 && position.z < -4.7 || v > 0 && position.z > 4.7)
            v = 0;

        transform.position += new Vector3(h, 0, v) * speed * Time.deltaTime;
        
        print(transform.position);
	}
}
