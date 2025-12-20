using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnHomePanel : MonoBehaviour
{
    HomeConnectManager connect;

    void Awake()
    {
        gameObject.SetActive(false);
    }

    // 아이템 먹은 순간 호출
    public void Show(HomeConnectManager connectManager)
    {
        connect = connectManager;
        gameObject.SetActive(true);
    }

    // 버튼 OnClick에 연결
    public void OnClickReturnHome()
    {
        // 엔딩 조건이면 엔딩, 아니면 홈
        if (connect != null)
            connect.ClearAndGoHome();
        else
            SceneManager.LoadScene("02_Home");
    }
}
