using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CompanionPlayer : BaseIsometricPlayer
{
    private int originalSortingOrder;

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

    // Método llamado cuando el MainPlayer presiona R
    public void BePickedUp(Transform mainPlayerTransform)
    {
        canMove = false;
        rb.simulated = false; // Desactiva físicas (para no empujar al jugador)

        // Emparentar y posicionar sobre la cabeza
        transform.SetParent(mainPlayerTransform);
        transform.localPosition = new Vector3(0f, 0.5f, 0f); // Ajusta la Y para que encaje visualmente
        spriteRenderer.sortingOrder = 10; // Dibujarse por delante del main player
    }

    // Método llamado cuando el MainPlayer suelta la R
    public void BeThrown(Vector2 targetPosition)
    {
        transform.SetParent(null); // Desemparentar
        StartCoroutine(FlightRoutine(targetPosition));
    }

    private IEnumerator FlightRoutine(Vector2 targetPosition)
    {
        Vector2 startPosition = transform.position;
        float duration = 0.5f; // Segundos que tarda en aterrizar
        float timePassed = 0f;
        float arcHeight = 2f; // Altura visual de la parábola de la catapulta

        while (timePassed < duration)
        {
            timePassed += Time.deltaTime;
            float linearT = timePassed / duration; // Progresión de 0 a 1

            // Movimiento plano (Lerp entre inicio y fin)
            Vector2 currentPos = Vector2.Lerp(startPosition, targetPosition, linearT);

            // Elevación parabólica simulada
            float heightOffset = Mathf.Sin(linearT * Mathf.PI) * arcHeight;
            currentPos.y += heightOffset;

            transform.position = currentPos;
            yield return null;
        }

        // Aterrizaje
        transform.position = targetPosition;
        rb.simulated = true; // Reactiva físicas
        canMove = true;
        spriteRenderer.sortingOrder = originalSortingOrder;
    }
}