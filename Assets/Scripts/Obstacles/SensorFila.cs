using UnityEngine;

public class SensorFila : MonoBehaviour
{
    public GestorGrillaVentiladores gestor;
    public int indiceDeFila;
    public bool esSensorFinal = false; // Casilla para el último sensor

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (esSensorFinal)
            {
                gestor.DesactivarPuzzle();
            }
            else
            {
                gestor.ActivarFila(indiceDeFila);
            }
        }
    }
}