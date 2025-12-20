using UnityEngine;

public class FoxNPC : MonoBehaviour, IInteractable
{
    [Header("Identity")]
    public string speakerName = "여우";

    [Header("Dialogue Lines")]
    [TextArea(2, 4)]
    public string firstLine = "어라, 인간이잖아? 잡아먹겠어, 와앙!";

    [TextArea(2, 4)]
    public string[] runAwayHintLines =
    {
        "겁쟁이, 거짓말이야! 너가 찾는 건 동그라미겠지?",
        "동그라미는 동그라미에 있지. 설마 못 알아듣는거야?",
        "검은 동그라미!"
    };

    [TextArea(2, 4)]
    public string[] laughLines = new string[]
    {
        "흥, 웃기는. 나 무시하니? 됐어, 가버려."
    };

    [TextArea(2, 4)]
    public string repeatLine = "한 번 말한 건 다시 안 말해.";

    // 상태: 한번이라도 선택지를 진행했는가?
    [SerializeField] bool alreadyTalked = false;

    public string GetPrompt() => "E : 대화하기";

    public void Interact()
    {
        if (DialogueUI.I == null) return;

        // 이미 한 번 대화했다면 repeatLine 출력
        if (alreadyTalked)
        {
            DialogueUI.I.Open(speakerName, new string[] { repeatLine });
            return;
        }

        // 1회차: 첫 줄 + 선택지
        DialogueUI.I.Open(speakerName, new string[] { firstLine });

        DialogueUI.I.ShowChoicesOnly(

            // 도망치는 선택지 > 힌트 제공
            "> 도망친다",
            () =>
            {
                alreadyTalked = true;
                DialogueUI.I.OpenOnePage(speakerName, runAwayHintLines);

            },
            // 웃는 선택지 > 힌트 주지 않고 끝.
            "> 웃는다",
            () =>
            {
                alreadyTalked = true;
                DialogueUI.I.OpenOnePage(speakerName, laughLines);
            }
        );

    }
}
