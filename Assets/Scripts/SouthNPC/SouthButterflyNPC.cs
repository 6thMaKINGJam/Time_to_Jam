using UnityEngine;

public class ButterflyNPC_Third : MonoBehaviour, IInteractable
{
    [Header("Identity")]
    public string speakerName = "나비";

    [Header("First Encounter Line")]
    [TextArea(2, 8)]
    public string firstLine =
        "안녕, 세 번째 보네. 드디어 꽃들이 주황색으로 물들었어.\n" +
        "사실은 흰색 꽃이지만.\n" +
        "진짜 주황색 꽃을 구별할 수 있어?";

    [Header("Choice A: Nod (Later)")]
    [TextArea(1, 3)]
    public string[] nodLines =
    {
        "그러면 나중에 가르쳐줘."
    };

    [Header("Choice B: Shake Head (Apple Tree Hint)")]
    [TextArea(1, 3)]
    public string[] shakeHeadLines =
    {
        "진짜 주황색 꽃은 딱 하나야.",
        "말하는 사과나무에게 물어봐."
    };

    [Header("Repeat Line")]
    [TextArea(1, 2)]
    public string repeatLine = "사과나무에게 가.";

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
            "> 끄덕인다",
            () =>
            {
                alreadyTalked = true;
                DialogueUI.I.OpenOnePage(speakerName, nodLines);

            },
            "> 고개를 젓는다",
            () =>
            {
                alreadyTalked = true;
                DialogueUI.I.OpenOnePage(speakerName, shakeHeadLines);

            }
        );
    }
}
