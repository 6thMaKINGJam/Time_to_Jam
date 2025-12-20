using UnityEngine;

public class NPCInteract : MonoBehaviour, IInteractable
{
    public string GetPrompt()
    {
        return "E : 대화하기"; // IInteractable.cs 인터페이스로 구현
    }


    public void Interact()
    {
        Debug.Log("NPC와 상호작용 성공!"); // 디버깅용
    }
}
