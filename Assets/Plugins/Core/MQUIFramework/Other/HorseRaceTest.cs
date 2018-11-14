using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseRaceTest : Singleton<HorseRaceTest>
{

    //public GameObject noticeTip_Panel;
    public void press01()
    {
        HorseRaceController.Instance.AddMessage("第一条消息", 5);
        //NoticeController.Instance.AddMessage("第二条消息");
        //NoticeController.Instance.AddMessage("第三条消息");
    }
    public void press02()
    {
        HorseRaceController.Instance.AddMessage("长消息。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。。", 3);
    }
}
