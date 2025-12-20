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
    public string endingSceneName = "99_Ending";

    [Header("✅ Ending Button (Appears when all clear)")]
    public GameObject endingButtonRoot; // 엔딩 버튼 GameObject(또는 부모 Panel)
    public Button endingButton;         // 엔딩 버튼 컴포넌트

    void OnEnable()
    {
        UpdateHomeState();
    }

    void Start()
    {
        // 엔딩 버튼 리스너 1회 연결
        if (endingButton != null)
        {
            endingButton.onClick.RemoveAllListeners();
            endingButton.onClick.AddListener(GoEnding);
        }

        UpdateHomeState();
    }

    public void UpdateHomeState()
    {
        if (maps == null || maps.Length == 0) return;

        // 1) 순차 진행 기준 clearCount 계산 (중간이 비면 거기서 멈춤)
        int clearCount = 0;
        for (int i = 0; i < maps.Length; i++)
        {
            if (PlayerPrefs.GetInt(maps[i].saveKey, 0) == 1) clearCount++;
            else break;
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

        // 4) ✅ 엔딩 버튼 표시/숨김 (자동 이동 제거)
        bool allClear = (clearCount >= maps.Length);

        if (endingButtonRoot != null)
            endingButtonRoot.SetActive(allClear);

        // 5) 다음 진행 맵만 열기 (딱 1개) - allClear면 더 이상 열 필요 없음
        if (!allClear)
        {
            if (maps[clearCount].moveButton != null)
                maps[clearCount].moveButton.interactable = true;
        }
    }

    // 엔딩 버튼 클릭 시에만 이동
    public void GoEnding()
    {
        SceneManager.LoadScene(endingSceneName);
    }

    // 버튼 OnClick에 연결할 함수(선택)
    public void GoMapByIndex(int index)
    {
        if (maps == null || maps.Length == 0) return;
        if (index < 0 || index >= maps.Length) return;

        int clearCount = 0;
        for (int i = 0; i < maps.Length; i++)
        {
            if (PlayerPrefs.GetInt(maps[i].saveKey, 0) == 1) clearCount++;
            else break;
        }

        bool allClear = (clearCount >= maps.Length);
        if (allClear) return; // 다 끝났으면 월드 재입장 막고 싶으면 유지, 허용하려면 이 줄 삭제

        bool unlocked = (index == clearCount); // 순차 1개만 오픈
        if (!unlocked) return;

        SceneManager.LoadScene(maps[index].sceneName);
    }

    // 디버그 리셋(선택)
    public void ResetAll()
    {
        if (maps == null) return;

        for (int i = 0; i < maps.Length; i++)
            PlayerPrefs.DeleteKey(maps[i].saveKey);

        PlayerPrefs.Save();

        if (endingButtonRoot != null)
            endingButtonRoot.SetActive(false);

        UpdateHomeState();
    }

    public void ResetProgress()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("Progress Reset");

        if (endingButtonRoot != null)
            endingButtonRoot.SetActive(false);

        UpdateHomeState();
    }
}
