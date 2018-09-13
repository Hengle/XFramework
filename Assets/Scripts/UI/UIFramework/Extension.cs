using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extension {

    public static void DoAlpha(this CanvasGroup canvasGroup, float alpha)
    {
        UIMono.DoAlpha(canvasGroup, alpha);
    }
}
