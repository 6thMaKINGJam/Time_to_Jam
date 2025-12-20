using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BadEndingController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text dialogueText;

    [Tooltip("ExitToTitleButton 게임오브젝트 자체를 넣어주세요(처음엔 꺼둘 것)")]
    [SerializeField] private GameObject exitButtonObject;

    [Tooltip("ExitToTitleButton의 Button 컴포넌트를 넣어주세요")]
    [SerializeField] private Button exitToTitleButton;

    [Header("Scene")]
    [SerializeField] private string titleSceneName = "00_Title";

    [Header("Timing")]
    [SerializeField] private float firstLineDelay = 0f;   // 첫 문장 시작 전 딜레이
    [SerializeField] private float lineInterval = 1.0f;   // 문장 간 간격
    [SerializeField] private float showExitDelay = 1.0f;  // 마지막 문장 후 버튼 뜨기까지

    private readonly string[] lines =
    {
         "결국 시간 내에 보물을 찾지 못했다...",
        "시간이 전부 섞이고, 세계가 무너졌다\n결국 당신은 원래 세계로 돌아가지 못했다",
    };

    private void Awake()
    {
        // 시작할 때 버튼 숨김
        if (exitButtonObject != null)
            exitButtonObject.SetActive(false);

        // 버튼 누르면 타이틀로
        if (exitToTitleButton != null)
            exitToTitleButton.onClick.AddListener(() => SceneManager.LoadScene(titleSceneName));

        StartCoroutine(PlayEndingSequence());
    }

    private IEnumerator PlayEndingSequence()
    {
        yield return new WaitForSeconds(firstLineDelay);

        for (int i = 0; i < lines.Length; i++)
        {
            if (dialogueText != null)
                dialogueText.text = lines[i];

            yield return new WaitForSeconds(lineInterval);
        }

        yield return new WaitForSeconds(showExitDelay);

        // 멘트 끝나면 버튼만 등장
        if (exitButtonObject != null)
            exitButtonObject.SetActive(true);
    }
}
