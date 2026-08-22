using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class AuraRadioactivePlayer : MonoBehaviour
{
    public Light2D auraLight;
    public float normalIntensity = 0.7f;
    public float dimmedIntensity = 0.2f;
    
    // Referencias para manipular la profundidad visual
    private SortingGroup sortingGroup;
    private SpriteRenderer spriteRenderer;
    private int originalOrder;

    void Start()
    {
        // Buscamos qué componente usa este personaje
        sortingGroup = GetComponent<SortingGroup>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Guardamos su capa original para no romper la lógica de cuando se cargan mutuamente
        if (sortingGroup != null) originalOrder = sortingGroup.sortingOrder;
        else if (spriteRenderer != null) originalOrder = spriteRenderer.sortingOrder;
    }

    public void SetOccluded(bool isOccluded)
    {
        //Atenuar la luz 
        if (auraLight != null)
        {
            auraLight.intensity = isOccluded ? dimmedIntensity : normalIntensity;
        }

        // Forzar la profundidad visual
        // Si está oculta, le restamos 1 a su orden para mandarla detrás del obstáculo.
        // Si sale, la devolvemos a su orden original.
        int newOrder = isOccluded ? originalOrder - 1 : originalOrder;

        if (sortingGroup != null)
        {
            sortingGroup.sortingOrder = newOrder;
        }
        else if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = newOrder;
        }
    }
}