using UnityEngine.UI;

public static class ShieldButtonTools
{
    /// <summary>
    /// 屏蔽Button的Interactable，时间单位为秒，默认最短时间为0.01秒
    /// </summary>
    /// <param name="btn"></param>
    /// <param name="second"></param>
    public static void ShieldInteractable(this Button btn, float second = 0.01f)
    {
        if (!btn)
        {
            return;
        }
        if (!btn.interactable)
        {
            return;
        }
        float period = second * 1000;
        ShieldButtonManager shieldButtonManager = new ShieldButtonManager(btn, period, ShieldBtnType.INTERACTABLE);
    }

    /// <summary>
    /// 屏蔽Button的Enable，时间单位为秒，默认最短时间为0.01秒
    /// </summary>
    /// <param name="btn"></param>
    /// <param name="second"></param>
    public static void ShieldEnable(this Button btn, float second = 0.01f)
    {
        if (!btn)
        {
            return;
        }
        if (!btn.enabled)
        {
            return;
        }
        float period = second * 1000;
        ShieldButtonManager shieldButtonManager = new ShieldButtonManager(btn, period, ShieldBtnType.ENABLE);
    }

    /// <summary>
    /// 屏蔽Button的上的Image的Raycast Target，时间单位为秒，默认最短时间为0.01秒
    /// </summary>
    /// <param name="btn"></param>
    /// <param name="second"></param>
    public static void ShieldRaycastTarget(this Button btn, float second = 0.01f)
    {
        if (!btn)
        {
            return;
        }
        if (!btn.image)
        {
            return;
        }
        if (!btn.image.raycastTarget)
        {
            return;
        }
        float period = second * 1000;
        ShieldButtonManager shieldButtonManager = new ShieldButtonManager(btn, period, ShieldBtnType.RAYCASTTARGET);
    }
}

public class ShieldButtonManager
{
    private float m_Period;
    private Timer timer;
    private Button m_Btn;

    public ShieldButtonManager(Button button, float period, ShieldBtnType type)
    {
        m_Btn = button;
        m_Period = period;
        timer = new Timer(m_Period, 1);

        switch (type)
        {
            case ShieldBtnType.INTERACTABLE:
                {
                    m_Btn.interactable = false;
                    timer.AddEventListener(ShieldInteractable);
                }
                break;
            case ShieldBtnType.ENABLE:
                {
                    m_Btn.enabled = false;
                    timer.AddEventListener(ShieldEnable);
                }
                break;
            case ShieldBtnType.RAYCASTTARGET:
                {
                    m_Btn.image.raycastTarget = false;
                    timer.AddEventListener(ShieldRaycastTarget);
                }
                break;
            default:
                break;
        }
        timer.Start();
    }

    private void ShieldInteractable()
    {
        if (m_Btn.interactable)
        {
            return;
        }
        timer.ReSet();
        m_Btn.interactable = true;
    }

    private void ShieldEnable()
    {
        if (m_Btn.enabled)
        {
            return;
        }
        timer.ReSet();
        m_Btn.enabled = true;
    }

    private void ShieldRaycastTarget()
    {
        if (m_Btn.image.raycastTarget)
        {
            return;
        }
        timer.ReSet();
        m_Btn.image.raycastTarget = true;
    }
}

public enum ShieldBtnType
{
    INTERACTABLE = 1,
    ENABLE = 2,
    RAYCASTTARGET = 3,
}