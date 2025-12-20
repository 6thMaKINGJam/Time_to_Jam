using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class HappyEndingController : MonoBehaviour
{
    [Header("Drop/Snap")]
    [SerializeField] private AssembleZoneUI assembleZone;
    [SerializeField] private RectTransform snapPoint;
    [SerializeField] private RectTransform piecesRoot; // RectTransform으로 받는 걸 추천

    [Header("Completed UI")]
    [SerializeField] private GameObject completedPanel;      // 전체 패널 (처음 OFF)
    [SerializeField] private Image completedClockImage;      // 완성 시계 Image (패널 안)
    [SerializeField] private TMP_Text happyText;             // 멘트
    [SerializeField] private Button exitToTitleButton;       // 타이틀로
    [SerializeField] private CanvasGroup completedCanvasGroup; // (선택) 페이드용

    [Header("Settings")]
    [SerializeField] private int totalPieces = 4;
    [SerializeField] private string titleSceneName = "00_Title";
    [SerializeField] private string happyLine = "시계가 만들어졌다!";

    [Header("Animation")]
    [SerializeField] private float settlePause = 0.15f;      // 마지막 조각 들어온 후 잠깐 멈춤
    [SerializeField] private float absorbDuration = 0.30f;    // 조각 빨려들기 시간
    [SerializeField] private float popDuration = 0.30f;       // 완성 시계 팝 시간
    [SerializeField] private float popStartScale = 0.70f;     // 팝 시작 스케일
    [SerializeField] private float popOvershootScale = 1.08f; // 살짝 오버슈트
    [SerializeField] private float uiFadeDuration = 0.0f;    // 패널 페이드(선택)

    private readonly HashSet<int> assembled = new HashSet<int>();
    private bool completing = false;

    private void Awake()
    {
        if (completedPanel != null) completedPanel.SetActive(false);

        if (assembleZone != null)
            assembleZone.OnPieceDropped += OnDropPiece;

        if (exitToTitleButton != null)
            exitToTitleButton.onClick.AddListener(() => SceneManager.LoadScene(titleSceneName));
    }

    private void OnDropPiece(DraggablePieceUI piece)
    {
        if (completing) return;
        if (piece == null) return;

        if (piecesRoot == null || snapPoint == null)
        {
            Debug.LogError("[HappyEnding] piecesRoot or snapPoint is not assigned.");
            return;
        }

        if (assembled.Contains(piece.pieceId))
        {
            piece.ReturnToStart(piecesRoot);
            return;
        }

        piece.SnapTo(snapPoint, piecesRoot);
        assembled.Add(piece.pieceId);

        if (assembled.Count >= totalPieces)
            StartCoroutine(CompleteSequence());
    }

    private IEnumerator CompleteSequence()
    {
        completing = true;

        // 0) 마지막 조각 스냅된 직후 잠깐 멈칫
        yield return new WaitForSecondsRealtime(settlePause);

        // 1) 조각들 빨려들어가며 사라짐(중앙으로 모이면서 축소)
        yield return StartCoroutine(AbsorbPiecesToCenter());

        // 2) 완성 패널 켜기(시계는 먼저, 글자는 나중)
        if (completedPanel != null) completedPanel.SetActive(true);

        if (completedCanvasGroup != null)
        {
            completedCanvasGroup.alpha = 0f;
            yield return StartCoroutine(FadeCanvasGroup(completedCanvasGroup, 0f, 1f, uiFadeDuration));
        }

        // 3) 완성 시계 팝(0.7 -> 1.08 -> 1.0)
        if (completedClockImage != null)
            yield return StartCoroutine(Pop(completedClockImage.rectTransform));

        // 4) 멘트/버튼 표시
        if (happyText != null) happyText.text = happyLine;
        if (exitToTitleButton != null) exitToTitleButton.gameObject.SetActive(true);

        completing = false;
    }

    private IEnumerator AbsorbPiecesToCenter()
    {
        if (piecesRoot == null) yield break;

        // 모든 조각의 시작 상태 저장
        int n = piecesRoot.childCount;
        var starts = new Vector2[n];
        var rects = new RectTransform[n];

        for (int i = 0; i < n; i++)
        {
            rects[i] = piecesRoot.GetChild(i) as RectTransform;
            if (rects[i] == null) continue;
            starts[i] = rects[i].anchoredPosition;
        }

        Vector2 target = snapPoint != null ? snapPoint.anchoredPosition : Vector2.zero;

        float t = 0f;
        while (t < absorbDuration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / absorbDuration);

            // 빨려드는 느낌: 점점 빨라지는 곡선
            float eased = 1f - Mathf.Pow(1f - p, 4f);

            for (int i = 0; i < n; i++)
            {
                var rt = rects[i];
                if (rt == null) continue;

                rt.anchoredPosition = Vector2.Lerp(starts[i], target, eased);

                // 축소(1 -> 0.2)
                float s = Mathf.Lerp(1f, 0.2f, eased);
                rt.localScale = new Vector3(s, s, 1f);

                // 회전(선택: 살짝 돌면서 들어가게)
                rt.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(0f, 720f, eased));
            }

            yield return null;
        }

        // 마무리: 전부 비활성
        for (int i = 0; i < n; i++)
        {
            if (rects[i] != null)
                rects[i].gameObject.SetActive(false);
        }
    }

    private IEnumerator Pop(RectTransform target)
    {
        if (target == null) yield break;

        // 혹시 이미지가 꺼져있으면 켜져 있어야 보임
        if (!target.gameObject.activeSelf) target.gameObject.SetActive(true);

        // 2단 팝: start -> overshoot -> 1
        Vector3 start = Vector3.one * popStartScale;
        Vector3 over = Vector3.one * popOvershootScale;
        Vector3 end = Vector3.one;

        target.localScale = start;

        // 절반: start -> over
        float half = popDuration * 0.55f;
        float t = 0f;
        while (t < half)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / half);
            float eased = 1f - Mathf.Pow(1f - p, 3f);
            target.localScale = Vector3.Lerp(start, over, eased);
            yield return null;
        }

        // 나머지: over -> end
        float rest = popDuration - half;
        t = 0f;
        while (t < rest)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / rest);
            float eased = 1f - Mathf.Pow(1f - p, 3f);
            target.localScale = Vector3.Lerp(over, end, eased);
            yield return null;
        }

        target.localScale = end;
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration)
    {
        if (cg == null) yield break;

        cg.alpha = from;

        float t = 0f;
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
