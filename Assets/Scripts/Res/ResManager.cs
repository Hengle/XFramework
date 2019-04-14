using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResManager : Singleton<ResManager>
{
    public string name;
    AssetBundle ab;

    // Start is called before the first frame update
    void Start()
    {
        ab = AssetBundle.LoadFromFile("AssetBundle/test.ab");
        ab.Unload(false);
    }
}