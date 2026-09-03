using System.Collections;
using UnityEngine;

public class TorretaElectrica : MonoBehaviour
{
    // Tiempo que la rata grande permanece electrocutada antes de morir
    public float tiempoElectrocutado = 1.2f;

    private bool trampaActiva = false;

    public void ActivarTrampa()
    {
        trampaActiva = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (trampaActiva && collision.CompareTag("Player"))
        {
            MainPlayer rata = collision.GetComponent<MainPlayer>();

            // Nos aseguramos de que la rata siga viva y moviéndose antes de atraparla
            if (rata != null && rata.canMove)
            {
                trampaActiva = false; // Apagamos la trampa para no ejecutar esto dos veces
                StartCoroutine(SecuenciaElectrocutar(rata));
            }
        }
    }

    private IEnumerator SecuenciaElectrocutar(MainPlayer rata)
    {
        rata.canMove = false;
        rata.ForzarDespawnCompanero();

        Animator rataAnim = rata.GetComponent<Animator>();
        if (rataAnim != null)
        {
            // El motor evalúa DirX y DirY automáticamente al recibir este Trigger
            rataAnim.SetTrigger("RataElectrocutada");
        }

        yield return new WaitForSeconds(tiempoElectrocutado);
        rata.Morir();
    }
}