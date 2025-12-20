using UnityEngine;

public class NorthAppleTreeNPC : MonoBehaviour, IInteractable
{
    [Header("Identity")]
    public string speakerName = "말하는 사과나무";

    [Header("First Encounter")]
    [TextArea(2, 4)]
    public string firstLine = "검이 정색하면?";

    [Header("Second+ Encounter")]
    [TextArea(1, 2)]
    public string repeatLine = "......";

    [SerializeField] private bool alreadyTalked = false;

    public string GetPrompt() => "E : 대화하기";

    public void Interact()
    {
        if (DialogueUI.I == null) return;

        if (alreadyTalked)
        {
            DialogueUI.I.Open(speakerName, new string[] { repeatLine });
            return;
        }

        alreadyTalked = true;

        DialogueUI.I.Open(speakerName, new string[] { firstLine });
    }
}
