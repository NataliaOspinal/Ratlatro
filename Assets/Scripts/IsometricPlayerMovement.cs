using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class IsometricPlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movementInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector2 input = Vector2.zero;

        // Verificamos si hay un teclado conectado y leemos WASD
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) input.y += 1;
            if (Keyboard.current.sKey.isPressed) input.y -= 1;
            if (Keyboard.current.dKey.isPressed) input.x += 1;
            if (Keyboard.current.aKey.isPressed) input.x -= 1;
        }

        // Normalizamos para que no se mueva más rápido en diagonal
        movementInput = input.normalized;
    }

    void FixedUpdate()
    {
        // Conversión isométrica
        Vector2 isometricMovement = new Vector2(
            movementInput.x - movementInput.y,
            (movementInput.x + movementInput.y) * 0.5f
        );

        rb.MovePosition(rb.position + isometricMovement * moveSpeed * Time.fixedDeltaTime);
    }
}