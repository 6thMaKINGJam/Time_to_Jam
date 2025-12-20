using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float speed = 4.5f;
    Rigidbody2D rb;
    Vector2 input;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 대화 중일 경우 움직이는 입력 차단
        if (DialogueUI.I != null && DialogueUI.I.IsOpen())
        {
            input = Vector2.zero;
            return;
        }
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        input = input.normalized;
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + input * speed * Time.fixedDeltaTime);
    }
}
