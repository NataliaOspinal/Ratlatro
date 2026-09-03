using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SalidaZona2 : MonoBehaviour
{
    [Header("Configuración de Destino")]
    public string escenaDestino;
    public string escenaTransicion = "PantallaGuardado";
    
    public int numeroSiguienteZona = 2; 

    private bool enTransicion = false; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !enTransicion)
        {
            MainPlayer jugador = collision.GetComponent<MainPlayer>();

            if (jugador != null)
            {
                enTransicion = true;
                
                jugador.canMove = false; 

                if (!string.IsNullOrEmpty(escenaDestino))
                {
                    StartCoroutine(ViajarAEscena());
                }
            }
        }
    }

    private IEnumerator ViajarAEscena()
    {
        yield return new WaitForSeconds(0.5f);

        PlayerPrefs.SetInt("SiguienteZona", numeroSiguienteZona);

        SceneManager.LoadScene(escenaTransicion);
    }
}
