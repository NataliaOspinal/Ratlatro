using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    public enum PuertaDireccion { Right, Left }
    public PuertaDireccion direccion;

    //Animación y efectos
    public Animator puertaAnimator;
    public string nombreTriggerAnimacion = "Abrir";
    public bool esVentilacion = false;
    public string nombreTriggerCerrar = "Cerrar";

    //Fisicas de la puerta
    public Collider2D colliderViaje; // El portal trigger
    public Collider2D colliderBloqueo; // La pared física

    private void Start()
    {
        // Al iniciar, el portal está apagado y el bloqueo físico está encendido
        if (colliderViaje != null) colliderViaje.enabled = false;
        if (colliderBloqueo != null) colliderBloqueo.enabled = true;
    }

    public void Abrir()
    {
        if (puertaAnimator != null)
        {
            puertaAnimator.SetBool("esVentilacion", esVentilacion);
            puertaAnimator.SetTrigger(nombreTriggerAnimacion);
        }

        // Encendemos el portal para cambiar de sala...
        if (colliderViaje != null) colliderViaje.enabled = true;

        // Apagamos el 
        if (colliderBloqueo != null) colliderBloqueo.enabled = false;
    }

    public void Cerrar()
    {
        if (puertaAnimator != null)
        {
            puertaAnimator.SetTrigger(nombreTriggerCerrar);
        }

        // Apagamos el portal de viaje para que no puedan salir
        if (colliderViaje != null) colliderViaje.enabled = false;

        // Y volvemos a activar el bloqueo
        if (colliderBloqueo != null) colliderBloqueo.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            RoomManager.Instance.LoadNextRoom(direccion, collision.gameObject);
        }
    }
}