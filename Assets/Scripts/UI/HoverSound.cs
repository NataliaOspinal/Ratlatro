using UnityEngine;
using UnityEngine.EventSystems;

public class HoverSonido : MonoBehaviour, IPointerEnterHandler
{
    [Header("Configuración de Sonido")]
    public AudioSource fuenteEfectos;
    public AudioClip sonidoHover;

    public void OnPointerEnter(PointerEventData eventData)
    {

        UnityEngine.UI.Button miBoton = GetComponent<UnityEngine.UI.Button>();
        if (miBoton != null && !miBoton.interactable) return;

        if (fuenteEfectos != null && sonidoHover != null)
        {
            fuenteEfectos.PlayOneShot(sonidoHover);
        }
    }
}