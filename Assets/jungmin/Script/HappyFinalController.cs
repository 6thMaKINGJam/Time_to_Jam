using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class HappyFinalController : MonoBehaviour
{

    
    [Header("Desk Clock Swap (Object Blink)")]
    [SerializeField] private GameObject clockIcon1;   // ClockIcon (ON)
    [SerializeField] private GameObject clockIcon2;   // ClockIcon_2 (OFF)
    [SerializeField] private float clockSwapDelay = 0.5f;
    [SerializeField] private float blinkTime = 0.1f;
    private bool clockSwapped = false; // 1번만 실행

    [Header("Fade Transition")]
    [SerializeField] private CanvasGroup fadeOverlay;
    [SerializeField] private float fadeOutTime = 1.0f;
    [SerializeField] private float fadeInTime = 0.5f;

    [Header("Stages")]
    [SerializeField] private GameObject clockStage;      // 시계 화면
    [SerializeField] private GameObject letterStage;     // 메일(편지) 화면
    [SerializeField] private GameObject finalDeskStage;  // 책상 화면(처음 OFF)
    [SerializeField] private GameObject mentText;        // 시계 화면 멘트(선택)

    [Header("Clock Images")]
    [SerializeField] private GameObject clockClosed;     // 닫힌 시계
    [SerializeField] private GameObject clockOpened;     // 열린 시계(있으면)

    [Header("Buttons")]
    [SerializeField] private Button enterButton;             // 시계 들어가기
    [SerializeField] private Button closeMailButton;         // 메일 닫기 -> 책상
    [SerializeField] private Button deskBackToTitleButton;   // 책상 돌아가기 -> 타이틀

    [Header("Portal Zoom In (use CLOSED clock)")]
    [SerializeField] private RectTransform clockClosedRect; // ClockClosed의 RectTransform
    [SerializeField] private CanvasGroup clockStageGroup;   // ClockStage에 CanvasGroup 추가 후 연결
    [SerializeField] private float zoomDuration = 0.9f;
    [SerializeField] private float zoomScale = 7.0f;
    [SerializeField] private float zoomRotate = -720f;      // 시계방향(음수)
    [SerializeField] private float fadeOutStart = 0.25f;

    [Header("Letter")]
    [SerializeField] private TMP_Text letterBody;

    [TextArea]
    [SerializeField] private string letterText =
        "용사님,\n\n" +
        "믿음직스럽진 않았는데 해냈네요?\n\n" +
        "보물을 찾아주셔서 감사해요\n\n" +
        "보물의 정체는 굳이 말하지 않아도 알겠죠?\n\n" +
        "덕분에 모든 것이 제시간에 돌아오고\n" +
        "세계는 제 할 일을 하고 있어요\n\n" +
        "이 세계의 시간을 되돌려주어서 감사합니다";

    [Header("Scene")]
    [SerializeField] private string titleSceneName = "00_Title";

    private bool opened = false;

    private void Awake()
    {
        // 초기 상태
        if (clockStage != null) clockStage.SetActive(true);
        if (letterStage != null) letterStage.SetActive(false);
        if (finalDeskStage != null) finalDeskStage.SetActive(false);

        if (clockClosed != null) clockClosed.SetActive(true);
        if (clockOpened != null) clockOpened.SetActive(false);
        if (mentText != null) mentText.SetActive(true);

        if (letterBody != null) letterBody.text = letterText;

        // 책상 시계 아이콘 초기 상태(권장)
        if (clockIcon1 != null) clockIcon1.SetActive(true);
        if (clockIcon2 != null) clockIcon2.SetActive(false);
        clockSwapped = false;

        // 버튼 리스너
        if (enterButton != null) enterButton.onClick.AddListener(Enter);
        if (closeMailButton != null) closeMailButton.onClick.AddListener(CloseMailAndShowDesk);
        if (deskBackToTitleButton != null) deskBackToTitleButton.onClick.AddListener(GoToTitle);

        // (선택) 닫힌 시계 클릭으로 열린 시계 보여주고 싶으면 Button 있어야 함
        var closedBtn = clockClosed != null ? clockClosed.GetComponent<Button>() : null;
        if (closedBtn != null) closedBtn.onClick.AddListener(OpenClock);
    }

    public void OpenClock()
    {
        if (opened) return;
        opened = true;

        if (clockClosed != null) clockClosed.SetActive(false);
        if (clockOpened != null) clockOpened.SetActive(true);
    }

    private void Enter()
    {
        StartCoroutine(ZoomIntoPortalThenShowLetter());
    }

    private IEnumerator ZoomIntoPortalThenShowLetter()
    {
        // 연결 안 돼 있으면 그냥 전환
        if (clockClosedRect == null)
        {
            if (clockStage != null) clockStage.SetActive(false);
            if (letterStage != null) letterStage.SetActive(true);
            yield break;
        }

        // 입력 잠금
        if (enterButton != null) enterButton.interactable = false;
        if (clockStageGroup != null)
        {
            clockStageGroup.interactable = false;
            clockStageGroup.blocksRaycasts = false;
        }

        // 시작 상태 저장
        Vector3 startS = clockClosedRect.localScale;
        Quaternion startR = clockClosedRect.localRotation;
        float startA = (clockStageGroup != null) ? clockStageGroup.alpha : 1f;

        Vector3 endS = startS * zoomScale;
        Quaternion endR = Quaternion.Euler(0f, 0f, zoomRotate);

        float t = 0f;
        while (t < zoomDuration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / zoomDuration);
            float eased = Mathf.Pow(p, 1.6f);

            clockClosedRect.localScale = Vector3.Lerp(startS, endS, eased);
            clockClosedRect.localRotation = Quaternion.Slerp(startR, endR, eased);

            if (clockStageGroup != null)
            {
                float fadeP = Mathf.InverseLerp(fadeOutStart, 1f, eased);
                clockStageGroup.alpha = Mathf.Lerp(startA, 0f, Mathf.Clamp01(fadeP));
            }

            yield return null;
        }

        // 전환: 시계 화면 OFF → 메일 화면 ON
        if (clockStage != null) clockStage.SetActive(false);
        if (letterStage != null) letterStage.SetActive(true);

        // 원상복구
        clockClosedRect.localScale = startS;
        clockClosedRect.localRotation = startR;

        if (clockStageGroup != null)
        {
            clockStageGroup.alpha = startA;
            clockStageGroup.interactable = true;
            clockStageGroup.blocksRaycasts = true;
        }

        if (enterButton != null) enterButton.interactable = true;
    }

    // 메일 닫기 -> 책상 화면 (페이드 포함)
    public void CloseMailAndShowDesk()
    {
        StartCoroutine(CloseMailAndShowDesk_Fade());
    }

    private IEnumerator CloseMailAndShowDesk_Fade()
    {
        // 페이드 오버레이 없으면 그냥 즉시 전환
        if (fadeOverlay == null)
        {
            if (letterStage != null) letterStage.SetActive(false);
            if (mentText != null) mentText.SetActive(false);

            if (finalDeskStage != null)
            {
                finalDeskStage.SetActive(true);
                finalDeskStage.transform.SetAsLastSibling();
            }

            StartCoroutine(SwapDeskClockOnce());
            yield break;
        }

        // 1) 페이드 아웃
        yield return StartCoroutine(FadeOverlay(0f, 1f, fadeOutTime));

        // 2) 화면 전환(검정 상태에서)
        if (letterStage != null) letterStage.SetActive(false);
        if (mentText != null) mentText.SetActive(false);

        if (finalDeskStage != null)
        {
            finalDeskStage.SetActive(true);
            finalDeskStage.transform.SetAsLastSibling();
        }

        // 3) 책상 시계 교체(1회)
        StartCoroutine(SwapDeskClockOnce());

        // 4) 페이드 인
        yield return StartCoroutine(FadeOverlay(1f, 0f, fadeInTime));
    }

    private IEnumerator FadeOverlay(float from, float to, float duration)
    {
        if (fadeOverlay == null) yield break;

        if (duration <= 0f)
        {
            fadeOverlay.alpha = to;
            yield break;
        }

        float t = 0f;
        fadeOverlay.alpha = from;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / duration);
            fadeOverlay.alpha = Mathf.Lerp(from, to, p);
            yield return null;
        }

        fadeOverlay.alpha = to;
    }

    // 책상 화면 돌아가기 -> 타이틀
    public void GoToTitle()
    {
        Debug.Log("[HappyFinal] GoToTitle CALLED. Trying to load: " + titleSceneName);
        SceneManager.LoadScene(titleSceneName);
    }

    // 책상 시계: 일정 시간 후 깜빡이며 1회 교체
    private IEnumerator SwapDeskClockOnce()
    {
        if (clockSwapped) yield break;
        clockSwapped = true;

        yield return new WaitForSecondsRealtime(clockSwapDelay);

        if (clockIcon1 == null || clockIcon2 == null) yield break;

        CanvasGroup cg1 = clockIcon1.GetComponent<CanvasGroup>();
        CanvasGroup cg2 = clockIcon2.GetComponent<CanvasGroup>();

        // CanvasGroup 없으면 그냥 교체
        if (cg1 == null || cg2 == null)
        {
            clockIcon1.SetActive(false);
            clockIcon2.SetActive(true);
            yield break;
        }

        // 1) ClockIcon 페이드 아웃
        yield return StartCoroutine(FadeCanvasGroup(cg1, 1f, 0f, blinkTime));

        clockIcon1.SetActive(false);
        clockIcon2.SetActive(true);

        // ClockIcon_2는 투명에서 시작
        cg2.alpha = 0f;

        // 2) ClockIcon_2 페이드 인
        yield return StartCoroutine(FadeCanvasGroup(cg2, 0f, 1f, blinkTime));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration)
    {
        if (cg == null) yield break;

        if (duration <= 0f)
        {
            cg.alpha = to;
            yield break;
        }

        float t = 0f;
        cg.alpha = from;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / duration);
            cg.alpha = Mathf.Lerp(from, to, p);
            yield return null;
        }

        cg.alpha = to;
    }
}
