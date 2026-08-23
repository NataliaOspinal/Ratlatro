using UnityEngine;
using UnityEngine.InputSystem;

public class MainPlayer : BaseIsometricPlayer
{
    //Catapulta y compañero
    public GameObject companionPrefab;
    public float pickupRange = 1.5f;
    public float maxThrowDistance = 6f; // Distancia límite en la habitación, ajustable
    public float maxChargeTime = 1.5f;  // Segundos requeridos para máxima potencia de tirada, ajustable

    private GameObject spawnedCompanion;
    private CompanionPlayer companionScript;
    private bool isHoldingCompanion = false;
    private float currentChargeTime = 0f;

    //Progresión de juego
    public bool puedeInvocarCompañero = false;

    protected override Vector2 GetInput()
    {
        Vector2 input = Vector2.zero;

        if (Keyboard.current != null)
        {
            // WASD
            if (Keyboard.current.dKey.isPressed) input.x += 1f;
            if (Keyboard.current.aKey.isPressed) input.x -= 1f;
            if (Keyboard.current.wKey.isPressed) input.y += 1f;
            if (Keyboard.current.sKey.isPressed) input.y -= 1f;

            // Spawn/Despawn (Letra E)
            if (puedeInvocarCompañero && Keyboard.current.eKey.wasPressedThisFrame)
            {
                if (spawnedCompanion == null)
                {
                    Vector3 spawnPos = transform.position + new Vector3(1f, -0.5f, 0f);
                    spawnedCompanion = Instantiate(companionPrefab, spawnPos, Quaternion.identity);
                    companionScript = spawnedCompanion.GetComponent<CompanionPlayer>();
                }
                else if (!isHoldingCompanion)
                {
                    Destroy(spawnedCompanion);
                }
            }

            // Catapulta (Letra R)
            if (spawnedCompanion != null)
            {
                // Agarrar ratita
                if (Keyboard.current.rKey.wasPressedThisFrame && !isHoldingCompanion)
                {
                    float distance = Vector2.Distance(transform.position, spawnedCompanion.transform.position);
                    if (distance <= pickupRange)
                    {
                        isHoldingCompanion = true;
                        currentChargeTime = 0f;
                        companionScript.BePickedUp(transform);
                    }
                }

                // Cargar fuerza
                if (Keyboard.current.rKey.isPressed && isHoldingCompanion)
                {
                    currentChargeTime += Time.deltaTime;
                    currentChargeTime = Mathf.Clamp(currentChargeTime, 0f, maxChargeTime);
                }

                // Disparar ratita
                if (Keyboard.current.rKey.wasReleasedThisFrame && isHoldingCompanion)
                {
                    isHoldingCompanion = false;

                    // Calculamos la distancia basada en qué tanto se mantuvo presionada la tecla R
                    float chargeRatio = currentChargeTime / maxChargeTime;
                    float throwDistance = maxThrowDistance * chargeRatio;
                    throwDistance = Mathf.Max(throwDistance, 1.5f); // Distancia mínima

                    // Calculamos la posición de aterrizaje usando la última dirección a la que caminamos
                    Vector2 targetPos = (Vector2)transform.position + (lastFacingDirection * throwDistance);

                    companionScript.BeThrown(targetPos);
                }
            }
        }

        return input;
    }

    public void DesbloquearCompañero()
    {
        puedeInvocarCompañero = true;
    }
}