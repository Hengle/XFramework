using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// 需要相应拖拽，点击等UI事件的类可以继承此基类
/// </summary>
public class Operation : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IDropHandler
{
    public delegate void OnOperation(PointerEventData eventData);
    private event OnOperation OnPointerClickCallBack;
    private event OnOperation OnDragCallBack;
    private event OnOperation OnBeginDragCallBack;
    private event OnOperation OnEndDragCallBack;
    private event OnOperation OnPointerDownCallBack;
    private event OnOperation OnPointerUpCallBack;
    private event OnOperation OnPointerExitCallBack;
    private event OnOperation OnPointerLongPressCallBack;
    private event OnOperation OnDropCallBack;

    private bool bIsDraging = false;

    private float intervalTime = 0.05f;                                 //回调触发间隔时间;
    private float delay = 0;                                        //延迟时间;
    public UnityEvent onLongPress = new UnityEvent();
    private bool isPointDown = false;
    private float lastInvokeTime;
    private float m_Delay = 0f;

    void Start()
    {
        m_Delay = delay;
    }

    void Update()
    {
        if (isPointDown)
        {
            if ((m_Delay -= Time.deltaTime) > 0f)
                return;
            if (Time.time - lastInvokeTime > intervalTime)
            {
                //触发点击;  
                OnPointerLongPress();
                lastInvokeTime = Time.time;
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (bIsDraging)
        {
            bIsDraging = false;
            return;
        }
        if (OnPointerClickCallBack != null) OnPointerClickCallBack(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (OnDragCallBack != null)
            bIsDraging = true;
        if (OnDragCallBack != null)
            OnDragCallBack(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (bIsDraging)
        {
            bIsDraging = false;
            return;
        }
        if (OnPointerDownCallBack != null) OnPointerDownCallBack(eventData);
        if (OnPointerLongPressCallBack != null)
        {
            isPointDown = true;
            m_Delay = delay;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (bIsDraging)
        {
            bIsDraging = false;
            return;
        }
        if (OnPointerUpCallBack != null) OnPointerUpCallBack(eventData);
        if (OnPointerLongPressCallBack != null)
        {
            isPointDown = false;
            m_Delay = delay;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (bIsDraging)
        {
            bIsDraging = false;
            return;
        }
        if (OnPointerExitCallBack != null)
            OnPointerExitCallBack(eventData);
        if (OnPointerLongPressCallBack != null)
        {
            isPointDown = false;
            m_Delay = delay;
        }

    }

    private void OnPointerLongPress()
    {
        if (OnPointerLongPressCallBack != null)
            OnPointerLongPressCallBack(null);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (OnDropCallBack != null) OnDropCallBack(eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        bIsDraging = true;
        if (OnBeginDragCallBack != null) OnBeginDragCallBack(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        bIsDraging = false;
        if (OnEndDragCallBack != null) OnEndDragCallBack(eventData);
    }
    /// <summary>
    /// 注册拖动开始与结束事件
    /// </summary>
    /// <param name="onBeginDragCallBack"></param>
    /// <param name="onEndDragCallBack"></param>
    public void RegisterDragEvent(OnOperation onBeginDragCallBack, OnOperation onEndDragCallBack)
    {
        if (onBeginDragCallBack != null)
        {
            OnBeginDragCallBack -= onBeginDragCallBack;
            OnBeginDragCallBack += onBeginDragCallBack;
        }
        if (onEndDragCallBack != null)
        {
            OnEndDragCallBack -= onEndDragCallBack;
            OnEndDragCallBack += onEndDragCallBack;
        }
    }
    /// <summary>
    /// 注册长按事件
    /// </summary>
    /// <param name="onPointerLongPressCallBack"></param>
    public void RegisterLongPressEvent(OnOperation onPointerLongPressCallBack, float interval = 0.05f)
    {
        if (onPointerLongPressCallBack != null)
        {
            OnPointerLongPressCallBack -= onPointerLongPressCallBack;
            OnPointerLongPressCallBack += onPointerLongPressCallBack;
            intervalTime = interval;
        }
    }
    /// <summary>
    /// 注册点击拖动事件
    /// </summary>
    /// <param name="onPointerClickCallBack"></param>
    /// <param name="onDragCallBack"></param>
    public void RegisterEvent(OnOperation onPointerClickCallBack, OnOperation onDragCallBack = null)
    {
        if (onPointerClickCallBack != null)
        {
            OnPointerClickCallBack -= onPointerClickCallBack;
            OnPointerClickCallBack += onPointerClickCallBack;
        }
        if (onDragCallBack != null)
        {
            OnDragCallBack -= onDragCallBack;
            OnDragCallBack += onDragCallBack;
        }
    }
    /// <summary>
    /// 注销点击事件
    /// </summary>
    /// <param name="onPointerClickCallBack"></param>
    /// <param name="onDragCallBack"></param>
    public void UnRegisterEvent(OnOperation onPointerClickCallBack, OnOperation onDragCallBack)
    {
        if (onPointerClickCallBack != null)
        {
            OnPointerClickCallBack -= onPointerClickCallBack;
        }
        if (onDragCallBack != null)
        {
            OnDragCallBack -= onDragCallBack;
        }
    }
    /// <summary>
    /// 注册点击事件
    /// </summary>
    /// <param name="onPointerDownCallBack"></param>
    /// <param name="onPointerUpCallBack"></param>
    /// <param name="onPointerExitCallBack"></param>
    public void RegisterClickEvent(OnOperation onPointerDownCallBack, OnOperation onPointerUpCallBack, OnOperation onPointerExitCallBack = null)
    {
        if (onPointerDownCallBack != null)
        {
            OnPointerDownCallBack -= onPointerDownCallBack;
            OnPointerDownCallBack += onPointerDownCallBack;
        }
        if (onPointerUpCallBack != null)
        {
            OnPointerUpCallBack -= onPointerUpCallBack;
            OnPointerUpCallBack += onPointerUpCallBack;
        }
        if (onPointerExitCallBack != null)
        {
            OnPointerExitCallBack -= onPointerExitCallBack;
            OnPointerExitCallBack += onPointerExitCallBack;
        }
    }
    /// <summary>
    /// 注销点击事件
    /// </summary>
    /// <param name="onPointerDownCallBack"></param>
    /// <param name="onPointerUpCallBack"></param>
    public void UnRegisterClickEvent(OnOperation onPointerDownCallBack, OnOperation onPointerUpCallBack, OnOperation onPointerExitCallBack)
    {
        if (onPointerDownCallBack != null)
            OnPointerClickCallBack -= onPointerDownCallBack;
        if (onPointerUpCallBack != null)
            OnDragCallBack -= onPointerUpCallBack;
        if (onPointerExitCallBack != null)
            OnPointerExitCallBack -= onPointerExitCallBack;
    }
    /// <summary>
    /// 注册下降事件
    /// </summary>
    /// <param name="onOnDropCallBack"></param>
    public void RegisterDropEvent(OnOperation onOnDropCallBack)
    {
        if (onOnDropCallBack != null)
        {
            OnDropCallBack -= onOnDropCallBack;
            OnDropCallBack += onOnDropCallBack;
        }
    }
    /// <summary>
    /// 注销拖动事件
    /// </summary>
    /// <param name="onBeginDragCallBack"></param>
    /// <param name="onEndDragCallBack"></param>
    public void UnRegisterDragEvent(OnOperation onBeginDragCallBack, OnOperation onEndDragCallBack)
    {
        if (onBeginDragCallBack != null)
        {
            OnBeginDragCallBack -= onBeginDragCallBack;
        }
        if (onEndDragCallBack != null)
        {
            OnEndDragCallBack -= onEndDragCallBack;
        }
    }
    /// <summary>
    /// 注销长按事件
    /// </summary>
    /// <param name="onPointerLongPressCallBack"></param>
    public void UnRegisterLongPressEvent(OnOperation onPointerLongPressCallBack)
    {
        if (onPointerLongPressCallBack != null)
            OnPointerLongPressCallBack -= onPointerLongPressCallBack;
    }

}