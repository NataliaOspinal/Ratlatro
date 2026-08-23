using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CompanionPlayer : BaseIsometricPlayer
{
    private int originalSortingOrder;
    private Coroutine flightCoroutine;
    public bool isFlying = false;

    protected override void Start()
    {
        base.Start();
        originalSortingOrder = spriteRenderer.sortingOrder;
    }

    protected override Vector2 GetInput()
    {
        Vector2 input = Vector2.zero;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.rightArrowKey.isPressed) input.x += 1f;
            if (Keyboard.current.leftArrowKey.isPressed) input.x -= 1f;
            if (Keyboard.current.upArrowKey.isPressed) input.y += 1f;
            if (Keyboard.current.downArrowKey.isPressed) input.y -= 1f;
        }
        return input;
    }

    // M�todo llamado cuando el MainPlayer presiona R
    public void BePickedUp(Transform mainPlayerTransform)
    {
        canMove = false;
        rb.simulated = false;

        transform.SetParent(mainPlayerTransform);
        transform.localPosition = new Vector3(0f, 0.5f, 0f);
        spriteRenderer.sortingOrder = 10;
    }

    // M�todo llamado cuando el MainPlayer suelta la R


    

    public void BeThrown(Vector2 targetPosition)
    {
        transform.SetParent(null);

        if (flightCoroutine != null) StopCoroutine(flightCoroutine);
        flightCoroutine = StartCoroutine(FlightRoutine(targetPosition));
    }

    private IEnumerator FlightRoutine(Vector2 targetPosition)
    {
        isFlying = true;
        rb.simulated = true;
        col.isTrigger = true; // Se vuelve fantasma para el vuelo

        Vector2 startPosition = transform.position;
        float duration = 0.5f;
        float timePassed = 0f;
        float arcHeight = 5f; // Tu altura perfecta

        while (timePassed < duration)
        {
            timePassed += Time.deltaTime;
            float linearT = timePassed / duration;

            Vector2 currentPos = Vector2.Lerp(startPosition, targetPosition, linearT);
            float heightOffset = Mathf.Sin(linearT * Mathf.PI) * arcHeight;
            currentPos.y += heightOffset;

            transform.position = currentPos;
            yield return null;
        }

        // Aterrizaje natural si no chocó con nada
        transform.position = targetPosition;
        InterrumpirVuelo();
    }

    // Este método lo llamará el Hueco o la propia rata al chocar
    public void InterrumpirVuelo()
    {
        if (flightCoroutine != null) StopCoroutine(flightCoroutine);

        isFlying = false;
        col.isTrigger = false; // Vuelve a ser sólida
        canMove = true;
        spriteRenderer.sortingOrder = originalSortingOrder;
    }

    // El radar anti-choques aéreos
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si está volando y toca el marco de la pared...
        if (isFlying && collision.CompareTag("MuroAlto"))
        {
            // ¡Pum! Chocó contra la pared. Cae al suelo inmediatamente.
            InterrumpirVuelo();
        }
    }

    private void TerminarVuelo()
    {
        isFlying = false;
        col.isTrigger = false;
        canMove = true;
        spriteRenderer.sortingOrder = originalSortingOrder;
    }
}