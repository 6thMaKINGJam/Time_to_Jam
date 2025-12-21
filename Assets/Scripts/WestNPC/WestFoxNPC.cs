using UnityEngine;

public class WestFoxNPC : MonoBehaviour, IInteractable
{
    [Header("Identity")]
    public string speakerName = "여우";

    [Header("First Encounter Line")]
    [TextArea(2, 6)]
    public string firstLine =
        "인간들은 밤이 되면 무섭지 않니?\n" +
        "걱정마··· 별일 없을걸.\n" +
        "어차피 밝아질텐데.";

    [Header("Choice A: Say Goodbye")]
    [TextArea(1, 3)]
    public string[] goodbyeLines =
    {
        "잘가, 인간.",
        "하루종일 보니까 정들었을지도..."
    };

    [Header("Choice B: See You Again")]
    [TextArea(1, 3)]
    public string[] hurryLines =
    {
        "다시 보긴 싫은데.",
        "여기 영영 있고 싶은 게 아니라면 빨리 가는 게 좋을걸."
    };

    [SerializeField] private bool alreadyTalked = false;

    public string GetPrompt() => "E : 대화하기";

    public void Interact()
    {
        if (DialogueUI.I == null) return;

        // 이 대사는 1회성으로 끝내는 게 연출상 좋음
        if (alreadyTalked)
            return;

        alreadyTalked = true;

        // 첫 대사
        DialogueUI.I.Open(speakerName, new string[] { firstLine });

        // 선택지 바로 표시
        DialogueUI.I.ShowChoicesOnly(
            "> 잘 있어",
            () =>
            {
                DialogueUI.I.OpenOnePage(speakerName, goodbyeLines);

                // (*확장) 엔딩 직전 플래그
                // GameState.I.foxFarewell = true;
            },
            "> 다음에 봐",
            () =>
            {
                DialogueUI.I.OpenOnePage(speakerName, hurryLines);
            }
        );
    }
}
