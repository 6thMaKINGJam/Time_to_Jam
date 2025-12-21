using UnityEngine;

public class SouthFoxNPC : MonoBehaviour, IInteractable
{
    [Header("Identity")]
    public string speakerName = "여우";

    [Header("First Encounter Line")]
    [TextArea(2, 6)]
    public string firstLine =
        "아까는 아니었는데 너도 나처럼 주황색이 됐네. 아직도 안 떠났구나.\n" +
        "의지는 인정해줄게.";

    [Header("Choice A: Ask (AppleTree Hint)")]
    [TextArea(1, 3)]
    public string[] askLines =
    {
        "글쎄, 떠나는 방법은 사과나무에게 물어보면 알지도?",
        "행운을 빌어."
    };

    [Header("Choice B: Dislike Orange")]
    [TextArea(1, 3)]
    public string[] dislikeLines =
    {
        "가버려!"
    };

    [Header("Repeat Line")]
    [TextArea(1, 2)]
    public string repeatLine = "귀찮게 왜 이래.";

    [SerializeField] private bool alreadyTalked = false;

    public string GetPrompt() => "E : 대화하기";

    public void Interact()
    {
        if (DialogueUI.I == null) return;

        // 2회차 이후
        if (alreadyTalked)
        {
            DialogueUI.I.Open(speakerName, new string[] { repeatLine });
            return;
        }

        // 1회차: 첫 줄 + 선택지
        DialogueUI.I.Open(speakerName, new string[] { firstLine });

        DialogueUI.I.ShowChoicesOnly(
            "> 떠날래",
            () =>
            {
                alreadyTalked = true;
                DialogueUI.I.OpenOnePage(speakerName, askLines);

            },
            "> 주황색 싫어",
            () =>
            {
                alreadyTalked = true;
                DialogueUI.I.OpenOnePage(speakerName, dislikeLines);
            }
        );
    }
}
