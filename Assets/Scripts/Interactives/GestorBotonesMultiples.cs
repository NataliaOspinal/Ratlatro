using UnityEngine;
using UnityEngine.Events;

public class GestorBotonesMultiples : MonoBehaviour
{
    //Config puzzle
    [Tooltip("Cuántos botones deben estar presionados a la vez para ganar")]
    public int botonesRequeridos = 2;

    private int botonesPresionados = 0;
    private bool puzleResuelto = false;

    //Gestor d eeventos
    public UnityEvent AlCompletarTodos;
    public UnityEvent AlSoltarAlguno;

    // Los botones individuales llamarán a este método al ser pisados
    public void AgregarBoton()
    {
        botonesPresionados++;
        EvaluarEstado();
    }

    // Los botones individuales llamarán a este método al ser soltados
    public void QuitarBoton()
    {
        botonesPresionados--;
        // Evitamos que baje de 0 por seguridad
        if (botonesPresionados < 0) botonesPresionados = 0;
        EvaluarEstado();
    }

    private void EvaluarEstado()
    {
        // Si llegamos a la meta y no estaba resuelto aún
        if (botonesPresionados >= botonesRequeridos && !puzleResuelto)
        {
            puzleResuelto = true;
            AlCompletarTodos.Invoke(); // ¡Abre la pared!
        }
        // Si estábamos resueltos pero alguien se bajó de un botón
        else if (botonesPresionados < botonesRequeridos && puzleResuelto)
        {
            puzleResuelto = false;
            AlSoltarAlguno.Invoke(); // ¡Cierra la pared!
        }
    }
}