using UnityEngine;
using UnityEngine.Rendering.Universal; 

public class AuraRadioactivePlayer : MonoBehaviour
{
    // Luz configurada en el prefab del jugador, que representa el aura radioactiva
    public Light2D auraLight;
    public float normalIntensity = 0.8f;
    public float dimmedIntensity = 0.3f; // Qué tan tenue se vuelve al estar detrás

    public void SetOccluded(bool isOccluded)
    {
        if (auraLight != null)
        {
            // Cambia la intensidad dependiendo de si está oculto o no
            auraLight.intensity = isOccluded ? dimmedIntensity : normalIntensity;
        }
    }
}