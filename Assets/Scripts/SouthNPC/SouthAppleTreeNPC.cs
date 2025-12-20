using UnityEngine;

public class AppleTreeNPC_OrangeHint : MonoBehaviour, IInteractable
{
    [Header("Identity")]
    public string speakerName = "말하는 사과나무";

    [Header("First Encounter")]
    [TextArea(1, 3)]
    public string firstLine = "나와 가장 가까운 꽃.";

    [Header("Second+ Encounter")]
    [TextArea(1, 2)]
    public string repeatLine = "......";

    [SerializeField] private bool alreadyTalked = false;

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

        // 한 페이지로 출력(문장 1줄이라 E 한번에 닫힘)
        DialogueUI.I.Open(speakerName, new string[] { firstLine });

        // (*확장) 여기서 근처 주황색 꽃 오브젝트 강조/활성화 가능
        // orangeFlowerGlow.SetActive(true);
    }
}
