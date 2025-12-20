using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HappyFinalController : MonoBehaviour
{
[Header("Portal Zoom In (use CLOSED clock)")]
[SerializeField] private RectTransform clockClosedRect; // ClockClosed의 RectTransform
[SerializeField] private CanvasGroup clockStageGroup;   // ClockStage의 CanvasGroup
[SerializeField] private float zoomDuration = 1.2f;
[SerializeField] private float zoomScale = 7.0f;        // 4~10 추천
[SerializeField] private float zoomRotate = -35f;

[SerializeField] private float fadeOutStart = 0.25f;    // 0~1




    
    [Header("Stages")]
    [SerializeField] private GameObject clockStage;
    [SerializeField] private GameObject letterStage;
    [SerializeField] private GameObject mentText;

    [Header("Clock Images")]
    [SerializeField] private GameObject clockClosed;
    [SerializeField] private GameObject clockOpened;

    [Header("Buttons")]
    [SerializeField] private Button enterButton;
    [SerializeField] private Button backButton;

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

    [Header("Animation")]
    [SerializeField] private float popDuration = 0.3f;
    [SerializeField] private float startScale = 0.8f;

    private bool opened = false;

    private void Awake()
    {
        if (clockStage != null) clockStage.SetActive(true);
        if (letterStage != null) letterStage.SetActive(false);

        if (clockClosed != null) clockClosed.SetActive(true);
        if (clockOpened != null) clockOpened.SetActive(false);
        if (mentText != null) mentText.SetActive(true);

        if (enterButton != null) enterButton.gameObject.SetActive(true);

        if (letterBody != null) letterBody.text = letterText;
        
        // 닫힌 시계를 클릭해서 열리게 하고 싶으면, clockClosed에 Button 컴포넌트가 있어야 함
        var closedBtn = clockClosed != null ? clockClosed.GetComponent<Button>() : null;
        if (closedBtn != null) closedBtn.onClick.AddListener(OpenClock);

        if (enterButton != null) enterButton.onClick.AddListener(Enter);
        if (backButton != null) backButton.onClick.AddListener(Back);
    }

    public void OpenClock()
    {
        if (opened) return;
        opened = true;

        if (clockClosed != null) clockClosed.SetActive(false);
        if (clockOpened != null) clockOpened.SetActive(true);

        if (enterButton != null) enterButton.gameObject.SetActive(true);

        if (clockOpened != null)
            StartCoroutine(Pop(clockOpened.transform));
    }

    private IEnumerator Pop(Transform target)
    {
        if (target == null) yield break;

        Vector3 start = Vector3.one * startScale;
        Vector3 end = Vector3.one;
        target.localScale = start;

        float t = 0f;
        while (t < popDuration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / popDuration);
            float eased = 1f - Mathf.Pow(1f - p, 3f);
            target.localScale = Vector3.Lerp(start, end, eased);
            yield return null;
        }

        target.localScale = end;
    }

    private void Enter()
    {
        StartCoroutine(ZoomIntoPortalWithClosedThenShowLetter());
    }


  private IEnumerator ZoomIntoPortalWithClosedThenShowLetter()
{
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
    Vector3 startScale = clockClosedRect.localScale;
    Quaternion startRot = clockClosedRect.localRotation;
    float startAlpha = clockStageGroup != null ? clockStageGroup.alpha : 1f;

    Vector3 endScale = startScale * zoomScale;
    Quaternion endRot = Quaternion.Euler(0f, 0f, -Mathf.Abs(zoomRotate));

    float t = 0f;
    while (t < zoomDuration)
    {
        t += Time.unscaledDeltaTime;
        float p = Mathf.Clamp01(t / zoomDuration);

        // 포탈 빨려들기 느낌: 초반 느리고 후반 급가속
        float eased = Mathf.Pow(p, 1.6f);

        clockClosedRect.localScale = Vector3.Lerp(startScale, endScale, eased);
        clockClosedRect.localRotation = Quaternion.Slerp(startRot, endRot, eased);

        // 후반부부터 페이드 아웃
        if (clockStageGroup != null)
        {
            float fadeP = Mathf.InverseLerp(fadeOutStart, 1f, eased);
            clockStageGroup.alpha = Mathf.Lerp(startAlpha, 0f, Mathf.Clamp01(fadeP));
        }

        yield return null;
    }

    // 화면 전환
    if (clockStage != null) clockStage.SetActive(false);
    if (letterStage != null) letterStage.SetActive(true);

    // 원상복구(다시 돌아올 수 있으니까)
    clockClosedRect.localScale = startScale;
    clockClosedRect.localRotation = startRot;

    if (clockStageGroup != null)
    {
        clockStageGroup.alpha = startAlpha;
        clockStageGroup.interactable = true;
        clockStageGroup.blocksRaycasts = true;
    }

    if (enterButton != null) enterButton.interactable = true;
}




    private void Back()
    {
        if (letterStage != null) letterStage.SetActive(false);
        if (clockStage != null) clockStage.SetActive(true);
    }
}
