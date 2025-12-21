using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class AssembleZoneUI : MonoBehaviour, IDropHandler
{
    public event Action<DraggablePieceUI> OnPieceDropped;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        var piece = eventData.pointerDrag.GetComponent<DraggablePieceUI>();
        if (piece == null) return;

        // 🔥 필수: 이번 드롭은 Zone 성공
        piece.MarkDroppedOnZone();

        Debug.Log($"[AssembleZone] DROP pieceId={piece.pieceId}");
        OnPieceDropped?.Invoke(piece);
    }
}
