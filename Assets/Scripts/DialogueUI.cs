using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI I;

    [Header("UI Refs")]
    public GameObject panel;
    public TMP_Text nameText;
    public TMP_Text bodyText;
    public TMP_Text hintText;

    string[] lines;
    int index;
    bool isOpen;

    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        
        if (panel) panel.SetActive(false);
    }

    void Start()
    {
        Close();
    }

    public bool IsOpen() => isOpen;

    public void Open(string speaker, string[] dialogueLines)
    {
        if (dialogueLines == null || dialogueLines.Length == 0) return;

        isOpen = true;
        lines = dialogueLines;
        index = 0;

        if (panel) panel.SetActive(true);
        if (nameText) nameText.text = speaker;
        ShowLine();
    }

    public void Next()
    {
        if (!isOpen) return;

        index++;
        if (index >= lines.Length)
        {
            Close();
            return;
        }
        ShowLine();
    }

    void ShowLine()
    {
        if (bodyText) bodyText.text = lines[index];
        if (hintText) hintText.text = (index == lines.Length - 1) ? "E: 닫기" : "E: 다음";
    }

    public void Close()
    {
        // 빡세게 강제 초기화
        isOpen = false;
        lines = null;       
        index = 0;

        if (nameText) nameText.text = "";
        if (bodyText) bodyText.text = "";
        if (hintText) hintText.text = "";

        if (panel) panel.SetActive(false);
    }

}
