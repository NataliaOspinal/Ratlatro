using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class BaseIsometricPlayer : MonoBehaviour
{
    public float moveSpeed = 5f;
    protected Rigidbody2D rb;
    private Vector2 currentMovement;

    // Vectores isométricos de solo lectura
    private readonly Vector2 upRight = new Vector2(1f, 0.5f).normalized;
    private readonly Vector2 downLeft = new Vector2(-1f, -0.5f).normalized;
    private readonly Vector2 upLeft = new Vector2(-1f, 0.5f).normalized;
    private readonly Vector2 downRight = new Vector2(1f, -0.5f).normalized;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Método abstracto para que se adapte a rata mayor y rata menor, cada una con su propio input
    protected abstract Vector2 GetInput();

    protected virtual void Update()
    {
        // Obtenemos el input crudo (X, Y) de la clase hija
        Vector2 rawInput = GetInput();
        Vector2 inputDirection = Vector2.zero;

        // Mapeo isométrico 
        if (rawInput.y > 0) inputDirection += upRight;
        if (rawInput.y < 0) inputDirection += downLeft;
        if (rawInput.x < 0) inputDirection += upLeft;
        if (rawInput.x > 0) inputDirection += downRight;

        currentMovement = inputDirection.normalized;

        // Rotación
        if (currentMovement != Vector2.zero)
        {
            float angle = (Mathf.Atan2(currentMovement.y, currentMovement.x) * Mathf.Rad2Deg) + 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    protected virtual void FixedUpdate()
    {
        rb.MovePosition(rb.position + currentMovement * moveSpeed * Time.fixedDeltaTime);
    }
}