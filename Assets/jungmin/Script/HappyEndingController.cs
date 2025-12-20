using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class HappyEndingController : MonoBehaviour
{
    [Header("Drop/Snap")]
    [SerializeField] private AssembleZoneUI assembleZone;
    [SerializeField] private RectTransform snapPoint;
    [SerializeField] private Transform piecesRoot;

    [Header("Completed UI")]
    [SerializeField] private GameObject completedPanel; // 안에 CompletedClock이 있어야 함
    [SerializeField] private TMP_Text happyText;
    [SerializeField] private Button exitToTitleButton;

    [Header("Settings")]
    [SerializeField] private int totalPieces = 4;
    [SerializeField] private string titleSceneName = "00_Title";
    [SerializeField] private string happyLine = "시계가 만들어졌다!";

    private readonly HashSet<int> assembled = new HashSet<int>();

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
        Debug.Log($"[Happy] pieceId={piece.pieceId}, count(before)={assembled.Count}");

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
            Complete();
    }

    private void Complete()
    {
        Debug.Log("[Happy] COMPLETE CALLED");
        // 조각들 숨김
        for (int i = 0; i < piecesRoot.childCount; i++)
            piecesRoot.GetChild(i).gameObject.SetActive(false);

        // 완성 패널 켬 (CompletedClock이 보이게 됨)
        if (happyText != null) happyText.text = happyLine;
        if (completedPanel != null) completedPanel.SetActive(true);
    }
}
