using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrologueManager : MonoBehaviour
{
    public GameObject messageCanvas;
    public GameObject mailCanvas;

    // Start is called before the first frame update
    void Start()
    {
        messageCanvas.SetActive(true);
        mailCanvas.SetActive(false);
    }

    public void OpenMail()
    {
        messageCanvas.SetActive(false);
        mailCanvas.SetActive(true);
    }
}
