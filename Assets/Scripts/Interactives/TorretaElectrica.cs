using System.Collections;
using UnityEngine;

public class TorretaElectrica : MonoBehaviour
{
    [Tooltip("Tiempo en segundos que dura la animación de electrocución antes de reiniciar")]
    public float tiempoElectrocutado = 1.2f;

    [Header("Muerte por Proximidad")]
    [Tooltip("Radio invisible alrededor de la torre que mata instantáneamente")]
    public float radioMortal = 1.0f;

    private bool trampaActiva = false;

    public void ActivarTrampa()
    {
        trampaActiva = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            MainPlayer rata = collision.GetComponent<MainPlayer>();

            if (rata != null && rata.canMove)
            {
                // Medimos la distancia exacta entre el centro de la torre y la rata
                float distanciaActual = Vector2.Distance(transform.position, rata.transform.position);

                // La rata muere si falló el puzzle O si invadió el espacio personal de la torre
                if (trampaActiva || distanciaActual <= radioMortal)
                {
                    trampaActiva = false; // Apagamos la trampa para no repetir el castigo dos veces
                    StartCoroutine(SecuenciaElectrocutar(rata));
                }
            }
        }
    }

    private IEnumerator SecuenciaElectrocutar(MainPlayer rata)
    {
        // Bloqueamos el movimiento y limpiamos a la rata pequeña
        rata.canMove = false;
        rata.ForzarDespawnCompanero();

        // Disparamos la animación
        Animator rataAnim = rata.GetComponent<Animator>();
        if (rataAnim != null)
        {
            rataAnim.SetTrigger("RataElectrocutada");
        }

        // Esperamos a que la rata se electrocute rip
        yield return new WaitForSeconds(tiempoElectrocutado);

        // Le devolvemos el movimiento justo antes para que la función Morir() no sea abortada
        rata.canMove = true;

        // Reiniciamos el nivel
        rata.Morir();
    }
}