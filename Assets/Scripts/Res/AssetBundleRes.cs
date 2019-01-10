using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetBundleRes : MonoBehaviour
{
    public string name;
    AssetBundle ab;
    // Start is called before the first frame update
    void Start()
    {
        ab = AssetBundle.LoadFromFile("AssetBundle/test.ab");
        ab.Unload(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            GameObject obj = ab.LoadAsset(name) as GameObject;
            Instantiate(obj);
        }
        
    }
}
