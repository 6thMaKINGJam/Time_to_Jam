using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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

    [Header("Home Button (Locked Mode)")]
    public GameObject homeButtonRoot;
    public Button homeButton;
    public TMP_Text homeButtonText;

    string[] lines;
    int index;

    bool isOpen;
    bool waitingChoice;
    bool lockedToButton;                 // ✅ 락모드: E로 못 닫고 버튼만
    string lockedHomeSceneName = "02_Home";

    Action onChoiceA;
    Action onChoiceB;

    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;

        // 안전 초기화
        if (panel) panel.SetActive(false);
        if (choiceRoot) choiceRoot.SetActive(false);
        if (homeButtonRoot) homeButtonRoot.SetActive(false);
    }

    void Start()
    {
        Close();
    }

    public bool IsOpen() => isOpen;

    // =========================
    // 기본 대화 (E로 Next/Close 가능)
    // =========================
    public void Open(string speaker, string[] dialogueLines)
    {
        if (dialogueLines == null || dialogueLines.Length == 0) return;

        isOpen = true;
        waitingChoice = false;
        lockedToButton = false;

        lines = dialogueLines;
        index = 0;

        if (panel) panel.SetActive(true);
        if (nameText) nameText.text = speaker;

        HideChoices();
        HideHomeButton();

        ShowLine();
    }

    // 구버전 호환: 한 문장만
    public void OpenOnePage(string speaker, string message)
    {
        Open(speaker, new string[] { message });
    }

    // 구버전 호환/유틸: 여러 줄을 한 페이지로 합치기
    public void OpenOnePage(string speaker, string[] dialogueLines, string separator = "\n")
    {
        if (dialogueLines == null || dialogueLines.Length == 0) return;
        string merged = string.Join(separator, dialogueLines);
        Open(speaker, new string[] { merged });
    }

    // =========================
    // ✅ 아이템 획득용 락 모드
    // - E로 Next/Close 절대 안 됨
    // - "홈으로 돌아가기" 버튼만
    // =========================
    public void OpenLockedWithHomeButton(string speaker, string message, string homeSceneName = "02_Home")
    {
        isOpen = true;
        waitingChoice = false;
        lockedToButton = true;

        lines = null;
        index = 0;

        if (panel) panel.SetActive(true);
        HideChoices();

        if (nameText) nameText.text = speaker;
        if (bodyText) bodyText.text = message;
        if (hintText) hintText.text = ""; // ✅ E 안내 제거

        lockedHomeSceneName = string.IsNullOrEmpty(homeSceneName) ? "02_Home" : homeSceneName;

        ShowHomeButton();
    }

    // =========================
    // Next (E키)
    // =========================
    public void Next()
    {
        // ✅ 락모드면 E키 무시
        if (!isOpen || waitingChoice || lockedToButton) return;

        index++;

        if (lines == null || index >= lines.Length)
        {
            Close();
            return;
        }

        ShowLine();
    }

    void ShowLine()
    {
        if (lines == null || lines.Length == 0) return;

        if (bodyText) bodyText.text = lines[index];

        if (hintText)
            hintText.text = (index == lines.Length - 1) ? "E: 닫기" : "E: 다음";
    }

    // =========================
    // 선택지 (promptText를 body에 표시)
    // =========================
    public void ShowChoices(
        string promptText,
        string choiceA, Action onA,
        string choiceB, Action onB
    )
    {
        isOpen = true;
        waitingChoice = true;
        lockedToButton = false;

        if (panel) panel.SetActive(true);
        HideHomeButton();

        if (bodyText) bodyText.text = promptText;
        if (hintText) hintText.text = ""; // 원하면 "선택하시오."로

        onChoiceA = onA;
        onChoiceB = onB;

        SetChoiceButtons(choiceA, choiceB);
        if (choiceRoot) choiceRoot.SetActive(true);
    }

    // =========================
    // 구버전 호환: body 유지 + 선택지만 띄움
    // =========================
    public void ShowChoicesOnly(
        string choiceA, Action onA,
        string choiceB, Action onB
    )
    {
        isOpen = true;
        waitingChoice = true;
        lockedToButton = false;

        if (panel) panel.SetActive(true);
        HideHomeButton();

        if (hintText) hintText.text = "  "; 

        onChoiceA = onA;
        onChoiceB = onB;

        SetChoiceButtons(choiceA, choiceB);
        if (choiceRoot) choiceRoot.SetActive(true);
    }

    void SetChoiceButtons(string choiceA, string choiceB)
    {
        if (choiceAText) choiceAText.text = choiceA;
        if (choiceBText) choiceBText.text = choiceB;

        if (choiceAButton != null)
        {
            choiceAButton.onClick.RemoveAllListeners();
            choiceAButton.onClick.AddListener(() =>
            {
                waitingChoice = false;
                if (choiceRoot) choiceRoot.SetActive(false);
                onChoiceA?.Invoke();
            });
        }

        if (choiceBButton != null)
        {
            choiceBButton.onClick.RemoveAllListeners();
            choiceBButton.onClick.AddListener(() =>
            {
                waitingChoice = false;
                if (choiceRoot) choiceRoot.SetActive(false);
                onChoiceB?.Invoke();
            });
        }
    }

    void HideChoices()
    {
        waitingChoice = false;
        onChoiceA = null;
        onChoiceB = null;
        if (choiceRoot) choiceRoot.SetActive(false);
    }

    // =========================
    // 홈 버튼 표시/숨김
    // =========================
    void ShowHomeButton()
    {
        if (homeButtonRoot) homeButtonRoot.SetActive(true);

        if (homeButtonText) homeButtonText.text = "홈으로 돌아가기";

        if (homeButton != null)
        {
            homeButton.onClick.RemoveAllListeners();
            homeButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(lockedHomeSceneName);
            });
        }
    }

    void HideHomeButton()
    {
        if (homeButtonRoot) homeButtonRoot.SetActive(false);
    }

    // =========================
    // 닫기
    // =========================
    public void Close()
    {
        isOpen = false;
        waitingChoice = false;
        lockedToButton = false;

        lines = null;
        index = 0;

        HideChoices();
        HideHomeButton();

        if (panel) panel.SetActive(false);

        if (nameText) nameText.text = "";
        if (bodyText) bodyText.text = "";
        if (hintText) hintText.text = "";
    }
}
