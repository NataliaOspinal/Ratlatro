using UnityEngine;

public class FloorButton : MonoBehaviour
{
    // Creamos una lista desplegable para el Inspector
    public enum ActivatorType
    {
        Cualquiera,
        SoloRataGrande,
        SoloRataPequeña
    }

    [Tooltip("Define qué personaje puede activar este botón")]
    public ActivatorType quienPuedeActivar = ActivatorType.Cualquiera;

    private Animator animator;
    private int ratsOnButton = 0;

    void Start()
    {
        // Busca el Animator
        animator = GetComponentInChildren<Animator>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (CanActivate(other))
        {
            ratsOnButton++;

            if (ratsOnButton > 0)
            {
                animator.SetBool("IsPressed", true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (CanActivate(other))
        {
            ratsOnButton--;

            if (ratsOnButton <= 0)
            {
                ratsOnButton = 0;
                animator.SetBool("IsPressed", false);
            }
        }
    }

    // Función auxiliar que decide si el objeto que pisó el botón tiene permiso
    private bool CanActivate(Collider2D other)
    {
        // Solo hace caso si tiene tag de player
        if (!other.CompareTag("Player")) return false;

        // Comprobamos la configuración del botón en el Inspector
        switch (quienPuedeActivar)
        {
            case ActivatorType.Cualquiera:
                return true;

            case ActivatorType.SoloRataGrande:
                // Retorna true solo si el objeto tiene el script de la rata grande
                return other.GetComponent<MainPlayer>() != null;

            case ActivatorType.SoloRataPequeña:
                // Retorna true solo si el objeto tiene el script de la rata pequeña
                return other.GetComponent<CompanionPlayer>() != null;

            default:
                return false;
        }
    }
}