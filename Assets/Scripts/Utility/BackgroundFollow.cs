using UnityEngine;

/// Snaps the background to the nearest tile boundary so the road tiles
/// scroll naturally in world space as the camera moves.
/// The background size must be large enough to always fill the camera view.
public class BackgroundFollow : MonoBehaviour
{
    // Must match: texture pixels / PPU  →  1024px / 100 PPU = 10.24 world units
    [SerializeField] private float tileWorldSize = 10.24f;

    private Transform cam;

    private void Start()
    {
        cam = Camera.main.transform;
    }

    private void LateUpdate()
    {
        if (cam == null) return;

        // Snap to tile grid — jumps are exactly one tile so the seam is invisible
        float x = Mathf.Round(cam.position.x / tileWorldSize) * tileWorldSize;
        float y = Mathf.Round(cam.position.y / tileWorldSize) * tileWorldSize;
        transform.position = new Vector3(x, y, transform.position.z);
    }
}
