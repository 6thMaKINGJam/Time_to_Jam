using UnityEngine;

public class WestButterflyNPC : MonoBehaviour, IInteractable
{
    [Header("Identity")]
    public string speakerName = "나비";

    [Header("First Encounter Line")]
    [TextArea(1, 3)]
    public string firstLine = "안녕, 네 번째 보네.";

    [Header("Choice A: Greet (Final Hint)")]
    [TextArea(1, 3)]
    public string[] greetLines =
    {
        "달콤하고 뾰족한 무언가에",
        "너가 찾는 게 있을 거야."
    };

    [Header("Choice B: Say Goodbye")]
    [TextArea(2, 4)]
    public string[] goodbyeLines =
    {
        "다음에는 못 봐.",
        "이제 난 끝이거든.",
        "어쨌든 즐거웠어.",
        "안녕."
    };

    [Header("Repeat Line (2nd+)")]
    public string repeatLine = "안녕...";

    bool alreadyFinished = false;

    public string GetPrompt() => "E : 대화하기";

    public void Interact()
    {
        if (DialogueUI.I == null) return;

        if (alreadyFinished)
        {
            DialogueUI.I.Open(speakerName, new string[] { repeatLine });
            return;
        }

        // 1회차
        alreadyFinished = true;

        DialogueUI.I.Open(speakerName, new string[] { firstLine });

        DialogueUI.I.ShowChoicesOnly(
            "> 안녕",
            () =>
            {
                DialogueUI.I.OpenOnePage(speakerName, greetLines);
            },
            "> 다음에 봐",
            () =>
            {
                DialogueUI.I.OpenOnePage(speakerName, goodbyeLines);
            }
        );
    }
}

