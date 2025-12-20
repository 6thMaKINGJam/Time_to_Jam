using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    public float radius = 1.2f;
    public LayerMask interactMask;

    IInteractable current;

    void Update()
    {
        // 대화 중일 경우, E는 Next
        if (DialogueUI.I != null && DialogueUI.I.IsOpen())
        {
            // 선택지 떠있는 동안에는 Next가 막혀있음
            if (Input.GetKeyDown(KeyCode.E))
                DialogueUI.I.Next();

            // 대화 중에는 타겟 탐색/상호작용 실행 금지
            if (InteractHintUI.I != null) InteractHintUI.I.Hide();
            current = null;
            return;
        }

        FindTarget();

        // 타겟 발견 시 E키를 누르면 상호작용
        if (current != null && Input.GetKeyDown(KeyCode.E))
        {
            current.Interact();
        }
    }

    // 상호작용할 타겟을 찾는다.
    void FindTarget()
    {
        current = null;

        Collider2D hit = Physics2D.OverlapCircle(
            transform.position,
            radius,
            interactMask
        );

        if (!hit)
        {
            InteractHintUI.I.Hide();
            return;
        }

        current = hit.GetComponent<IInteractable>();

        if (current != null)
            InteractHintUI.I.Show(current.GetPrompt());
        else
            InteractHintUI.I.Hide();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
