using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrologueManager : MonoBehaviour
{
    public GameObject messageCanvas;
    public GameObject mailCanvas;
    public GameObject houseCanvas;

    // Start is called before the first frame update
    void Start()
    {
        messageCanvas.SetActive(true);
        mailCanvas.SetActive(false);
        houseCanvas.SetActive(false);
    }

    public void OpenMail()
    {
        messageCanvas.SetActive(false);
        mailCanvas.SetActive(true);
        houseCanvas.SetActive(false);
    }

    public void OpenHouse()
    {
        messageCanvas.SetActive(false);
        mailCanvas.SetActive(false);
        houseCanvas.SetActive(true);
    }
}
