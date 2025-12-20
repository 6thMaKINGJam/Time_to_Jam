using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smooth = 12f;

    [Header("Bounds")]
    public BoxCollider2D bounds; 

    Camera cam;
    float minX, maxX, minY, maxY;

    void Start()
    {
        cam = GetComponent<Camera>();

        // 카메라 화면 반크기
        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;

        // bounds의 실제 월드 좌표 Bounds
        Bounds b = bounds.bounds;

        // 카메라가 화면 크기만큼 안쪽으로만 움직이게 제한
        minX = b.min.x + halfW;
        maxX = b.max.x - halfW;
        minY = b.min.y + halfH;
        maxY = b.max.y - halfH;
    }

    void LateUpdate()
    {
        if (!target) return;

        Vector3 desired = new Vector3(target.position.x, target.position.y, -10f);

        // Clamp를 사용하여 맵 밖으로 카메라가 못 나가게 막기
        desired.x = Mathf.Clamp(desired.x, minX, maxX);
        desired.y = Mathf.Clamp(desired.y, minY, maxY);

        transform.position = Vector3.Lerp(transform.position, desired, Time.deltaTime * smooth);
    }
}
