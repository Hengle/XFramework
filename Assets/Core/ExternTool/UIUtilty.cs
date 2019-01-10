// ==========================================
// 描述： 
// 作者： HAK
// 时间： 2018-11-14 14:40:58
// 版本： V 1.0
// ==========================================
using UnityEngine;
using UnityEngine.UI;

public static class UIUtilty {

    /// <summary>
    /// 滚动区自动化设置
    /// </summary>
	public static void AutoSetScrollRect(RectTransform scrollTran, RectTransform contentTran)
    {
        VerticalLayoutGroup contentLayout = contentTran.GetComponent<VerticalLayoutGroup>();

        int contentCount = contentTran.childCount;
        float contentY = contentCount == 0? 0: contentTran.GetChild(0).GetComponent<RectTransform>().sizeDelta.y;    // 获取子物体大小
        float y = contentLayout.padding.top + (contentCount - 1) * contentLayout.spacing + contentY * contentCount;  // 通过自身参数以及子物体大小计算自身大小
        contentTran.sizeDelta = contentTran.sizeDelta.WithY(y);

        if (scrollTran.GetComponent<RectTransform>().sizeDelta.y > y)
        {
            scrollTran.GetComponent<ScrollRect>().vertical = false;
        }
        else
        {
            scrollTran.GetComponent<ScrollRect>().vertical = true;
        }
        scrollTran.localPosition = scrollTran.localPosition.WithY(0);
    }
}
