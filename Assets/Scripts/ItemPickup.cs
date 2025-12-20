using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    [Header("Link HomeConnectManager in this scene")]
    public HomeConnectManager connect;

    [Header("UI: return-home panel")]
    public ReturnHomePanel returnHomePanel;

    [Header("Optional: show message")]
    public bool showMessage = true;

    public string GetPrompt() => "E : 획득하기";

    void Start()
    {
        if (connect == null)
            connect = FindFirstObjectByType<HomeConnectManager>();

        // 이미 클리어한 월드면 아이템 숨김
        if (connect != null && PlayerPrefs.GetInt(connect.worldSaveKey, 0) == 1)
            gameObject.SetActive(false);
    }

    public void Interact()
    {
        if (connect == null)
        {
            Debug.LogError("[ItemPickup] HomeConnectManager not found/linked.");
            return;
        }

        // 중복 방지
        if (PlayerPrefs.GetInt(connect.worldSaveKey, 0) == 1)
        {
            if (DialogueUI.I != null)
                DialogueUI.I.Open("시스템", new string[] { "이미 획득했다." });
            return;
        }

        // 저장 (해금)
        PlayerPrefs.SetInt(connect.worldSaveKey, 1);
        PlayerPrefs.Save();

        // 아이템 획득 메세지 
        if (showMessage && DialogueUI.I != null)
        {
            DialogueUI.I.OpenOnePage("획득", new string[] { "아이템을 획득했다!", "다음 맵으로 가자." });
        }

        // 홈 복귀 버튼
        if (returnHomePanel != null)
            returnHomePanel.Show(connect);
        else
            Debug.LogWarning("[ItemPickup] returnHomePanel not linked.");

        // 오브젝트 제거
        gameObject.SetActive(false);

    }
}
