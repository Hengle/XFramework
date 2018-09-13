using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMono : MonoBehaviour {

    // Use this for initialization
    private static CanvasGroup canvasGroup;
    private static float targetAlpha;
    private static bool isRun = false;

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (isRun)
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime);
            if (canvasGroup.alpha < 0.05)
                isRun = false;
        }
	}

    public static void DoAlpha(CanvasGroup _canvasGroup, float _alpha)
    {
        isRun = true;
        canvasGroup = _canvasGroup;
        targetAlpha = _alpha;
    }
}
