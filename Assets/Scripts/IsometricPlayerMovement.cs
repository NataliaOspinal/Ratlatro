using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class IsometricPlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 currentMovement;

    // Direcciones base de la grilla (ayuda)
    private Vector2 upRight = new Vector2(1f, 0.5f).normalized;
    private Vector2 downLeft = new Vector2(-1f, -0.5f).normalized;
    private Vector2 upLeft = new Vector2(-1f, 0.5f).normalized;
    private Vector2 downRight = new Vector2(1f, -0.5f).normalized;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float xInput = 0f;
        float yInput = 0f;

        // Lectura directa del New Input System
        if (Keyboard.current != null)
        {
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) xInput += 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) xInput -= 1f;
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) yInput += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) yInput -= 1f;
        }

        Vector2 inputDirection = Vector2.zero;

        // Mapeo a las diagonales isométricas
        if (yInput > 0) inputDirection += upRight;
        if (yInput < 0) inputDirection += downLeft;
        if (xInput < 0) inputDirection += upLeft;
        if (xInput > 0) inputDirection += downRight;

        currentMovement = inputDirection.normalized;

        // Rotación ratinha
        if (currentMovement != Vector2.zero)
        {
            float angle = (Mathf.Atan2(currentMovement.y, currentMovement.x) * Mathf.Rad2Deg) + 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + currentMovement * moveSpeed * Time.fixedDeltaTime);
    }
}