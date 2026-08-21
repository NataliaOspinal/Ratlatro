using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    public enum PuertaDireccion {Right, Left}
    public PuertaDireccion direccion;

    [Header("Animación y Efectos")]
    public Animator puertaAnimator; 
    public string nombreTriggerAnimacion = "Abrir"; 
    
    public float duracionTemblor = 0.3f; 
    public float fuerzaTemblor = 0.03f;  

    private bool puertaActivada = false;


    public void Abrir()
    {
        if (puertaAnimator != null)
        {
            puertaAnimator.SetTrigger(nombreTriggerAnimacion);
        }
        
        StartCoroutine(EfectoTemblor());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !puertaActivada)
        {
            puertaActivada = true; 
            RoomManager.Instance.LoadNextRoom(direccion, collision.gameObject);
        }
    }

    private IEnumerator EfectoTemblor()
    {
        Vector3 posicionOriginal = transform.position;
        float tiempo = 0f;
        while (tiempo < duracionTemblor)
        {
            transform.position = posicionOriginal + (Vector3)Random.insideUnitCircle * fuerzaTemblor;
            tiempo += Time.deltaTime;
            yield return null;
        }
        transform.position = posicionOriginal;
    }
}
