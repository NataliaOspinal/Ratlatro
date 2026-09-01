using UnityEngine;

public class SensorFila : MonoBehaviour
{
    public GestorGrillaVentiladores gestor;
    public int indiceDeFila; // 0 para la primera fila, 1 para la segunda, etc.

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            gestor.ActivarFila(indiceDeFila);
        }
    }
}