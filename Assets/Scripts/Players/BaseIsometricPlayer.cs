using UnityEngine;

// Estructura para agrupar los datos de cada dirección
[System.Serializable]
public struct DirectionData
{
    public Sprite sprite;
    public Vector2 colliderSize;
    public Vector2 colliderOffset;
    public CapsuleDirection2D colliderDirection;
}

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(CapsuleCollider2D))]
public abstract class BaseIsometricPlayer : MonoBehaviour
{
    public float moveSpeed = 5f;
    protected Rigidbody2D rb;
    private Vector2 currentMovement;
    private SpriteRenderer spriteRenderer;
    private CapsuleCollider2D col; // Referencia al colisionador

    // 0: Abajo-Izq, 1: Abajo-Der, 2: Arriba-Der, 3: Arriba-Izq
    public DirectionData[] directionData = new DirectionData[4];

    private readonly Vector2 upRight = new Vector2(1f, 0.5f).normalized;
    private readonly Vector2 downLeft = new Vector2(-1f, -0.5f).normalized;
    private readonly Vector2 upLeft = new Vector2(-1f, 0.5f).normalized;
    private readonly Vector2 downRight = new Vector2(1f, -0.5f).normalized;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<CapsuleCollider2D>();
    }

    protected abstract Vector2 GetInput();

    protected virtual void Update()
    {
        Vector2 rawInput = GetInput();
        Vector2 inputDirection = Vector2.zero;

        if (rawInput.y > 0) inputDirection += upRight;
        if (rawInput.y < 0) inputDirection += downLeft;
        if (rawInput.x < 0) inputDirection += upLeft;
        if (rawInput.x > 0) inputDirection += downRight;

        currentMovement = inputDirection.normalized;

        if (currentMovement != Vector2.zero)
        {
            Vector2[] baseDirections = { downLeft, downRight, upRight, upLeft };
            int bestIndex = 0;
            float maxDot = -1f;

            for (int i = 0; i < 4; i++)
            {
                float dot = Vector2.Dot(currentMovement, baseDirections[i]);
                if (dot > maxDot)
                {
                    maxDot = dot;
                    bestIndex = i;
                }
            }

            // Aplicamos los datos de la estructura según la dirección para el collision
            spriteRenderer.sprite = directionData[bestIndex].sprite;
            spriteRenderer.flipX = false;

            col.size = directionData[bestIndex].colliderSize;
            col.offset = directionData[bestIndex].colliderOffset;
            col.direction = directionData[bestIndex].colliderDirection;

            // Rotación compensatoria
            if (maxDot < 0.99f)
            {
                float currentAngle = Mathf.Atan2(currentMovement.y, currentMovement.x) * Mathf.Rad2Deg;
                float baseAngle = Mathf.Atan2(baseDirections[bestIndex].y, baseDirections[bestIndex].x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, Mathf.DeltaAngle(baseAngle, currentAngle));
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }

    protected virtual void FixedUpdate()
    {
        rb.MovePosition(rb.position + currentMovement * moveSpeed * Time.fixedDeltaTime);
    }
}