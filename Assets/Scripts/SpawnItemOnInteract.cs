using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnItemOnInteract : MonoBehaviour, IInteractable
{
    [Header("Prompt")]
    public string prompt = "E : 조사하기";

    [Header("Unlock Key (HomeManager가 보는 키)")]
    public string unlockKey = "NorthClear"; // North > East > South > West 

    [Header("Spawn (optional)")]
    public GameObject itemPrefab; // 옆에 생기는 key 아이템
    public Vector3 spawnOffset = new Vector3(0.8f, 0f, 0f);

    [Header("After Pickup")]
    public bool goHomeAfterPickup = true;
    public string homeSceneName = "02_Home";

    bool used = false;

    public string GetPrompt() => prompt;

    void Start()
    {
        // 이미 먹었으면 다시 못 먹게(또는 오브젝트 비활성)
        if (PlayerPrefs.GetInt(unlockKey, 0) == 1)
        {
            used = true;
            gameObject.SetActive(false); 
        }
    }

    public void Interact()
    {
        if (used) return;
        used = true;

        // 1) 진행 해금 저장
        PlayerPrefs.SetInt(unlockKey, 1);
        PlayerPrefs.Save();

        // 2) 메시지
        if (DialogueUI.I != null)
            DialogueUI.I.Open("시스템", new string[] { "월드의 새로운 아이템을 획득했다!" });

        // 3) 옆에 아이템 생성(연출)
        if (itemPrefab != null)
            Instantiate(itemPrefab, transform.position + spawnOffset, Quaternion.identity);

        // 4) 홈으로 복귀하면 즉시 다음 맵 해금 확인 가능
        if (goHomeAfterPickup)
            SceneManager.LoadScene(homeSceneName);
    }
}
