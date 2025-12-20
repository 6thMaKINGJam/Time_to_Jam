using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeConnectManager : MonoBehaviour
{
    public string worldSaveKey;
    public string homeSceneName = "02_Home";

    public void ClearAndGoHome()
    {
        PlayerPrefs.SetInt(worldSaveKey, 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene(homeSceneName);
    }
}
