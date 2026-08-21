using UnityEngine;

public class ObstacleObstruction : MonoBehaviour
{
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Buscamos todos los controladores de luz en el jugador y en sus hijos (para que aplique a la ratinha cargada)
            AuraRadioactivePlayer[] auras = other.GetComponentsInChildren<AuraRadioactivePlayer>();

            // Recorremos cada aura encontrada y la actualizamos
            foreach (AuraRadioactivePlayer aura in auras)
            {
                // Usamos la posición y del jugador que activó el trigger para que ambas ratas se oculten al mismo tiempo
                if (other.transform.position.y > transform.position.y)
                {
                    aura.SetOccluded(true);
                }
                else
                {
                    aura.SetOccluded(false);
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Al salir, restauramos todas las luces encontradas
            AuraRadioactivePlayer[] auras = other.GetComponentsInChildren<AuraRadioactivePlayer>();

            foreach (AuraRadioactivePlayer aura in auras)
            {
                aura.SetOccluded(false);
            }
        }
    }
}