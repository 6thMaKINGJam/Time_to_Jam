using UnityEngine;

public class AppleTreeNPC_Riddle : MonoBehaviour, IInteractable
{
    [Header("Identity")]
    public string speakerName = "말하는 사과나무";

    [Header("Riddle (First Encounter)")]
    [TextArea(4, 8)]
    public string riddleLine =
        "여긴... _쪽 월드다. 그 쪽으로 가.";

    [Header("Second+ Encounter")]
    [TextArea(1, 2)]
    public string repeatLine = "......";

    [SerializeField] private bool alreadyTalked = false;

    public string GetPrompt() => "E : 말 걸기";

    public void Interact()
    {
        if (DialogueUI.I == null) return;

        // 두 번째부터는 침묵
        if (alreadyTalked)
        {
            DialogueUI.I.Open(speakerName, new string[] { repeatLine });
            return;
        }

        alreadyTalked = true;

        // 수수께끼 제시 (한 페이지)
        DialogueUI.I.Open(speakerName, new string[] { riddleLine });

    }
}
