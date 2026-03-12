using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 10f;

    private Vector3 offset;

    private void Start()
    {
        if (target != null)
            offset = transform.position - target.position;
        else
            offset = new Vector3(0, 0, -10f);
    }

    private void LateUpdate()
    {
        if (target == null) return;
        Vector3 desired = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime);
    }
}
