using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HappyFinalController : MonoBehaviour
{
    [Header("Stages")]
    [SerializeField] private GameObject clockStage;
    [SerializeField] private GameObject letterStage;
    [SerializeField] private GameObject mentText;

    [Header("Clock Images")]
    [SerializeField] private GameObject clockClosed;
    [SerializeField] private GameObject clockOpened;

    [Header("Buttons")]
    [SerializeField] private Button enterButton;
    [SerializeField] private Button backButton;

    [Header("Letter")]
    [SerializeField] private TMP_Text letterBody;

    [TextArea]
    [SerializeField] private string letterText =
        "용사님,\n\n" +
        "믿음직스럽진 않았는데 해냈네요?\n\n" +
        "보물을 찾아주셔서 감사해요\n\n" +
        "보물의 정체는 굳이 말하지 않아도 알겠죠?\n\n" +
        "덕분에 모든 것이 제시간에 돌아오고\n" +
        "세계는 제 할 일을 하고 있어요\n\n" +
        "이 세계의 시간을 되돌려주어서 감사합니다";

    [Header("Animation")]
    [SerializeField] private float popDuration = 0.3f;
    [SerializeField] private float startScale = 0.8f;

    private bool opened = false;

    private void Awake()
    {
        if (clockStage != null) clockStage.SetActive(true);
        if (letterStage != null) letterStage.SetActive(false);

        if (clockClosed != null) clockClosed.SetActive(true);
        if (clockOpened != null) clockOpened.SetActive(false);
        if (mentText != null) mentText.SetActive(true);

        if (enterButton != null) enterButton.gameObject.SetActive(true);

        if (letterBody != null) letterBody.text = letterText;
        
        // 닫힌 시계를 클릭해서 열리게 하고 싶으면, clockClosed에 Button 컴포넌트가 있어야 함
        var closedBtn = clockClosed != null ? clockClosed.GetComponent<Button>() : null;
        if (closedBtn != null) closedBtn.onClick.AddListener(OpenClock);

        if (enterButton != null) enterButton.onClick.AddListener(Enter);
        if (backButton != null) backButton.onClick.AddListener(Back);
    }

    public void OpenClock()
    {
        if (opened) return;
        opened = true;

        if (clockClosed != null) clockClosed.SetActive(false);
        if (clockOpened != null) clockOpened.SetActive(true);

        if (enterButton != null) enterButton.gameObject.SetActive(true);

        if (clockOpened != null)
            StartCoroutine(Pop(clockOpened.transform));
    }

    private IEnumerator Pop(Transform target)
    {
        if (target == null) yield break;

        Vector3 start = Vector3.one * startScale;
        Vector3 end = Vector3.one;
        target.localScale = start;

        float t = 0f;
        while (t < popDuration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / popDuration);
            float eased = 1f - Mathf.Pow(1f - p, 3f);
            target.localScale = Vector3.Lerp(start, end, eased);
            yield return null;
        }

        target.localScale = end;
    }

        private void Enter()
    {
        if (mentText != null) mentText.SetActive(false);
        if (clockStage != null) clockStage.SetActive(false);
        if (letterStage != null) letterStage.SetActive(true);
    }


    private void Back()
    {
        if (letterStage != null) letterStage.SetActive(false);
        if (clockStage != null) clockStage.SetActive(true);
    }
}
