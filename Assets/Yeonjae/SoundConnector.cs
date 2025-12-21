using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundConnector : MonoBehaviour
{
    // Start is called before the first frame update
    public void ClickSound()
    {
        if (SfxManager.instance != null)
        {
            SfxManager.instance.PlayClick();
        }
    }
}
