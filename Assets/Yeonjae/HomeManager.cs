using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HomeManager : MonoBehaviour
{
    // 1. 데이터 구조 정의 (여기에만 [System.Serializable]이 있어야 합니다)
    [System.Serializable]
    public class MapState
    {
        public string saveKey;       // 예: NorthClear
        public Button moveButton;    // 이동 버튼
        public GameObject itemIcon;  // 인벤토리 아이콘
    }

    [Header("맵별 설정 (순서: 북, 동, 남, 서)")]
    public MapState[] maps;

    [Header("배경 설정")]
    public Image backgroundImage;
    public Sprite[] backgroundSprites;

    void Start()
    {
        UpdateHomeState();
    }

    public void UpdateHomeState()
    {
        // 1. 클리어 진행도 계산
        int clearCount = 0;
        if (PlayerPrefs.GetInt("NorthClear", 0) == 1) clearCount = 1;
        if (PlayerPrefs.GetInt("EastClear", 0) == 1) clearCount = 2;
        if (PlayerPrefs.GetInt("SouthClear", 0) == 1) clearCount = 3;
        if (PlayerPrefs.GetInt("WestClear", 0) == 1) clearCount = 4;

        // 2. 배경 교체
        if (backgroundImage != null && clearCount < backgroundSprites.Length)
        {
            backgroundImage.sprite = backgroundSprites[clearCount];
        }

        // 3. 버튼 및 아이콘 상태 업데이트
        for (int i = 0; i < maps.Length; i++)
        {
            if (maps[i].itemIcon != null)
                maps[i].itemIcon.SetActive(i < clearCount);

            if (maps[i].moveButton != null)
                maps[i].moveButton.interactable = false;
        }

        // 4. 다음 진행할 버튼만 활성화
        if (clearCount < maps.Length && maps[clearCount].moveButton != null)
        {
            maps[clearCount].moveButton.interactable = true;
        }
    }

    // 모든 맵 이동 공용 함수
    public void MoveToWorld(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    // 데이터 초기화
    public void ResetAllData()
    {
        PlayerPrefs.DeleteAll();
        UpdateHomeState();
        Debug.Log("데이터 초기화 완료");
    }
}