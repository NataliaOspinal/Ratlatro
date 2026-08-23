using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CompanionPlayer : BaseIsometricPlayer
{
    private int originalSortingOrder;
    private Coroutine flightCoroutine;
    private Coroutine fallCoroutine;
    public bool isFlying = false;

    private Vector2 savedColliderSize;
    private float currentZHeight = 0f; // Altura virtual actual

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

    public void BePickedUp(Transform mainPlayerTransform)
    {
        canMove = false;
        rb.simulated = false;

        transform.SetParent(mainPlayerTransform);
        transform.localPosition = new Vector3(0f, 0.5f, 0f);
        spriteRenderer.sortingOrder = 10;
    }

    public void BeThrown(Vector2 floorTargetPosition, float alturaDinamica)
    {
        transform.SetParent(null);

        if (flightCoroutine != null) StopCoroutine(flightCoroutine);
        if (fallCoroutine != null) StopCoroutine(fallCoroutine);

        flightCoroutine = StartCoroutine(FlightRoutine(floorTargetPosition, alturaDinamica));
    }

    private IEnumerator FlightRoutine(Vector2 floorTarget, float targetHeight)
    {
        isFlying = true;
        rb.simulated = true;
        col.isTrigger = true;

        savedColliderSize = col.size;
        col.size = new Vector2(0.1f, 0.1f); // Píxel perfecto

        Vector2 startFloorPos = transform.position;
        float distanciaReal = Vector2.Distance(startFloorPos, floorTarget);
        float duration = Mathf.Clamp(distanciaReal * 0.15f, 0.4f, 0.8f);

        float timePassed = 0f;

        while (timePassed < duration)
        {
            timePassed += Time.deltaTime;
            float linearT = timePassed / duration;

            // Movimiento lineal en el piso
            Vector2 currentFloorPos = Vector2.Lerp(startFloorPos, floorTarget, linearT);

            // ¡LA HIPOTENUSA! Movimiento lineal directo hacia arriba, sin curva.
            currentZHeight = Mathf.Lerp(0f, targetHeight, linearT);

            transform.position = currentFloorPos + new Vector2(0f, currentZHeight);
            yield return null;
        }

        // Si llega al final del láser sin chocar con nada ni entrar al hueco:
        InterrumpirVuelo();
    }

    public void InterrumpirVuelo()
    {
        if (flightCoroutine != null) StopCoroutine(flightCoroutine);

        // Inicia la caída libre por gravedad
        if (fallCoroutine != null) StopCoroutine(fallCoroutine);
        fallCoroutine = StartCoroutine(FallRoutine());
    }

    private IEnumerator FallRoutine()
    {
        // Guardamos el punto exacto del piso debajo de la rata
        Vector2 floorPos = (Vector2)transform.position - new Vector2(0f, currentZHeight);

        float fallSpeed = 15f; // Velocidad de la gravedad
        while (currentZHeight > 0f)
        {
            currentZHeight -= fallSpeed * Time.deltaTime;
            if (currentZHeight < 0f) currentZHeight = 0f;

            transform.position = floorPos + new Vector2(0f, currentZHeight);
            yield return null;
        }

        RestaurarFisicas();
    }

    // Nuevo método: El ducto llama a este método si el tiro fue perfecto
    public void AterrizajePerfecto(Vector2 nuevaPosicion)
    {
        if (flightCoroutine != null) StopCoroutine(flightCoroutine);
        if (fallCoroutine != null) StopCoroutine(fallCoroutine);

        transform.position = nuevaPosicion;
        currentZHeight = 0f;

        RestaurarFisicas();
    }

    private void RestaurarFisicas()
    {
        isFlying = false;
        col.isTrigger = false;
        if (savedColliderSize != Vector2.zero) col.size = savedColliderSize;
        canMove = true;
        spriteRenderer.sortingOrder = originalSortingOrder;
    }

    // El radar anti-choques aéreos
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Como el láser del MainPlayer ya revisó el suelo, 
        // ahora la rata es un fantasma para las paredes normales.
        // ¡SOLO vigilamos los marcos altos (MuroAlto) por si fallamos el tiro al ducto!
        if (isFlying && collision.CompareTag("MuroAlto"))
        {
            InterrumpirVuelo();
        }
    }
}