using UnityEngine;

public class ConductoAereo : MonoBehaviour
{
    [Tooltip("Arrastra aquí tu objeto PuntoAterrizaje")]
    public Transform puntoDeAterrizaje;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CompanionPlayer rata = collision.GetComponent<CompanionPlayer>();

        // Si lo que entró al hueco es la rata pequeña y está volando
        if (rata != null && rata.isFlying)
        {
            // La teletransportamos suavemente al otro lado
            rata.InterrumpirVuelo();
            rata.transform.position = puntoDeAterrizaje.position;
        }
    }
}