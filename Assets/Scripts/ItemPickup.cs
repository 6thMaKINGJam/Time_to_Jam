using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    [Header("Link HomeConnectManager in this scene")]
    public HomeConnectManager connect;

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
            Debug.LogError("[ItemPickupAndProgress] HomeConnectManager not found/linked.");
            return;
        }

        // 중복 방지
        if (PlayerPrefs.GetInt(connect.worldSaveKey, 0) == 1)
        {
            if (DialogueUI.I != null)
                DialogueUI.I.Open("시스템", new string[] { "이미 획득했다." });
            return;
        }

        if (showMessage && DialogueUI.I != null)
        {
            // 한 페이지로 보여주고 E 한번에 닫히게 하고 싶으면 OpenOnePage 써도 됨
            DialogueUI.I.OpenOnePage("획득", new string[] { "아이템을 획득했다.", "진행상황 저장됨." });
        }

        // 오브젝트 제거
        gameObject.SetActive(false);

        // 저장 + 홈/엔딩 이동
        connect.ClearAndGoHome();
    }
}
