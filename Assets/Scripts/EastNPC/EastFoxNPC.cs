using UnityEngine;

public class FoxNPC_Second : MonoBehaviour, IInteractable
{
    [Header("Identity")]
    public string speakerName = "여우";

    [Header("First Encounter Line")]
    [TextArea(2, 6)]
    public string firstLine =
        "또 보네. 그런데 이번에도 여기 있을 거라고 생각해서 온 건 아니겠지?\n" +
        "에이, 설마, 그 정도로 바보는 아니겠지.";

    [Header("Choice A: Turn Away (Hint)")]
    [TextArea(2, 6)]
    public string[] turnAwayLines =
    {
        "야아, 어디가, 삐졌냐?",
        "글쎄, 여기로 올 때 어떤 덤불에서 소리가 났던 거 같기도 하고.",
        "그래, 어떤 바위랑 같이 있었어."
    };

    [Header("Choice B: Ask")]
    [TextArea(1, 3)]
    public string[] askLines =
    {
        "나도 몰라.",
        "내가 척척박사니?"
    };

    [Header("Repeat Line")]
    [TextArea(1, 2)]
    public string repeatLine = "낮잠 잘거야, 말 걸지 마.";

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

        // 1회차: 첫 대사
        DialogueUI.I.Open(speakerName, new string[] { firstLine });

        // 바로 선택지 표시 (bodyText 유지)
        DialogueUI.I.ShowChoicesOnly(
            "> 돌아선다",
            () =>
            {
                alreadyTalked = true;

                // 힌트는 한 페이지로 출력 → E 한 번에 닫힘
                DialogueUI.I.OpenOnePage(speakerName, turnAwayLines);

            },
            "> 물어본다",
            () =>
            {
                alreadyTalked = true;
                DialogueUI.I.OpenOnePage(speakerName, askLines);
            }
        );
    }
}
