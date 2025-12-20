using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class HappyEndingController : MonoBehaviour
{
    [Header("Go Happy Ending Scene Automatically")]
    [SerializeField] private string happyEndingSceneName = "99_Ending(happy)_finish";
    [SerializeField] private float goEndingDelay = 0.8f;

    [Header("Drop/Snap")]
    [SerializeField] private AssembleZoneUI assembleZone;
    [SerializeField] private RectTransform snapPoint;
    [SerializeField] private RectTransform piecesRoot;

    [Header("Completed UI")]
    [SerializeField] private GameObject completedPanel;       // 전체 패널 (처음 OFF)
    [SerializeField] private Image completedClockImage;       // 완성 시계 Image (패널 안)
    [SerializeField] private TMP_Text happyText;              // 멘트
    [SerializeField] private CanvasGroup completedCanvasGroup; // (선택) 페이드용

    [Header("Settings")]
    [SerializeField] private int totalPieces = 4;
    [SerializeField] private string happyLine = "시계가 만들어졌다!";

    [Header("Animation")]
    [SerializeField] private float settlePause = 0.05f;     // 마지막 조각 들어온 후 잠깐 멈춤
    [SerializeField] private float absorbDuration = 0.35f;   // 조각 빨려들기 시간
    [SerializeField] private float popDuration = 0.30f;      // 완성 시계 팝 시간
    [SerializeField] private float popStartScale = 0.70f;    // 팝 시작 스케일
    [SerializeField] private float popOvershootScale = 1.08f;// 살짝 오버슈트
    [SerializeField] private float uiFadeDuration = 0.0f;    // 패널 페이드(원하면 0.1~0.2)

    private readonly HashSet<int> assembled = new HashSet<int>();
    private bool completing = false;

    private void Awake()
    {
        if (completedPanel != null) completedPanel.SetActive(false);

        if (assembleZone != null)
            assembleZone.OnPieceDropped += OnDropPiece;
        else
            Debug.LogError("[HappyEndingController] assembleZone not assigned!");
    }

    private void OnDropPiece(DraggablePieceUI piece)
    {
        if (completing) return;
        if (piece == null) return;

        if (piecesRoot == null || snapPoint == null)
        {
            Debug.LogError("[HappyEndingController] piecesRoot or snapPoint is not assigned.");
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

        // 0) 잠깐 멈칫
        if (settlePause > 0f)
            yield return new WaitForSecondsRealtime(settlePause);

        // 1) 조각 빨려들기
        yield return StartCoroutine(AbsorbPiecesToCenter());

        // 2) 완성 패널 켜기
        if (completedPanel != null) completedPanel.SetActive(true);

        if (completedCanvasGroup != null)
        {
            completedCanvasGroup.alpha = 0f;
            yield return StartCoroutine(FadeCanvasGroup(completedCanvasGroup, 0f, 1f, uiFadeDuration));
        }

        // 3) 완성 시계 팝
        if (completedClockImage != null)
            yield return StartCoroutine(Pop(completedClockImage.rectTransform));

        // 4) 멘트 표시
        if (happyText != null) happyText.text = happyLine;

        // 5) 자동 엔딩 이동
        yield return new WaitForSecondsRealtime(goEndingDelay);
        SceneManager.LoadScene(happyEndingSceneName);
    }

    private IEnumerator AbsorbPiecesToCenter()
    {
        if (piecesRoot == null) yield break;

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

            float eased = 1f - Mathf.Pow(1f - p, 4f);

            for (int i = 0; i < n; i++)
            {
                var rt = rects[i];
                if (rt == null) continue;

                rt.anchoredPosition = Vector2.Lerp(starts[i], target, eased);

                float s = Mathf.Lerp(1f, 0.2f, eased);
                rt.localScale = new Vector3(s, s, 1f);

                // 시계방향 회전(+는 반시계, -가 시계)
                rt.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(0f, -720f, eased));
            }

            yield return null;
        }

        // 조각 숨김
        for (int i = 0; i < n; i++)
        {
            if (rects[i] != null)
                rects[i].gameObject.SetActive(false);
        }
    }

    private IEnumerator Pop(RectTransform target)
    {
        if (target == null) yield break;
        if (!target.gameObject.activeSelf) target.gameObject.SetActive(true);

        Vector3 start = Vector3.one * popStartScale;
        Vector3 over = Vector3.one * popOvershootScale;
        Vector3 end = Vector3.one;

        target.localScale = start;

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

        float rest = Mathf.Max(0.0001f, popDuration - half);
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

        if (duration <= 0f)
        {
            cg.alpha = to;
            yield break;
        }

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
