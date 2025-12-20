using UnityEngine;
using TMPro;

public class InteractHintUI : MonoBehaviour
{
    public static InteractHintUI I;
    public TextMeshProUGUI text;

    void Awake()
    {
        I = this;
        Hide();
    }

    // 상호작용이 보이게 SetActive
    public void Show(string msg)
    {
        text.text = msg;
        text.gameObject.SetActive(true);
    }

    public void Hide()
    {
        text.gameObject.SetActive(false);
    }
}
