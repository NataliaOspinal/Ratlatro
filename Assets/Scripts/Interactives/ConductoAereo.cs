using UnityEngine;

public class ConductoAereo : MonoBehaviour
{
    public Transform puntoDeAterrizaje;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CompanionPlayer rata = collision.GetComponent<CompanionPlayer>();

        if (rata != null && rata.isFlying)
        {
            // Detiene el vuelo, cancela la gravedad y la teletransporta suavemente
            rata.AterrizajePerfecto(puntoDeAterrizaje.position);
        }
    }
}