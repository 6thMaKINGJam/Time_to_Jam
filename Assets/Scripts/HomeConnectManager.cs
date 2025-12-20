using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeConnectManager : MonoBehaviour
{
    [Header("Save Key for this world (e.g. EastClear)")]
    public string worldSaveKey = "EastClear";

    [Header("Scenes")]
    public string homeSceneName = "02_Home";
    public string endingSceneName = "99_Ending";

    [Header("All Keys (for ending check)")]
    public string[] allWorldKeys = new string[] { "EastClear", "SouthClear", "WestClear", "NorthClear" };

    // 월드에서 아이템 획득 완료 시 호출
    public void ClearAndGoHome()
    {
        // 1) 저장
        PlayerPrefs.SetInt(worldSaveKey, 1);
        PlayerPrefs.Save();

        // 2) 엔딩 조건 체크
        if (IsAllCleared())
        {
            SceneManager.LoadScene(endingSceneName);
            return;
        }

        // 3) 홈으로 복귀
        SceneManager.LoadScene(homeSceneName);
    }

    bool IsAllCleared()
    {
        if (allWorldKeys == null || allWorldKeys.Length == 0) return false;

        for (int i = 0; i < allWorldKeys.Length; i++)
        {
            if (PlayerPrefs.GetInt(allWorldKeys[i], 0) != 1)
                return false;
        }
        return true;
    }
}
