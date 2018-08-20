using UnityEngine;

public class RescrouceCode : MonoBehaviour
{
    private int m_Rrs_UGUID = 0;
    private bool m_Rrs_UseState = false;

    public void Init(int ugid)
    {
        m_Rrs_UGUID = ugid;
        m_Rrs_UseState = true;
    }

    public int UGID
    {
        get { return m_Rrs_UGUID; }
    }

    public bool UseState
    {
        get { return m_Rrs_UseState; }
        set { m_Rrs_UseState = value; }
    }
}