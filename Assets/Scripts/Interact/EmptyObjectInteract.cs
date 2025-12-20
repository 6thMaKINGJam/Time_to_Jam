using UnityEngine;

public class EmptyObjectInteract : MonoBehaviour, IInteractable
{
    public string prompt = "E : 조사하기";
    public string speakerName = ""; 
    [TextArea(1, 2)]
    public string message = "아무것도 없다.";

    public string GetPrompt() => prompt;

    public void Interact()
    {
        if (DialogueUI.I == null) return;

        DialogueUI.I.Open(speakerName, new string[] { message });
    }
}
