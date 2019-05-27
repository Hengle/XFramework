using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

/// <summary>
/// 用于创建一些自定义UI组件
/// </summary>
public class CreateComponent
{
    private static DefaultControls.Resources s_StandardResources;

    private const string kUILayerName = "UI";

    private const string kStandardSpritePath = "UI/Skin/UISprite.psd";
    private const string kBackgroundSpritePath = "UI/Skin/Background.psd";
    private const string kInputFieldBackgroundPath = "UI/Skin/InputFieldBackground.psd";
    private const string kKnobPath = "UI/Skin/Knob.psd";
    private const string kCheckmarkPath = "UI/Skin/Checkmark.psd";
    private const string kDropdownArrowPath = "UI/Skin/DropdownArrow.psd";
    private const string kMaskPath = "UI/Skin/UIMask.psd";

    [MenuItem("GameObject/UI/Tree")]
    public static void CreateTree()
    {
        GameObject parent = Selection.activeGameObject;

        RectTransform tree = new GameObject("Tree").AddComponent<RectTransform>();
        tree.SetParent(parent.transform);
        tree.localPosition = Vector3.zero;
        tree.gameObject.AddComponent<XFramework.UI.Tree>();
        tree.sizeDelta = new Vector2(180, 30);

        // 设置模板
        RectTransform itemTemplate = new GameObject("NodeTemplate").AddComponent<RectTransform>();
        itemTemplate.SetParent(tree);
        itemTemplate.pivot = new Vector2(0, 1);
        itemTemplate.anchorMin = new Vector2(0, 1);
        itemTemplate.anchorMax = new Vector2(0, 1);
        itemTemplate.anchoredPosition = new Vector2(0, 0);
        itemTemplate.sizeDelta = new Vector2(180, 30);

        RectTransform button = DefaultControls.CreateButton(GetStandardResources()).GetComponent<RectTransform>();
        button.SetParent(itemTemplate);
        button.anchoredPosition = new Vector2(10, 0);
        button.sizeDelta = new Vector2(160, 30);

        RectTransform toggle = DefaultControls.CreateToggle(GetStandardResources()).GetComponent<RectTransform>();
        toggle.SetParent(itemTemplate);
        Object.DestroyImmediate(toggle.Find("Label").gameObject);
        toggle.anchoredPosition = new Vector2(-80, 0);
        toggle.sizeDelta = new Vector2(20, 20);

        RectTransform child = new GameObject("Child").AddComponent<RectTransform>();
        child.SetParent(itemTemplate);
        child.pivot = new Vector2(0, 1);
        child.anchorMin = new Vector2(0, 1);
        child.anchorMax = new Vector2(0, 1);
        child.sizeDelta = Vector2.zero;
        child.anchoredPosition = new Vector2(20, -30);


        // 设置树的跟结点位置
        RectTransform treeRoot = new GameObject("Root").AddComponent<RectTransform>();
        treeRoot.SetParent(tree);
        treeRoot.pivot = new Vector2(0, 1);
        treeRoot.anchorMin = new Vector2(0, 1);
        treeRoot.anchorMax = new Vector2(0, 1);
        treeRoot.anchoredPosition = new Vector2(0, 0);
        treeRoot.sizeDelta = new Vector2(0, 0);
    }

    private static DefaultControls.Resources GetStandardResources()
    {
        if (s_StandardResources.standard == null)
        {
            s_StandardResources.standard = AssetDatabase.GetBuiltinExtraResource<Sprite>(kStandardSpritePath);
            s_StandardResources.background = AssetDatabase.GetBuiltinExtraResource<Sprite>(kBackgroundSpritePath);
            s_StandardResources.inputField = AssetDatabase.GetBuiltinExtraResource<Sprite>(kInputFieldBackgroundPath);
            s_StandardResources.knob = AssetDatabase.GetBuiltinExtraResource<Sprite>(kKnobPath);
            s_StandardResources.checkmark = AssetDatabase.GetBuiltinExtraResource<Sprite>(kCheckmarkPath);
            s_StandardResources.dropdown = AssetDatabase.GetBuiltinExtraResource<Sprite>(kDropdownArrowPath);
            s_StandardResources.mask = AssetDatabase.GetBuiltinExtraResource<Sprite>(kMaskPath);
        }
        return s_StandardResources;
    }
}