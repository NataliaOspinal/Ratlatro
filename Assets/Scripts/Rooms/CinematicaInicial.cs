using System.Collections;
using UnityEngine;

public class CinematicaInicial : MonoBehaviour
{
    [Header("Referencias")]
    public Animator mesaAnimator;
    public Transform mascaraCirculo;
    public GameObject pantallaNegra;
    

    [Header("Posicionamiento")]
    public Transform puntoDeSpawnMesa;
   
    [Header("Configuración de Tiempos")]
    public float tiempoSilencioInicial=1f;
    public float duracionTemblor = 1.5f;
    public float fuerzaTemblor = 0.05f;
    public float tiempoEsperaParaAbrir = 1f;
    public float velocidadApertura = 3f;

    private GameObject jugador;

    private void Start()
    {
        jugador = GameObject.Find("Rata");

       if (jugador != null && puntoDeSpawnMesa != null)
        {
            
            jugador.transform.position = new Vector3(
                puntoDeSpawnMesa.position.x, 
                puntoDeSpawnMesa.position.y, 
                jugador.transform.position.z
            );
        }

        // Desactivar el script de movimiento de la rata
        StartCoroutine(EjecutarCinematica());
    }

    private IEnumerator EjecutarCinematica()
    {
        yield return new WaitForSeconds(tiempoSilencioInicial);

        Vector3 posicionOriginalMesa = mesaAnimator.transform.position;
        float tiempo = 0f;
        while (tiempo < duracionTemblor)
        {
            mesaAnimator.transform.position = posicionOriginalMesa + (Vector3)Random.insideUnitCircle * fuerzaTemblor;
            tiempo += Time.deltaTime;
            yield return null;
        }
        mesaAnimator.transform.position = posicionOriginalMesa;

        if (mesaAnimator != null)
        {
            mesaAnimator.SetTrigger("Romper");
        }
        
        yield return new WaitForSeconds(tiempoEsperaParaAbrir);

        Vector3 escalaGigante = new Vector3(80f, 80f, 1f);
        while (mascaraCirculo.localScale.x < 79f)
        {
            mascaraCirculo.localScale = Vector3.Lerp(mascaraCirculo.localScale, escalaGigante, velocidadApertura * Time.deltaTime);
            yield return null;
        }

        Destroy(mascaraCirculo.gameObject);
        Destroy(pantallaNegra);
        
        //Activar el movimiento de la rata

    }
}
