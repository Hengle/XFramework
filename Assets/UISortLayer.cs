using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISortLayer : MonoBehaviour {

    private void Start()
    {
        Debug.Log(GetComponent<Canvas>().sortingLayerID);
        GetComponent<Canvas>().sortingLayerID = 2;
        Debug.Log(transform.gameObject.layer);
    }
}
