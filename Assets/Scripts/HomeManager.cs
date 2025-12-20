using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HomeManager : MonoBehaviour
{
    [System.Serializable]
    public class MapState
    {
        [Header("Save key (e.g. EastClear)")]
        public string saveKey;

        [Header("Button to go this map")]
        public Button moveButton;

        [Header("Scene name to load")]
        public string sceneName;

        [Header("Optional: icon that appears when cleared")]
        public GameObject itemIcon;
    }

    [Header("Map Progress Order (IMPORTANT: order = progression order)")]
    public MapState[] maps;

    [Header("Optional: background by progress count (0~4)")]
    public Image backgroundImage;
    public Sprite[] backgroundSprites; 

    [Header("Scenes")]
    public string endingSceneName = "99_Ending(bad)";

    void OnEnable()
    {
        UpdateHomeState();
    }

    void Start()
    {
        UpdateHomeState();
    }

    public void UpdateHomeState()
    {
        // 1) 순차 진행 기준으로 clearCount 계산 (중간이 비면 거기서 멈춤)
        int clearCount = 0;

        for (int i = 0; i < maps.Length; i++)
        {
            if (PlayerPrefs.GetInt(maps[i].saveKey, 0) == 1)
                clearCount++;
            else
                break;
        }

        // 2) 배경 교체 (선택)
        if (backgroundImage != null && backgroundSprites != null && backgroundSprites.Length > 0)
        {
            int idx = Mathf.Clamp(clearCount, 0, backgroundSprites.Length - 1);
            backgroundImage.sprite = backgroundSprites[idx];
        }

        // 3) 모든 버튼 잠금 + 아이콘 갱신
        for (int i = 0; i < maps.Length; i++)
        {
            bool cleared = PlayerPrefs.GetInt(maps[i].saveKey, 0) == 1;

            
            if (maps[i].moveButton != null)
                maps[i].moveButton.interactable = false;

            if (maps[i].itemIcon != null)
                maps[i].itemIcon.SetActive(cleared);
        }

        // 4) 4개 다 모으면 엔딩 이동(자동)
        if (clearCount >= maps.Length)
        {
            SceneManager.LoadScene(endingSceneName);
            return;
        }

        // 5) 다음 진행 맵만 열기 (딱 1개)
        if (maps[clearCount].moveButton != null)
            maps[clearCount].moveButton.interactable = true;
    }

    // 버튼 OnClick에 연결할 함수(선택)
    public void GoMapByIndex(int index)
    {
        if (index < 0 || index >= maps.Length) return;

        // 해금 조건: index가 clearCount와 같거나, 이미 클리어했으면 이동 가능하게 하고 싶다면 아래 조건 수정 가능
        int clearCount = 0;
        for (int i = 0; i < maps.Length; i++)
        {
            if (PlayerPrefs.GetInt(maps[i].saveKey, 0) == 1) clearCount++;
            else break;
        }

        bool unlocked = (index == clearCount); // 순차 1개만 오픈
        if (!unlocked) return;

        SceneManager.LoadScene(maps[index].sceneName);
    }

    // 디버그 리셋(선택)
    public void ResetAll()
    {
        for (int i = 0; i < maps.Length; i++)
            PlayerPrefs.DeleteKey(maps[i].saveKey);

        PlayerPrefs.Save();
        UpdateHomeState();
    }
}
