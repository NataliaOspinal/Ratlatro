using UnityEngine;
using System.Collections;

public class VentiladorSuelo : MonoBehaviour
{
    private Animator animator;
    public bool estaEncendido = true;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void Detenerse()
    {
        estaEncendido = false;
        if (animator != null) animator.speed = 0f;
    }

    public void Encender()
    {
        estaEncendido = true;
        if (animator != null) animator.speed = 1f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!estaEncendido) return;

        if (collision.CompareTag("Player"))
        {
            MainPlayer rataGrande = collision.GetComponent<MainPlayer>();
            CompanionPlayer rataChiquita = collision.GetComponent<CompanionPlayer>();

            // Si es la rata grande, muere normalmente
            if (rataGrande != null)
            {
                if (animator != null) animator.speed = 0f;
                rataGrande.Morir();
            }
            // Si es la chiquita, sale volando
            else if (rataChiquita != null)
            {
                // Apagamos sus colisiones para que no active el ventilador mil veces
                Collider2D col = collision.GetComponent<Collider2D>();
                if (col != null) col.enabled = false;

                StartCoroutine(VolarRataChiquita(collision.gameObject));
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Se ejecuta constantemente mientras el jugador esté dentro del área
        if (!estaEncendido) return;
        OnTriggerEnter2D(collision);
    }

    private IEnumerator VolarRataChiquita(GameObject rataChiquita)
    {
        // Sintaxis para buscar objetos
        MainPlayer jugadorPrincipal = Object.FindFirstObjectByType<MainPlayer>();
        if (jugadorPrincipal != null)
        {
            jugadorPrincipal.enabled = false;
        }

        // Físicas 2D 
        Rigidbody2D rb = rataChiquita.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; // Reemplazo de rb.velocity
            rb.bodyType = RigidbodyType2D.Kinematic; // Reemplazo de rb.isKinematic = true
        }

        // Vuelo hacia la parte superior de la pantalla
        float velocidadVuelo = 15f;
        float tiempoVuelo = 1.2f;
        float timer = 0f;

        while (timer < tiempoVuelo)
        {
            if (rataChiquita != null)
            {
                rataChiquita.transform.Translate(Vector3.up * velocidadVuelo * Time.deltaTime, Space.World);
            }
            timer += Time.deltaTime;
            yield return null;
        }

        // Reinicio automático de la sala
        if (RoomManager.Instance != null)
        {
            RoomManager.Instance.ResetCurrentRoom();
        }
    }
}