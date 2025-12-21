using UnityEngine;
using UnityEngine.EventSystems;

public class DraggablePieceUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int pieceId;

    private RectTransform rect;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    private Vector2 startPos;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPos = rect.anchoredPosition;

        // 드롭 영역이 드롭을 받을 수 있게
        canvasGroup.blocksRaycasts = false;

        // 드래그 중 위로 올리기 (부모는 canvas로 잠깐)
        transform.SetParent(canvas.transform, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
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
   
    }

    public void ReturnToStart(Transform piecesRoot)
    {
        transform.SetParent(piecesRoot, true);
        rect.anchoredPosition = startPos;
    }

    // ✅ 부모를 바꾸지 않고, "중앙 좌표로만 이동"
    public void SnapTo(RectTransform snapPoint, Transform piecesRoot)
{
    // 위치만 이동 (부모 유지)
    transform.SetParent(piecesRoot, true);
    rect.anchoredPosition = snapPoint.anchoredPosition;

    // 🔥 이게 핵심: 중앙에 붙은 조각은 Raycast 차단
    canvasGroup.blocksRaycasts = false;
    canvasGroup.interactable = false;
}

}
