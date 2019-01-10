using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TestForDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    RectTransform rt;

    void Start()
    {
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");
        //并将拖拽时的坐标给予被拖拽对象的代替品
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rt,
            eventData.position, eventData.pressEventCamera, out globalMousePos))
        {
            rt.position = globalMousePos;
        }

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
        rt = this.GetComponent<RectTransform>();
        rt.transform.SetParent(transform.root,false);
        Debug.Log(transform.root.name);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
        
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");
        //根据代替品的信息，改变当前对象的Sprite。
        GameObject obj = eventData.pointerDrag;
        this.GetComponent<Image>().sprite = obj.GetComponent<Image>().sprite;
        Debug.Log("Image已替换");
    }
}
