using UnityEngine;

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
    public bool canMove = true;

    //Opcional animator para manejar animaciones en lugar de cambiar sprites manualmente
    public Animator animator;

    //Idle especial random, se activa si el jugador no se mueve durante un tiempo determinado
    public float tiempoParaIdleEspecial = 5f; // Tiempo en segundos
    private float temporizadorIdle = 0f;

    protected Rigidbody2D rb;
    protected SpriteRenderer spriteRenderer;
    protected CapsuleCollider2D col;
    protected Vector2 lastFacingDirection = new Vector2(1, -0.5f).normalized;

    private Vector2 currentMovement;
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
        Vector2 rawInput = canMove ? GetInput() : Vector2.zero;

        if (Mathf.Abs(rawInput.x) > 0 && Mathf.Abs(rawInput.y) > 0)
        {
            rawInput.y = 0f;
        }

        Vector2 inputDirection = Vector2.zero;

        if (rawInput.y > 0) inputDirection += upRight;
        if (rawInput.y < 0) inputDirection += downLeft;
        if (rawInput.x < 0) inputDirection += upLeft;
        if (rawInput.x > 0) inputDirection += downRight;

        currentMovement = inputDirection.normalized;

        if (currentMovement != Vector2.zero)
        {
            //¡SE ESTÁ MOVIENDO! Reiniciamos el reloj.
            temporizadorIdle = 0f;
            lastFacingDirection = currentMovement;

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

            if (animator != null)
            {
                float dirX = 0f;
                float dirY = 0f;

                if (bestIndex == 0) { dirX = -1f; dirY = -1f; }
                else if (bestIndex == 1) { dirX = 1f; dirY = -1f; }
                else if (bestIndex == 2) { dirX = 1f; dirY = 1f; }
                else if (bestIndex == 3) { dirX = -1f; dirY = 1f; }

                animator.SetFloat("DirX", dirX);
                animator.SetFloat("DirY", dirY);
            }
            else
            {
                if (directionData.Length > bestIndex)
                {
                    spriteRenderer.sprite = directionData[bestIndex].sprite;
                    spriteRenderer.flipX = false;
                    col.size = directionData[bestIndex].colliderSize;
                    col.offset = directionData[bestIndex].colliderOffset;
                    col.direction = directionData[bestIndex].colliderDirection;
                }

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
        else
        {
            // NO SE ESTÁ MOVIENDO
            if (animator != null && canMove)
            {
                temporizadorIdle += Time.deltaTime;

                if (temporizadorIdle >= tiempoParaIdleEspecial)
                {
                    int animacionRandom = Random.Range(0, 2);

                    // ¡NUESTRA LUPA! Esto imprimirá un mensaje en la consola de Unity
                    Debug.Log("¡Tiempo cumplido! Lanzando Idle Especial ID: " + animacionRandom);

                    animator.SetInteger("IdleID", animacionRandom);
                    animator.SetTrigger("IdleEspecial");

                    temporizadorIdle = 0f;
                }
            }
        }

        // ENVIAR VELOCIDAD 
        if (animator != null)
        {
            animator.SetFloat("Velocidad", canMove ? currentMovement.magnitude : 0f);
        }
    }

    protected virtual void FixedUpdate()
    {
        if (canMove)
        {
            rb.MovePosition(rb.position + currentMovement * moveSpeed * Time.fixedDeltaTime);
        }
    }
}