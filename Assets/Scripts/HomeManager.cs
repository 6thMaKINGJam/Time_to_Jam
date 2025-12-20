using UnityEngine;
using UnityEngine.UI;

public class HomeManager : MonoBehaviour
{
    [System.Serializable]
    public class MapState
    {
        public string saveKey;       // 저장 키 (예: "NorthClear")
        public Button moveButton;    // 이동 버튼
        public GameObject itemIcon;  // 인벤토리 아이템 아이콘
    }

    [Header("맵별 설정 (순서대로: 북, 동, 남, 서)")]
    public MapState[] maps;

    [Header("배경 설정")]
    public Image backgroundImage;
    public Sprite[] backgroundSprites; // 0:기본, 1:동쪽해금, 2:남쪽해금, 3:서쪽해금...

    void Start()
    {
        UpdateHomeState();
    }

        public void UpdateHomeState()
    {
        // 1. 몇 개를 깼는지 정확히 계산 (북-동-남-서 순서)
        int clearCount = 0;
        if (PlayerPrefs.GetInt("NorthClear", 0) == 1) clearCount = 1;
        if (PlayerPrefs.GetInt("EastClear", 0) == 1) clearCount = 2;
        if (PlayerPrefs.GetInt("SouthClear", 0) == 1) clearCount = 3;
        if (PlayerPrefs.GetInt("WestClear", 0) == 1) clearCount = 4;

        // 2. 배경 교체 (0개 깨면 0번 배경, 1개 깨면 1번 배경...)
        if (clearCount < backgroundSprites.Length)
        {
            backgroundImage.sprite = backgroundSprites[clearCount];
        }

        // 3. 버튼과 아이템 상태 세팅
        for (int i = 0; i < maps.Length; i++)
        {
            // 아이템: 이미 깬 맵의 아이템만 보여줌
            maps[i].itemIcon.SetActive(i < clearCount);

            // 버튼: 일단 모두 끔
            maps[i].moveButton.interactable = false;
        }

        // 4. [핵심] 현재 깨야 할 딱 '하나'의 버튼만 켬
        // 0개 깼으면 0번(북) 버튼 활성화, 1개 깼으면 1번(동) 버튼 활성화...
        if (clearCount < maps.Length)
        {
            maps[clearCount].moveButton.interactable = true;
        }
    }

    // ResetAllData는 삭제하지 마세요! 테스트할 때 아주 유용합니다.
    public void ResetAllData()
    {
        PlayerPrefs.DeleteAll();
        UpdateHomeState();
    }
}