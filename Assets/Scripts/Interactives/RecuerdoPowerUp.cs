using UnityEngine;

public class RecuerdoPowerUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verificamos si quien lo tocó fue el jugador principal
        if (collision.CompareTag("Player"))
        {
            MainPlayer jugador = collision.GetComponent<MainPlayer>();

            if (jugador != null)
            {
                jugador.DesbloquearCompañero();

                // Maybe instanciar partículas en el futuro
                Debug.Log("¡Poder desbloqueado!");

                Destroy(gameObject);
            }
        }
    }
}