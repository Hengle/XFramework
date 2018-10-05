using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Test : MonoBehaviour {

    private EventTrigger eventTrigger;
    private BaseEventData data;
    private AxisEventData aixsData;
    private PointerEventData pointDat;
    private Button aa;


    private void Start()
    {
        eventTrigger = GetComponent<EventTrigger>();
        eventTrigger.AddOnBeginDrag(() => { });
        eventTrigger.AddOnCancel(() => { });
        eventTrigger.AddOnDeselect(() => { });
        eventTrigger.AddOnDrag(() => { });
        eventTrigger.AddOnDrop(() => { });
        eventTrigger.AddOnEndDrag(() => { });
        eventTrigger.AddOnInitializePotentialDrag(() => { });
        eventTrigger.AddOnMove((data) => { Debug.Log((data as AxisEventData).moveDir) ; });
        eventTrigger.AddOnPointerClick(() => { Debug.Log("Click"); });
        eventTrigger.AddOnPointerDown(() => { });
        eventTrigger.AddOnPointerEnter((data) => { Debug.Log(data.currentInputModule); });
        eventTrigger.AddOnPointerExit(() => { });
        eventTrigger.AddOnPointerUp(() => { });
        eventTrigger.AddOnScroll(() => { });
        eventTrigger.AddOnSelect(() => { });
        eventTrigger.AddOnSubmit(() => { });
        eventTrigger.AddOnUpdateSelect(() => { });
    }
}
