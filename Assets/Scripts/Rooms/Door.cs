using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    public enum PuertaDireccion {Right, Left}
    public PuertaDireccion direccion;

    [Header("Animación y Efectos")]
    public Animator puertaAnimator; 
    public string nombreTriggerAnimacion = "Abrir"; 
    
  


    public void Abrir()
    {
        if (puertaAnimator != null)
        {
            puertaAnimator.SetTrigger(nombreTriggerAnimacion);
        }
        
    
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {            
            RoomManager.Instance.LoadNextRoom(direccion, collision.gameObject);
        }
    }

}
