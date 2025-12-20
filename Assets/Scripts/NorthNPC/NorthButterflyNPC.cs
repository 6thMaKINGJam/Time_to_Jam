using UnityEngine;

public class NorthButterflyNPC : MonoBehaviour, IInteractable
{
    [Header("Identity")]
    public string speakerName = "나비";

    [Header("First Line")]
    [TextArea(2, 4)]
    public string firstLine = "안녕, 처음 보네. 나는 흰색 꽃이 좋아. 너는?";

    [Header("Choice A: Agree (White)")]
    [TextArea(2, 4)]
    public string[] agreeLines =
    {
        "내 흰색 꽃은 절대로 건들지 마.",
        "다른색 꽃이라면 위쪽에서 본 거 같아."
    };

    [Header("Choice B: Orange is better")]
    [TextArea(2, 4)]
    public string[] orangeLines =
    {
        "취향이 독특하네.",
        "주황색은 좀 기다려야 할텐데."
    };

    [Header("Repeat Line")]
    [TextArea(2, 4)]
    public string repeatLine = "나는 꿀을 먹느라 바빠.";

    [SerializeField] bool alreadyTalked = false;

    public string GetPrompt() => "E : 대화하기";

    public void Interact()
    {
        if (DialogueUI.I == null) return;

        // 2회차부터 고정
        if (alreadyTalked)
        {
            DialogueUI.I.Open(speakerName, new string[] { repeatLine });
            return;
        }

        // 1회차: 첫 줄 + 선택지(바로 아래 버튼)
        DialogueUI.I.Open(speakerName, new string[] { firstLine });

        DialogueUI.I.ShowChoicesOnly(
            "> 동의해.",
            () =>
            {
                alreadyTalked = true;

                DialogueUI.I.OpenOnePage(speakerName, agreeLines);

            },
            "> 난 주황색이 더 좋아.",
            () =>
            {
                alreadyTalked = true;
                DialogueUI.I.OpenOnePage(speakerName, orangeLines);
            }
        );
    }
}
