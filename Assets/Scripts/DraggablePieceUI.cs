using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggablePieceUI : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int pieceId;

    private RectTransform rect;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Image image;

    private Transform startParent;
    private Vector2 startPos;

    // AssembleZone이 "이번 드롭은 성공"이라고 찍어주는 플래그
    private bool droppedOnZoneThisDrag = false;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        image = GetComponent<Image>();
    }

    // AssembleZoneUI에서 호출
    public void MarkDroppedOnZone()
    {
        droppedOnZoneThisDrag = true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        droppedOnZoneThisDrag = false;

        startParent = transform.parent;
        startPos = rect.anchoredPosition;

        // 드롭 판정이 Zone까지 내려가게
        canvasGroup.blocksRaycasts = false;
        if (image != null) image.raycastTarget = true;

        // 드래그 중 최상단
        if (canvas != null)
        {
            transform.SetParent(canvas.transform, true);
            transform.SetAsLastSibling();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas == null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)canvas.transform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 local
        );

        rect.anchoredPosition = local;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        // ❌ Zone 위 드롭 실패 → 원래 자리로 복귀 (다시 드래그 가능)
        if (!droppedOnZoneThisDrag && startParent != null)
        {
            transform.SetParent(startParent, true);
            rect.anchoredPosition = startPos;
            transform.SetAsLastSibling();
        }
        // ✅ 성공이면 AssembleZone/Controller가 SnapTo 처리
    }

    // 중복 드롭 등에서 사용
    public void ReturnToStart(Transform piecesRoot)
    {
        transform.SetParent(piecesRoot, true);
        rect.anchoredPosition = startPos;
        transform.SetAsLastSibling();

        canvasGroup.blocksRaycasts = true;
        if (image != null) image.raycastTarget = true;
    }

    // 🔥 핵심: 스냅된 조각은 Raycast를 완전히 끔
    public void SnapTo(RectTransform snapPoint, Transform piecesRoot)
    {
        transform.SetParent(piecesRoot, true);
        rect.anchoredPosition = snapPoint.anchoredPosition;
        transform.SetAsLastSibling();

        canvasGroup.blocksRaycasts = false;
        if (image != null) image.raycastTarget = false;
    }
}
