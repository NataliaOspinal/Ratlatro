using UnityEngine;
using UnityEngine.InputSystem;

public class MainPlayer : BaseIsometricPlayer
{
    [Header("Catapulta y Compañero")]
    public GameObject companionPrefab;
    public float pickupRange = 1.5f;

    [Header("Configuración de Apuntado")]
    public float maxThrowDistance = 5f;
    public float maxArcHeight = 6f;
    public float minArcHeight = 0.5f;
    public float tiempoCargaDistancia = 1f;
    public float tiempoCargaAltura = 1f;

    [Header("Láser de Apuntado")]
    public LineRenderer laserApuntado; // Arrastra tu nuevo objeto aquí

    private GameObject spawnedCompanion;
    private CompanionPlayer companionScript;
    private bool isHoldingCompanion = false;
    private float currentChargeTime = 0f;
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
                // AGARRAR
                if (Keyboard.current.rKey.wasPressedThisFrame && !isHoldingCompanion)
                {
                    float distance = Vector2.Distance(transform.position, spawnedCompanion.transform.position);
                    if (distance <= pickupRange)
                    {
                        isHoldingCompanion = true;
                        currentChargeTime = 0f;
                        companionScript.BePickedUp(transform);

                        // Encendemos el láser
                        if (laserApuntado != null) laserApuntado.gameObject.SetActive(true);
                    }
                }

                // CARGAR Y DIBUJAR LÁSER
                if (Keyboard.current.rKey.isPressed && isHoldingCompanion)
                {
                    currentChargeTime += Time.deltaTime;

                    Vector2 startPos = transform.position;
                    Vector2 dir = lastFacingDirection.normalized;

                    // Cálculos de distancia máxima permitida por muros
                    float ratioDistancia = Mathf.Clamp01(currentChargeTime / tiempoCargaDistancia);
                    float distanciaTeorica = Mathf.Max(ratioDistancia * maxThrowDistance, 1f);
                    float distanciaReal = distanciaTeorica;

                    // Escáner de seguridad
                    RaycastHit2D[] hits = Physics2D.RaycastAll(startPos, dir, distanciaTeorica);
                    foreach (RaycastHit2D hit in hits)
                    {
                        if (hit.collider != null && hit.collider.CompareTag("Pared"))
                        {
                            if (hit.distance < distanciaReal) distanciaReal = hit.distance - 0.4f;
                        }
                    }

                    Vector2 posicionCodo = startPos + (dir * distanciaReal);

                    // Cálculo de altura
                    float alturaActual = 0f;
                    if (currentChargeTime > tiempoCargaDistancia)
                    {
                        float tiempoSobrante = currentChargeTime - tiempoCargaDistancia;
                        float ratioAltura = Mathf.Clamp01(tiempoSobrante / tiempoCargaAltura);
                        alturaActual = Mathf.Max(ratioAltura * maxArcHeight, minArcHeight);
                    }

                    // Calculamos la punta sumando Y
                    Vector2 posicionPunta = posicionCodo + new Vector2(0f, alturaActual);

                    // Actualizamos los puntos del LineRenderer
                    if (laserApuntado != null)
                    {
                        laserApuntado.SetPosition(0, startPos);       // Origen (Rata)
                        laserApuntado.SetPosition(1, posicionCodo);   // Destino en suelo
                        laserApuntado.SetPosition(2, posicionPunta);  // Altura final
                    }
                }

                // 2. CARGAR Y DIBUJAR LÁSER
                if (Keyboard.current.rKey.isPressed && isHoldingCompanion)
                {
                    currentChargeTime += Time.deltaTime;

                    Vector2 startPos = transform.position;

                    // --- ¡MAGIA ISOMÉTRICA AQUÍ! ---
                    Vector2 rawDir = lastFacingDirection.normalized;
                    // Proyectamos el vector plano (WASD) a los ejes diagonales de tu mapa
                    Vector2 dir = new Vector2(rawDir.x - rawDir.y, (rawDir.x + rawDir.y) * 0.5f).normalized;
                    // ------------------------------

                    // Cálculos de distancia máxima permitida por muros
                    float ratioDistancia = Mathf.Clamp01(currentChargeTime / tiempoCargaDistancia);
                    float distanciaTeorica = Mathf.Max(ratioDistancia * maxThrowDistance, 1f);
                    float distanciaReal = distanciaTeorica;

                    // Escáner de seguridad (Ahora viaja perfecto por el pasillo isométrico)
                    RaycastHit2D[] hits = Physics2D.RaycastAll(startPos, dir, distanciaTeorica);
                    foreach (RaycastHit2D hit in hits)
                    {
                        if (hit.collider != null && hit.collider.CompareTag("Pared"))
                        {
                            if (hit.distance < distanciaReal) distanciaReal = hit.distance - 0.4f;
                        }
                    }

                    Vector2 posicionCodo = startPos + (dir * distanciaReal);

                    // Cálculo de altura
                    float alturaActual = 0f;
                    if (currentChargeTime > tiempoCargaDistancia)
                    {
                        float tiempoSobrante = currentChargeTime - tiempoCargaDistancia;
                        float ratioAltura = Mathf.Clamp01(tiempoSobrante / tiempoCargaAltura);
                        alturaActual = Mathf.Max(ratioAltura * maxArcHeight, minArcHeight);
                    }

                    // Calculamos la punta sumando Y
                    Vector2 posicionPunta = posicionCodo + new Vector2(0f, alturaActual);

                    // Actualizamos los puntos del LineRenderer
                    if (laserApuntado != null)
                    {
                        laserApuntado.SetPosition(0, startPos);       // Origen (Rata)
                        laserApuntado.SetPosition(1, posicionCodo);   // Destino en suelo
                        laserApuntado.SetPosition(2, posicionPunta);  // Altura final
                    }
                }

                // 3. DISPARAR
                if (Keyboard.current.rKey.wasReleasedThisFrame && isHoldingCompanion)
                {
                    isHoldingCompanion = false;

                    if (laserApuntado != null) laserApuntado.gameObject.SetActive(false);

                    float ratioDistancia = Mathf.Clamp01(currentChargeTime / tiempoCargaDistancia);
                    float distanciaCalculada = Mathf.Max(ratioDistancia * maxThrowDistance, 1.5f);

                    float ratioAltura = 0f;
                    if (currentChargeTime > tiempoCargaDistancia)
                    {
                        ratioAltura = Mathf.Clamp01((currentChargeTime - tiempoCargaDistancia) / tiempoCargaAltura);
                    }
                    float alturaCalculada = Mathf.Max(ratioAltura * maxArcHeight, minArcHeight);

                    Vector2 startPos = (Vector2)transform.position;

                    // --- ¡REPETIMOS LA CONVERSIÓN PARA EL TIRO REAL! ---
                    Vector2 rawDir = lastFacingDirection.normalized;
                    Vector2 dir = new Vector2(rawDir.x - rawDir.y, (rawDir.x + rawDir.y) * 0.5f).normalized;
                    // ---------------------------------------------------

                    float distanciaFinal = distanciaCalculada;

                    RaycastHit2D[] hits = Physics2D.RaycastAll(startPos, dir, distanciaCalculada);
                    foreach (RaycastHit2D hit in hits)
                    {
                        if (hit.collider != null && hit.collider.CompareTag("Pared"))
                        {
                            if (hit.distance < distanciaFinal) distanciaFinal = hit.distance - 0.4f;
                        }
                    }

                    Vector2 targetPos = startPos + (dir * Mathf.Max(distanciaFinal, 1.0f));

                    // ¡Fuego en diagonal perfecta!
                    companionScript.BeThrown(targetPos, alturaCalculada);
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