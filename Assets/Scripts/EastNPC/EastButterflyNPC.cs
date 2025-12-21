using UnityEngine;

public class EastButterflyNPC : MonoBehaviour, IInteractable
{
    [Header("Identity")]
    public string speakerName = "나비";

    [Header("First Encounter Line")]
    [TextArea(2, 6)]
    public string firstLine =
        "안녕, 두 번째 보네. 내 꽃들이 햇빛을 받고 있어.\n" +
        "너도 햇빛 받는 걸 좋아하니?";

    [Header("Choice A: Agree (Hint Success)")]
    [TextArea(1, 3)]
    public string[] agreeLines =
    {
        "오른쪽으로 가면 햇빛이 쨍쨍해. 돌들이 반짝이는 걸 봤어.."
    };

    [Header("Choice B: Dislike (Hint Fail)")]
    [TextArea(1, 3)]
    public string[] dislikeLines =
    {
        "살이 타기 전에 빨리 움직이는 게 좋을거야."
    };

    [Header("Repeat Line")]
    [TextArea(1, 2)]
    public string repeatLine = "비켜줄래? 그림자가 생기잖아.";

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

        // 1회차: 첫 대사 + 선택지
        DialogueUI.I.Open(speakerName, new string[] { firstLine });

        DialogueUI.I.ShowChoicesOnly(
            "> 동의한다",
            () =>
            {
                alreadyTalked = true;

                DialogueUI.I.OpenOnePage(speakerName, agreeLines);

            },
            "> 싫어한다",
            () =>
            {
                alreadyTalked = true;

                DialogueUI.I.OpenOnePage(speakerName, dislikeLines);
            }
        );
    }
}
