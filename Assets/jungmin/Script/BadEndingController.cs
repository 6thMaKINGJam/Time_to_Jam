using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class BadEndingController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private Button exitToTitleButton;

    [Header("Options")]
    [SerializeField] private bool clickAnywhereToAdvance = true;
    [SerializeField] private bool spaceEnterToAdvance = true;

    [Header("Scene")]
    [SerializeField] private string titleSceneName = "00_Title";
    
    private readonly string[] lines =
    {
        "결국 시간 내에 보물을 찾지 못했다...",
        "시간이 점멸하라 세계가 부서졌다\n결국 당신은 환계 세바르를 구하지 못했다",
    };

    private int index = 0;
    private bool finished = false;

    private void Awake()
    {
        if (endPanel != null) endPanel.SetActive(false);

        if (exitToTitleButton != null)
            exitToTitleButton.onClick.AddListener(() => SceneManager.LoadScene(titleSceneName));

        ShowLine(0);
    }

    private void Update()
    {
        if (finished) return;

        bool advanced = false;

        if (clickAnywhereToAdvance && Input.GetMouseButtonDown(0))
            advanced = true;

        if (!advanced && spaceEnterToAdvance &&
            (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)))
            advanced = true;

        if (advanced)
            Advance();
    }

    private void Advance()
    {
        index++;

        if (index < lines.Length)
        {
            ShowLine(index);
            return;
        }

        finished = true;

        if (endPanel != null)
            endPanel.SetActive(true);
    }

    private void ShowLine(int i)
    {
        if (dialogueText != null)
            dialogueText.text = lines[i];
    }
}
