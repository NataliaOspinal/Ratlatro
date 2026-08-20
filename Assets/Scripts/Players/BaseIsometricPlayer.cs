using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class BaseIsometricPlayer : MonoBehaviour
{
    public float moveSpeed = 5f;
    protected Rigidbody2D rb;
    private Vector2 currentMovement;
    private SpriteRenderer spriteRenderer;

    // Vectores isométricos de solo lectura
    private readonly Vector2 upRight = new Vector2(1f, 0.5f).normalized;
    private readonly Vector2 downLeft = new Vector2(-1f, -0.5f).normalized;
    private readonly Vector2 upLeft = new Vector2(-1f, 0.5f).normalized;
    private readonly Vector2 downRight = new Vector2(1f, -0.5f).normalized;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
            if (currentMovement != Vector2.zero)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);

                if (currentMovement.x < 0)
                {
                    spriteRenderer.flipX = true;
                }
                else if (currentMovement.x > 0)
                {
                    spriteRenderer.flipX = false;
                }
            }
        }
    }

    protected virtual void FixedUpdate()
    {
        rb.MovePosition(rb.position + currentMovement * moveSpeed * Time.fixedDeltaTime);
    }
}