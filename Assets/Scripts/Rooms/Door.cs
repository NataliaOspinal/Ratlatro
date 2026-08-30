using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    public enum PuertaDireccion { Right, Left }
    public PuertaDireccion direccion;

    public bool estaAbierta { get; private set; } = false;

    [Header("Sonidos")]
    public AudioSource fuenteDeAudio;
    public AudioClip sfxAbrir;
    [Header("Animaciones y efectos")]
    public Animator puertaAnimator;
    public string nombreTriggerAnimacion = "Abrir";
    public bool esVentilacion = false;
    public string nombreTriggerCerrar = "Cerrar";
    [Header("Colisiones")]
    public Collider2D colliderViaje; 
    public Collider2D colliderBloqueo;


    private void Start()
    {
        // Comunicamos inmediatamente al Animator el tipo de puerta para que no desaparezca
        if (puertaAnimator != null)
        {
            puertaAnimator.SetBool("esVentilacion", esVentilacion);
        }

        if (colliderViaje != null) colliderViaje.enabled = false;
        if (colliderBloqueo != null) colliderBloqueo.enabled = true;

        if (fuenteDeAudio == null) fuenteDeAudio = GetComponent<AudioSource>();
    }

    public void Cerrar()
    {
        estaAbierta = false;
        if (puertaAnimator != null)
        {
            // Validamos que siga siendo ventilación al reproducir la animación de cierre
            puertaAnimator.SetBool("esVentilacion", esVentilacion);
            puertaAnimator.SetTrigger(nombreTriggerCerrar);
        }

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

        if (fuenteDeAudio != null && sfxAbrir != null)
        {
            fuenteDeAudio.PlayOneShot(sfxAbrir);
        }

        if (colliderViaje != null) colliderViaje.enabled = true;

        if (colliderBloqueo != null) colliderBloqueo.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.GetComponent<MainPlayer>() != null)
        {
            RoomManager.Instance.LoadNextRoom(direccion, collision.gameObject);
        }
    }
}