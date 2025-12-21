using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmManagerSetter : MonoBehaviour
{
    [Header("BGM For This Scene")]
    public AudioClip sceneBGM;

    // Start is called before the first frame update
    void Start()
    {
        if (BGMManager.instance != null)
        {
            AudioSource source = BGMManager.instance.GetComponent<AudioSource>();

            if (source.clip != sceneBGM)
            {
                source.Stop();
                source.clip = sceneBGM;
                source.Play();
            }
        }
    }
}
