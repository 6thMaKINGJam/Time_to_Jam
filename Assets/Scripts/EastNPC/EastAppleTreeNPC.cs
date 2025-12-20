using UnityEngine;

public class AppleTreeNPC_Riddle : MonoBehaviour, IInteractable
{
    [Header("Identity")]
    public string speakerName = "말하는 사과나무";

    [Header("Riddle (First Encounter)")]
    [TextArea(4, 8)]
    public string riddleLine =
        "사과 n개가 있다.\n" +
        "n의 3배에서 5를 빼고,\n" +
        "그 결과를 2로 나눈 뒤,\n" +
        "다시 7을 더했더니\n" +
        "처음 n의 2배가 되었다.\n\n" +
        "n개인 무언가를 찾아라.";

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
