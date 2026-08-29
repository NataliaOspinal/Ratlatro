using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ConductoAereo : MonoBehaviour
{
    public Transform puntoDeAterrizaje;

    public string escenaDestino;

    public string escenaTransicion = "PantallaGuardado";
    public int numeroSiguienteZona = 1; // 1 es zona 2, 2 es zona 3

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CompanionPlayer rata = collision.GetComponent<CompanionPlayer>();

        if (rata != null && rata.isFlying)
        {
            rata.AterrizajePerfecto(puntoDeAterrizaje.position);

            if (!string.IsNullOrEmpty(escenaDestino))
            {
                StartCoroutine(ViajarAEscena());
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