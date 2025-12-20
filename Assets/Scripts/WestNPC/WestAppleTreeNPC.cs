using UnityEngine;

public class WestAppleTreeNPC : MonoBehaviour, IInteractable
{
    [Header("Identity")]
    public string speakerName = "말하는 사과나무";

    [Header("First Encounter")]
    public string firstLine = "한 입 베어먹은 사과는?";

    [Header("Second+ Encounter")]
    public string repeatLine = "......";

    bool alreadyTalked = false;

    public string GetPrompt() => "E : 말 걸기";

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
