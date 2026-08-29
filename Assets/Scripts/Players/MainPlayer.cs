using UnityEngine;
using UnityEngine.InputSystem;

public class MainPlayer : BaseIsometricPlayer
{
    //Catapulta de compañero
    public GameObject companionPrefab;
    public float pickupRange = 1.5f;

    // Parámetros de lanzamiento
    public float maxThrowDistance = 5f;
    public float maxArcHeight = 6f;
    public float minArcHeight = 0.5f;
    public float tiempoCargaDistancia = 1f;
    public float tiempoCargaAltura = 1f;

    // Referencia al LineRenderer para el láser de apuntado
    public LineRenderer laserApuntado;

    // Estado del compañero
    private GameObject spawnedCompanion;
    private CompanionPlayer companionScript;
    private bool isHoldingCompanion = false;
    private float currentChargeTime = 0f;
    public bool puedeInvocarCompanero = false;

    // Bandera para evitar moverse o disparar mientras invoca
    private bool estaInvocando = false;

    // Rastro manchitas
    public GameObject manchaPrefab;
    public float tiempoEntreManchas = 0.3f;
    private float temporizadorMancha = 0f;

    protected override void Update()
    {
        base.Update(); // Mantiene intacta la caminata y los idles especiales

        // Si tenemos asignado el prefab y podemos movernos
        if (manchaPrefab != null && canMove && !estaInvocando)
        {
            // Usamos GetInput() para saber si el jugador está presionando las teclas
            Vector2 input = GetInput();

            if (input != Vector2.zero)
            {
                temporizadorMancha += Time.deltaTime;
                if (temporizadorMancha >= tiempoEntreManchas)
                {
                    // Instanciamos la mancha a la altura de los pies
                    Vector3 posicionPies = transform.position + new Vector3(0f, -0.3f, 0f);
                    Instantiate(manchaPrefab, posicionPies, Quaternion.identity);
                    temporizadorMancha = 0f;
                }
            }
            else
            {
                // Reseteamos al tope para que la primera mancha salga casi al instante al reanudar el paso
                temporizadorMancha = tiempoEntreManchas;
            }
        }
    }
    protected override Vector2 GetInput()
    {
        Vector2 input = Vector2.zero;

        // Si está a mitad de una invocación, anulamos todos los controles
        if (estaInvocando) return input;

        if (Keyboard.current != null)
        {
            // WASD
            if (Keyboard.current.dKey.isPressed) input.x += 1f;
            if (Keyboard.current.aKey.isPressed) input.x -= 1f;
            if (Keyboard.current.wKey.isPressed) input.y += 1f;
            if (Keyboard.current.sKey.isPressed) input.y -= 1f;

            // Spawn/Despawn (Letra E)
            if (puedeInvocarCompanero && Keyboard.current.eKey.wasPressedThisFrame)
            {
                if (spawnedCompanion == null && !estaInvocando)
                {
                    estaInvocando = true;
                    canMove = false; // Congelamos a la rata grande

                    // Disparamos la animación
                    if (animator != null) animator.SetTrigger("Invocar");
                }
                else if (!isHoldingCompanion && spawnedCompanion != null && !estaInvocando)
                {
                    Destroy(spawnedCompanion);
                }
            }

            // Catapulta (Letra R)
            if (spawnedCompanion != null)
            {
                // Agarrar rata (presionar R)
                if (Keyboard.current.rKey.wasPressedThisFrame && !isHoldingCompanion)
                {
                    float distance = Vector2.Distance(transform.position, spawnedCompanion.transform.position);
                    if (distance <= pickupRange)
                    {
                        isHoldingCompanion = true;
                        currentChargeTime = 0f;
                        companionScript.BePickedUp(transform);

                        if (laserApuntado != null) laserApuntado.gameObject.SetActive(true);
                    }
                }

                // Carga de lanzamiento (mantener R)
                if (Keyboard.current.rKey.isPressed && isHoldingCompanion)
                {
                    currentChargeTime += Time.deltaTime;

                    Vector2 startPos = transform.position;
                    Vector2 dir = lastFacingDirection.normalized;

                    float ratioDistancia = Mathf.Clamp01(currentChargeTime / tiempoCargaDistancia);
                    float distanciaTeorica = Mathf.Max(ratioDistancia * maxThrowDistance, 1f);
                    float distanciaReal = distanciaTeorica;

                    RaycastHit2D[] hits = Physics2D.RaycastAll(startPos, dir, distanciaTeorica);
                    foreach (RaycastHit2D hit in hits)
                    {
                        if (hit.collider != null && hit.collider.CompareTag("Pared"))
                        {
                            if (hit.distance < distanciaReal) distanciaReal = hit.distance - 0.4f;
                        }
                    }

                    Vector2 posicionCodo = startPos + (dir * distanciaReal);

                    float alturaActual = 0f;
                    if (currentChargeTime > tiempoCargaDistancia)
                    {
                        float tiempoSobrante = currentChargeTime - tiempoCargaDistancia;
                        float ratioAltura = Mathf.Clamp01(tiempoSobrante / tiempoCargaAltura);
                        alturaActual = Mathf.Max(ratioAltura * maxArcHeight, minArcHeight);
                    }

                    Vector2 posicionPunta = posicionCodo + new Vector2(0f, alturaActual);

                    if (laserApuntado != null)
                    {
                        laserApuntado.SetPosition(0, startPos);
                        laserApuntado.SetPosition(1, posicionCodo);
                        laserApuntado.SetPosition(2, posicionPunta);
                    }
                }

                // Tirar rata (soltar R)
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
                    Vector2 dir = lastFacingDirection.normalized;
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
                    companionScript.BeThrown(targetPos, alturaCalculada);
                }
            }
        }

        return input;
    }

    public void DesbloquearCompanero()
    {
        puedeInvocarCompanero = true;
    }

    // Animación de invocación finalizada, llamada desde un evento de animación
    public void TerminarInvocacion()
    {
        estaInvocando = false;
        canMove = true; // Descongelamos a la rata grande

        // La instanciamos al costado izquierdo
        Vector3 spawnPos = transform.position + new Vector3(-1.5f, 0f, 0f);
        spawnedCompanion = Instantiate(companionPrefab, spawnPos, Quaternion.identity);
        companionScript = spawnedCompanion.GetComponent<CompanionPlayer>();
    }
    
    // Llamado externamente por el RoomManager para limpiar la sala
    public void ForzarDespawnCompanero()
    {
        if (spawnedCompanion != null)
        {
            Destroy(spawnedCompanion);
            isHoldingCompanion = false;

            if (laserApuntado != null)
            {
                laserApuntado.gameObject.SetActive(false);
            }
        }
    }
}