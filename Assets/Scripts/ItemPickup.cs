using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    [Header("Progress Key Source")]
    public HomeConnectManager connect;  // worldSaveKey 제공

    [Header("Spawn FX (optional)")]
    public GameObject itemPrefab;       // 옆에 나타나는 연출용 아이템 프리팹
    public Vector3 spawnOffset = new Vector3(0.8f, 0f, 0f);

    [Header("UI / Scene")]
    public string homeSceneName = "02_Home"; // 홈 씬 이름 (HomeConnectManager에 값 있으면 그거 쓰게도 가능)

    [Header("Prompt")]
    public string prompt = "E : 조사하기";

    bool used;

    public string GetPrompt() => prompt;

    void Start()
    {
        if (connect == null)
            connect = FindFirstObjectByType<HomeConnectManager>();

        if (connect == null)
        {
            Debug.LogError("[ItemPickup] HomeConnectManager not found in scene.");
            return;
        }

        // 이미 클리어된 월드면 이 오브젝트 자체를 숨김
        if (PlayerPrefs.GetInt(connect.worldSaveKey, 0) == 1)
        {
            used = true;
            gameObject.SetActive(false);
        }

        // connect에 homeSceneName이 있다면 그걸 우선 사용(있을 때만)
        // (HomeConnectManager에 homeSceneName 필드가 없으면 아래 줄 지워도 됨)
        try
        {
            var type = connect.GetType();
            var field = type.GetField("homeSceneName");
            if (field != null)
            {
                var value = field.GetValue(connect) as string;
                if (!string.IsNullOrEmpty(value))
                    homeSceneName = value;
            }
        }
        catch { /* ignore */ }
    }

    public void Interact()
    {
        if (used) return;
        if (connect == null) return;

        // UI가 떠있으면 상호작용 막기(안전)
        if (DialogueUI.I != null && DialogueUI.I.IsOpen())
            return;

        // 이미 먹었으면 무시
        if (PlayerPrefs.GetInt(connect.worldSaveKey, 0) == 1)
            return;

        used = true;

        // 1) 저장 (해금)
        PlayerPrefs.SetInt(connect.worldSaveKey, 1);
        PlayerPrefs.Save();

        // 2) 아이템 스폰(연출)
        if (itemPrefab != null)
            Instantiate(itemPrefab, transform.position + spawnOffset, Quaternion.identity);
        else
            Debug.LogWarning("[ItemPickup] itemPrefab not linked. (OK if no spawn FX needed)");

        // 3) 대사창 락 모드: E로 못 나감 + 홈 버튼만
        if (DialogueUI.I != null)
        {
            DialogueUI.I.OpenLockedWithHomeButton(
                "획득",
                "새로운 아이템을 획득했다!",
                homeSceneName
            );
        }
        else
        {
            Debug.LogWarning("[ItemPickup] DialogueUI.I is null. Put DialogueUI in the scene.");
        }


        // 5) 힌트 UI 숨김(있으면)
        if (InteractHintUI.I != null)
            InteractHintUI.I.Hide();
    }
}
