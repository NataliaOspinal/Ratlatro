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
        transform.SetParent(null); // Desemparentar
        StartCoroutine(FlightRoutine(targetPosition));
    }

    private IEnumerator FlightRoutine(Vector2 targetPosition)
    {
        rb.simulated = true; 
        col.isTrigger = true;

        Vector2 startPosition = transform.position;
        float duration = 0.5f;
        float timePassed = 0f;
        float arcHeight = 2f;

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

        // Aterrizaje
        transform.position = targetPosition;
        col.isTrigger = false; 

        canMove = true;
        spriteRenderer.sortingOrder = originalSortingOrder;
    }
}