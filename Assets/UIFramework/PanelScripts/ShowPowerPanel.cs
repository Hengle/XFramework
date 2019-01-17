using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ShowPowerPanel : BasePanel {

    protected override void Awake()
    {
        base.Awake();
        level = UILevel.Two;
    }
}
