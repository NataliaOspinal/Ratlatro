using UnityEngine;
using DG.Tweening;

public class MenuOpcionesAnimado : MonoBehaviour
{
    [Header("Referencias")]
    public RectTransform panelOpciones;

    [Header("Configuración de Sonido")]
    public AudioSource fuenteSonido; 
    public AudioClip sonidoAbrirPapel;
   

    [Header("Configuración de Posiciones")]
    public float posicionOcultaX; 

    

    public float posicionVisibleX; 

    public float duracionAnimacion = 0.5f;
    private bool estaVisible = false;

    private void Start()
    {
        if (panelOpciones != null)
        {
            panelOpciones.anchoredPosition = new Vector2(posicionOcultaX, panelOpciones.anchoredPosition.y);
        }

        
    }

    public void AlternarOpciones()
    {
        if (estaVisible)
        {
            OcultarOpciones();
        }
        else
        {
            MostrarOpciones();
        }
    }

    public void MostrarOpciones()
    {
        if (panelOpciones != null)
        {
            
            panelOpciones.DOAnchorPosX(posicionVisibleX, duracionAnimacion).SetEase(Ease.OutBack, 0.7f);
        }

        if (fuenteSonido != null && sonidoAbrirPapel != null)
        {
            fuenteSonido.PlayOneShot(sonidoAbrirPapel);
        }

        estaVisible = true;
    }

    public void OcultarOpciones()
    {
        if (panelOpciones != null)
        {
            panelOpciones.DOAnchorPosX(posicionOcultaX, duracionAnimacion).SetEase(Ease.InBack, 0.7f);
        }

        if (fuenteSonido != null && sonidoAbrirPapel != null)
        {
            fuenteSonido.PlayOneShot(sonidoAbrirPapel);
        }

        estaVisible = false;
    }
}

