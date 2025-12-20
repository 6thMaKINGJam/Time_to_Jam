using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smooth = 12f;

    void LateUpdate()
    {
        if (!target) return; //target을 플레이어로 해서, 카메라가 플레이어를 따라오게.

        Vector3 desired = new Vector3(target.position.x, target.position.y, -10f);
        transform.position = Vector3.Lerp(transform.position, desired, Time.deltaTime * smooth);
    }
}
