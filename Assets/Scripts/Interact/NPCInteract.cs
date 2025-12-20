using UnityEngine;

public class NPCInteract : MonoBehaviour, IInteractable
{
    public string speakerName = "NPC";
    [TextArea(2, 4)]
    public string[] dialogueLines;
    
    public string GetPrompt()
    {
        return "E : 대화하기"; // IInteractable.cs 인터페이스로 구현
    }


    public void Interact()
    {
         Debug.Log("E Interact called");
         
        // DialogueUI.cs 사용
        if (DialogueUI.I == null) return;

        // 이미 대화 중이면 다음 줄
        if (DialogueUI.I.IsOpen())
        {
            DialogueUI.I.Next();
        }
        else
        // 대화 중이 아닐 경우 Open
        {
            DialogueUI.I.Open(speakerName, dialogueLines);
        }
    }
}
