using UnityEngine;

/// Keeps the background centered on the camera so it tiles infinitely.
public class BackgroundFollow : MonoBehaviour
{
    private Transform cam;

    private void Start()
    {
        cam = Camera.main.transform;
    }

    private void LateUpdate()
    {
        if (cam == null) return;
        transform.position = new Vector3(cam.position.x, cam.position.y, transform.position.z);
    }
}
