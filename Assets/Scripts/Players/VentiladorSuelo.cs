using UnityEngine;

public class VentiladorSuelo : MonoBehaviour
{
    private Animator animator;

    public bool estaEncendido = true;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    // Llamado externamente para hacer la zona segura
    public void Detenerse()
    {
        estaEncendido = false; // La trampa ya no es letal yippie
        if (animator != null)
        {
            animator.speed = 0f; // Congela visualmente las aspas
        }
    }

    // Llamado externamente por si el jugador desactiva el botón y la trampa revive
    public void Encender()
    {
        estaEncendido = true; // Vuelve a ser letal
        if (animator != null)
        {
            animator.speed = 1f; // Reanuda la animación
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // El ventilador solo ataca si está encendido
        if (estaEncendido && collision.CompareTag("Player"))
        {
            // Trabamos las aspas al chocar con la rata idk si dejar esto la vd
            if (animator != null) animator.speed = 0f;

            MainPlayer jugador = collision.GetComponent<MainPlayer>();
            if (jugador != null)
            {
                jugador.Morir();
            }
        }
    }
}