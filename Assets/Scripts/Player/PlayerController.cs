using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private FloatingJoystick joystick;
    [SerializeField] private InputActionAsset inputActions;

    private InputAction moveAction;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        var playerMap = inputActions.FindActionMap("Player", throwIfNotFound: true);
        moveAction = playerMap.FindAction("Move", throwIfNotFound: true);
    }

    private void OnEnable()
    {
        moveAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
    }

    private void Update()
    {
        Vector2 joystickDir = joystick != null ? joystick.Direction : Vector2.zero;
        Vector2 keyboardDir = moveAction.ReadValue<Vector2>();
        moveInput = joystickDir.sqrMagnitude > 0.01f ? joystickDir : keyboardDir;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }
}
