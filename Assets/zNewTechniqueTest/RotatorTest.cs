using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatorTest : MonoBehaviour
{
    public float speed;

    private void Update()
    {
        transform.Rotate(0f,speed * Time.deltaTime, 0f);
    }
}
