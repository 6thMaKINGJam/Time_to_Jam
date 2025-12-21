using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxManager : MonoBehaviour
{
    public static SfxManager instance;
    private AudioSource audioSource;

    public AudioClip clickClip;
    public AudioClip doorOpenClip;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
    }

    public void PlayClick()
    {
        if (audioSource != null)
        {
            audioSource.PlayOneShot(clickClip);
        }
    }

    public static void PlayClickFromAnywhere()
    {
        if (instance != null && instance.clickClip != null)
        {
            instance.audioSource.PlayOneShot(instance.clickClip);
        }
    }

    public void PlayDoor()
    {
        if (audioSource != null && doorOpenClip != null)
        {
            audioSource.PlayOneShot(doorOpenClip);
        }
    }
}
