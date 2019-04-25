using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Toggle))]
public class GUToggle : MonoBehaviour
{
    public Toggle toggle;

    public void AddListener(UnityAction<bool> action)
    {
        toggle.onValueChanged.AddListener(action);
    }

    private void Reset()
    {
        this.toggle = GetComponent<Toggle>();
    }
}
