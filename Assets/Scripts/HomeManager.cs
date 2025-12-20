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

        // 3) 모든 버튼 잠금 + 아이콘 갱신 + 버튼 클릭 리스너 연결
        for (int i = 0; i < maps.Length; i++)
        {
            int index = i; // ★ 클로저 버그 방지

            bool cleared = PlayerPrefs.GetInt(maps[i].saveKey, 0) == 1;

            if (maps[i].moveButton != null)
            {
                // 버튼 리스너를 매번 최신으로 리셋
                maps[i].moveButton.onClick.RemoveAllListeners();

                // 기본 잠금
                maps[i].moveButton.interactable = false;

                // 클릭 시 씬 로드
                maps[i].moveButton.onClick.AddListener(() =>
                {
                    // (선택) 해금 안 됐으면 막기
                    int currentClearCount = GetClearCountSequential();
                    bool allClear = (currentClearCount >= maps.Length);
                    if (!allClear && index != currentClearCount) return;

                    SceneManager.LoadScene(maps[index].sceneName);
                });
            }

            if (maps[i].itemIcon != null)
                maps[i].itemIcon.SetActive(cleared);
        }

        // 4) ✅ 엔딩 버튼 표시/숨김 (자동 이동 없음)
        bool all = (clearCount >= maps.Length);

        if (endingButtonRoot != null)
            endingButtonRoot.SetActive(all);

        // 5) 다음 진행 맵만 열기 (딱 1개) - allClear면 더 이상 열 필요 없음
        if (!all)
        {
            if (maps[clearCount].moveButton != null)
                maps[clearCount].moveButton.interactable = true;
        }
    }

    int GetClearCountSequential()
    {
        if (maps == null) return 0;

        int clearCount = 0;
        for (int i = 0; i < maps.Length; i++)
        {
            if (PlayerPrefs.GetInt(maps[i].saveKey, 0) == 1) clearCount++;
            else break;
        }
        return clearCount;
    }

    // 엔딩 버튼 클릭 시에만 이동
    public void GoEnding()
    {
        SceneManager.LoadScene(endingSceneName);
    }

    // 디버그 리셋
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
