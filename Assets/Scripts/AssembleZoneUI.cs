using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class AssembleZoneUI : MonoBehaviour, IDropHandler
{
    public Action<DraggablePieceUI> OnPieceDropped;

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("[AssembleZone] DROP");
        var piece = eventData.pointerDrag ? eventData.pointerDrag.GetComponent<DraggablePieceUI>() : null;
        if (piece == null) return;

        OnPieceDropped?.Invoke(piece);
    }
}
