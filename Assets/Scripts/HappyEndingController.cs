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
    [SerializeField] private RectTransform piecesRoot; // Pieces 부모(같은 Canvas 안)

    [Header("Completed UI")]
    [SerializeField] private GameObject completedPanel;        // 처음 OFF
    [SerializeField] private Image completedClockImage;        // 완성 시계 이미지
    [SerializeField] private TMP_Text happyText;
    [SerializeField] private Button exitToTitleButton;
    [SerializeField] private CanvasGroup completedCanvasGroup; // 선택(페이드)

    [Header("Settings")]
    [SerializeField] private int totalPieces = 4;
    [SerializeField] private string titleSceneName = "00_Title";
    [SerializeField] private string happyLine = "시계가 만들어졌다!";

    [Header("After Complete (optional)")]
    [SerializeField] private bool goToHappyFinishScene = true;
    [SerializeField] private string happyFinishSceneName = "99_EndingLast";
    [SerializeField] private float goFinishDelay = 1.5f;

    [Header("Animation")]
    [SerializeField] private float settlePause = 0.12f;
    [SerializeField] private float absorbDuration = 0.45f;
    [SerializeField] private float popDuration = 0.30f;
    [SerializeField] private float popStartScale = 0.75f;
    [SerializeField] private float popOvershootScale = 1.10f;
    [SerializeField] private float uiFadeDuration = 0.0f;

    private readonly HashSet<int> assembled = new HashSet<int>();
    private bool completing = false;

    private void Awake()
    {
        if (completedPanel != null) completedPanel.SetActive(false);

        if (assembleZone != null)
            assembleZone.OnPieceDropped += OnDropPiece;

        if (exitToTitleButton != null)
        {
            exitToTitleButton.onClick.RemoveAllListeners();
            exitToTitleButton.onClick.AddListener(() => SceneManager.LoadScene(titleSceneName));
            exitToTitleButton.gameObject.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        if (assembleZone != null)
            assembleZone.OnPieceDropped -= OnDropPiece;
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

        // 이미 등록된 pieceId면 원위치(중복 방지)
        if (assembled.Contains(piece.pieceId))
        {
            piece.ReturnToStart(piecesRoot);
            return;
        }

        // ✅ 기존 기능 유지: 드롭 즉시 센터로 스냅
        piece.SnapTo(snapPoint, piecesRoot);
        assembled.Add(piece.pieceId);

        Debug.Log($"[Happy] pieceId={piece.pieceId} assembled={assembled.Count}/{totalPieces}");

        if (assembled.Count >= totalPieces)
            StartCoroutine(CompleteSequence());
    }

    private IEnumerator CompleteSequence()
    {
        completing = true;

        yield return new WaitForSecondsRealtime(settlePause);

        // 1) 조각들 빨려들기
        yield return StartCoroutine(AbsorbPiecesToCenter());

        // 2) 완성 UI
        if (completedPanel != null) completedPanel.SetActive(true);

        if (completedCanvasGroup != null)
        {
            completedCanvasGroup.alpha = 0f;
            if (uiFadeDuration > 0f)
                yield return StartCoroutine(FadeCanvasGroup(completedCanvasGroup, 0f, 1f, uiFadeDuration));
            else
                completedCanvasGroup.alpha = 1f;
        }

        if (completedClockImage != null)
            yield return StartCoroutine(Pop(completedClockImage.rectTransform));

        if (happyText != null) happyText.text = happyLine;

        // 3) 다음 씬 이동(원하면)
        if (goToHappyFinishScene)
        {
            yield return new WaitForSecondsRealtime(goFinishDelay);
            SceneManager.LoadScene(happyFinishSceneName);
        }

        completing = false;
    }

    private IEnumerator AbsorbPiecesToCenter()
    {
        if (piecesRoot == null) yield break;

        int n = piecesRoot.childCount;
        var rects = new RectTransform[n];
        var starts = new Vector2[n];

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
                if (rt == null || !rt.gameObject.activeSelf) continue;

                rt.anchoredPosition = Vector2.Lerp(starts[i], target, eased);

                float s = Mathf.Lerp(1f, 0.2f, eased);
                rt.localScale = new Vector3(s, s, 1f);

                rt.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(0f, 720f, eased));
            }

            yield return null;
        }

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

        float rest = Mathf.Max(0.01f, popDuration - half);
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

        if (duration <= 0f)
        {
            cg.alpha = to;
            yield break;
        }

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
