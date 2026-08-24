using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ConductoAereo : MonoBehaviour
{
    public Transform puntoDeAterrizaje;

    //Nombre de escena destino (si existe) para teletransportar al jugador a otra escena
    public string escenaDestino;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CompanionPlayer rata = collision.GetComponent<CompanionPlayer>();

        if (rata != null && rata.isFlying)
        {
            // Detiene el vuelo, cancela la gravedad y la teletransporta suavemente
            rata.AterrizajePerfecto(puntoDeAterrizaje.position);

            // Si le indicamos un nombre de escena en el Inspector, iniciamos el viaje
            if (!string.IsNullOrEmpty(escenaDestino))
            {
                StartCoroutine(ViajarAEscena());
            }
        }
    }

    private IEnumerator ViajarAEscena()
    {
        // Un pequeñísimo retraso (0.5 segundos) para que el jugador alcance a ver a la rata entrando al ducto
        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(escenaDestino);
    }
}