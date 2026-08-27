using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    public enum PuertaDireccion { Right, Left }
    public PuertaDireccion direccion;

    public bool estaAbierta { get; private set; } = false;



    //Animación y efectos
    public Animator puertaAnimator;
    public string nombreTriggerAnimacion = "Abrir";
    public bool esVentilacion = false;
    public string nombreTriggerCerrar = "Cerrar";

    public Collider2D colliderViaje; 
    public Collider2D colliderBloqueo; 

    private void Start()
    {
        if (colliderViaje != null) colliderViaje.enabled = false;
        if (colliderBloqueo != null) colliderBloqueo.enabled = true;
    }

    public void Abrir()
    {
        estaAbierta=true;

        if (puertaAnimator != null)
        {
            puertaAnimator.SetBool("esVentilacion", esVentilacion);
            puertaAnimator.SetTrigger(nombreTriggerAnimacion);
        }

        if (colliderViaje != null) colliderViaje.enabled = true;

        if (colliderBloqueo != null) colliderBloqueo.enabled = false;
    }

    public void Cerrar()
    {
        estaAbierta=false;
        if (puertaAnimator != null)
        {
            puertaAnimator.SetTrigger(nombreTriggerCerrar);
        }

        if (colliderViaje != null) colliderViaje.enabled = false;

        if (colliderBloqueo != null) colliderBloqueo.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.GetComponent<MainPlayer>() != null)
        {
            RoomManager.Instance.LoadNextRoom(direccion, collision.gameObject);
        }
    }
}