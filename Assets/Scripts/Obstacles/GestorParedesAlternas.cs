using System.Collections.Generic;
using UnityEngine;

public class GestorParedesAlternas : MonoBehaviour
{
    //Config Paredes que bloquean al iniciar sala
    public List<ObstaculoHundible> grupoA;

    // Paredes ocultas al iniciar sala
    public List<ObstaculoHundible> grupoB;

    // Controla qué grupo está bloqueando el paso actualmente
    private bool grupoAEstaArriba = true;

    private void Start()
    {
        // El Grupo A se asegura de estar arriba
        foreach (ObstaculoHundible pared in grupoA)
        {
            if (pared != null) pared.Mostrar();
        }

        // El Grupo B arranca hundido al instante de forma invisible
        foreach (ObstaculoHundible pared in grupoB)
        {
            if (pared != null) pared.HundirInstantaneo();
        }
    }

    public void AlternarGrupos()
    {
        // Invertimos el valor lógico 
        grupoAEstaArriba = !grupoAEstaArriba;

        // Aplicamos el nuevo estado
        AplicarEstado(grupoA, grupoAEstaArriba);
        AplicarEstado(grupoB, !grupoAEstaArriba);
    }

    private void AplicarEstado(List<ObstaculoHundible> grupo, bool arriba)
    {
        foreach (ObstaculoHundible pared in grupo)
        {
            if (pared != null)
            {
                // Llamamos directamente a tus funciones de hundimiento
                if (arriba)
                {
                    pared.Mostrar();
                }
                else
                {
                    pared.Ocultar();
                }
            }
        }
    }
}