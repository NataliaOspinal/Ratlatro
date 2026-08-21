using UnityEngine;
using UnityEngine.Events;

public class FloorButton : MonoBehaviour
{
    public enum ActivatorType
    {
        Cualquiera,
        SoloRataGrande,
        SoloRataPequena
    }

    // Config visual del botón y quién puede activarlo
    public ActivatorType quienPuedeActivar = ActivatorType.Cualquiera;

    // Eventos que se disparan al presionar y soltar el botón
    public UnityEvent AlPresionar;
    public UnityEvent AlSoltar;

    private Animator animator;

    // Cambiamos el nombre a objetos porque ahora se activa cn ratas y bloques
    private int objectsOnButton = 0;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Revisamos si el objeto que entró tiene permiso de activar el botón
        if (IsValidActivator(other))
        {
            objectsOnButton++;

            // Si es el primer objeto en pisarlo, se activa
            if (objectsOnButton == 1)
            {
                animator.SetBool("IsPressed", true);
                AlPresionar.Invoke(); // Dispara la función lógica
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (IsValidActivator(other))
        {
            objectsOnButton--;

            // Si ya no queda nada encima, se desactiva
            if (objectsOnButton <= 0)
            {
                objectsOnButton = 0;
                animator.SetBool("IsPressed", false);
                AlSoltar.Invoke(); // Dispara la función lógica inversa (ej. Cerrar puerta)
            }
        }
    }

    // Función que decide quién tiene permiso de pisar el botón
    private bool IsValidActivator(Collider2D other)
    {
        // Detecta bloque empujable, que siempre puede activar el botón
        if (other.GetComponent<PushableBlock>() != null)
        {
            return true;
        }

        // Es jugador chi o ño
        if (other.CompareTag("Player"))
        {
            switch (quienPuedeActivar)
            {
                case ActivatorType.Cualquiera:
                    return true;
                case ActivatorType.SoloRataGrande:
                    return other.GetComponent<MainPlayer>() != null;
                case ActivatorType.SoloRataPequena:
                    return other.GetComponent<CompanionPlayer>() != null;
            }
        }

        // Si no es ni bloque ni la rata correcta, lo ignoramos
        return false;
    }
}