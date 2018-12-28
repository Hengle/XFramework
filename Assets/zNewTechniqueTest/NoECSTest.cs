using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoECSTest : MonoBehaviour
{
    public float speed = 1;

    private GameManager GM;

    private void Start()
    {
        GM = GameManager.GM;
    }

    void Update()
    {
        transform.Translate(new Vector3(0, 0,  - Time.deltaTime * speed));

        if (transform.position.z < GM.bottomBound)
            transform.position = transform.position.WithZ(GM.topBound);
    }
}
