using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct HorseRace
{
    public string msg;
    public int loopTime;

    public HorseRace(string msg, int loopTime)
    {
        this.msg = msg;
        this.loopTime = loopTime;
    }
}
public class HorseRaceController : Singleton<HorseRaceController>
{
    [SerializeField]
    Text m_TxtMsg;

    Queue<HorseRace> m_MsgQueue;

    bool isScrolling = false;
    float panelRight;
    private float speed;
    private float panelWidth;
    RectTransform panelTransform;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        panelTransform = GetComponent<RectTransform>();
        m_MsgQueue = new Queue<HorseRace>();
        panelRight = (float)0.5 * panelTransform.rect.size.x + panelTransform.localPosition.x;
        speed = 300f;
        panelWidth = panelTransform.rect.size.x;
    }

    /// <summary>
    /// 添加消息
    /// </summary>
    public void AddMessage(string msg, int loopTime)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            //Init();
            m_MsgQueue = new Queue<HorseRace>();
        }
        m_MsgQueue.Enqueue(new HorseRace(msg, loopTime));
        if (isScrolling) return;
        StartCoroutine(Scrolling());
    }


    public IEnumerator Scrolling()
    {
        float beginX;
        while (m_MsgQueue.Count > 0)
        {
            float duration = 10f;
            HorseRace horseRace = m_MsgQueue.Dequeue();
            string msg = horseRace.msg;
            int loopTime = horseRace.loopTime;
            m_TxtMsg.text = msg;

            float txtWidth = m_TxtMsg.preferredWidth;
            float txtRectWidth = (float)0.5 * m_TxtMsg.rectTransform.rect.size.x;
            Vector3 pos = m_TxtMsg.rectTransform.localPosition;

            beginX = panelRight + txtRectWidth;
            float distance = txtWidth + panelWidth;
            float endRect = (float) 0.5 * (txtRectWidth - panelWidth) - txtWidth;
            duration = distance / speed;
            isScrolling = true;
            while (loopTime-- > 0)
            {
                Debug.Log(loopTime);
                m_TxtMsg.rectTransform.localPosition = new Vector3(beginX, pos.y, pos.z);
                m_TxtMsg.rectTransform.DOLocalMoveX(endRect, duration).SetEase(Ease.Linear);
                yield return new WaitForSeconds(duration);
            }
            yield return null;
        }
        isScrolling = false;
        gameObject.SetActive(false);
    }
}

