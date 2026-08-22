using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Door : MonoBehaviour
{
    public enum PuertaDireccion {Right, Left}
    public PuertaDireccion direccion;

    [Header("Animación y Efectos")]
    public Animator puertaAnimator; 
    public string nombreTriggerAnimacion = "Abrir"; 

    public bool esVentilacion = false;
    
  
    public Collider2D colliderViaje;

    private void Start()
    {
        if (colliderViaje != null) colliderViaje.enabled = false;
    }

    public void Abrir()
    {
        if (puertaAnimator != null)
        {
            puertaAnimator.SetBool("esVentilacion", esVentilacion);

            puertaAnimator.SetTrigger(nombreTriggerAnimacion);
        }

        if (colliderViaje != null) colliderViaje.enabled = true;
    
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {            
            RoomManager.Instance.LoadNextRoom(direccion, collision.gameObject);
        }
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
        {
            Abrir();
        }
    }

}
