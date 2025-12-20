using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class DialogueUI : MonoBehaviour
{
    public static DialogueUI I;

    [Header("UI Refs")]
    public GameObject panel;
    public TMP_Text nameText;
    public TMP_Text bodyText;
    public TMP_Text hintText;

    [Header("Choices")]
    public GameObject choiceRoot;
    public Button choiceAButton;
    public TMP_Text choiceAText;
    public Button choiceBButton;
    public TMP_Text choiceBText;

    string[] lines;
    int index;
    bool isOpen;

    bool waitingChoice = false;
    Action onChoiceA;
    Action onChoiceB;


    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        
        if (panel) panel.SetActive(false);
        if (choiceRoot) choiceRoot.SetActive(false);
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
        waitingChoice = false;
        if (choiceRoot) choiceRoot.SetActive(false);

        lines = dialogueLines;
        index = 0;

        if (panel) panel.SetActive(true);
        if (nameText) nameText.text = speaker;
        ShowLine();
    }

    public void Next()
    {
        // 선택지 떠있는 동안에는 Next 막음
        if (!isOpen || waitingChoice) return;

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

    // 선택지 보이기
    public void ShowChoices(
        string promptText,
        string choiceA, Action onA,
        string choiceB, Action onB
    )
    {
        isOpen = true;
        waitingChoice = true;

        if (panel) panel.SetActive(true);
        if (bodyText) bodyText.text = promptText;
        if (hintText) hintText.text = " ";

        onChoiceA = onA;
        onChoiceB = onB;

        if (choiceAText) choiceAText.text = choiceA;
        if (choiceBText) choiceBText.text = choiceB;

        // 리스너 초기화 후 재등록
        choiceAButton.onClick.RemoveAllListeners();
        choiceBButton.onClick.RemoveAllListeners();

        choiceAButton.onClick.AddListener(() =>
        {
            waitingChoice = false;
            if (choiceRoot) choiceRoot.SetActive(false);
            onChoiceA?.Invoke();
        });

        choiceBButton.onClick.AddListener(() =>
        {
            waitingChoice = false;
            if (choiceRoot) choiceRoot.SetActive(false);
            onChoiceB?.Invoke();
        });

        if (choiceRoot) choiceRoot.SetActive(true);
    }



    public void Close()
    {
        // 빡세게 강제 초기화
        isOpen = false;
        waitingChoice = false;
        lines = null;       
        index = 0;

        if (choiceRoot) choiceRoot.SetActive(false);
        if (panel) panel.SetActive(false);

        if (nameText) nameText.text = "";
        if (bodyText) bodyText.text = "";
        if (hintText) hintText.text = "";

    }

    
    public void ShowChoicesOnly(
    string choiceA, System.Action onA,
    string choiceB, System.Action onB
    )
    {
        isOpen = true;
        waitingChoice = true;

        if (panel) panel.SetActive(true);

        if (hintText) hintText.text = " ";

        onChoiceA = onA;
        onChoiceB = onB;

        choiceAText.text = choiceA;
        choiceBText.text = choiceB;

        choiceAButton.onClick.RemoveAllListeners();
        choiceBButton.onClick.RemoveAllListeners();

        choiceAButton.onClick.AddListener(() =>
        {
            waitingChoice = false;
            if (choiceRoot) choiceRoot.SetActive(false);
            onChoiceA?.Invoke();
        });

        choiceBButton.onClick.AddListener(() =>
        {
            waitingChoice = false;
            if (choiceRoot) choiceRoot.SetActive(false);
            onChoiceB?.Invoke();
        });

        if (choiceRoot) choiceRoot.SetActive(true);
    }


    public void OpenOnePage(string speaker, string[] dialogueLines, string separator = "\n")
    {
        if (dialogueLines == null || dialogueLines.Length == 0) return;

        // 여러 줄을 한 페이지로 합쳐서 1줄짜리 배열로 Open
        string merged = string.Join(separator, dialogueLines);
        Open(speaker, new string[] { merged });
    }



}