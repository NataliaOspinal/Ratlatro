using UnityEngine;
using DG.Tweening;

public class MenuOpcionesAnimado : MonoBehaviour
{
    [Header("Referencias")]
    public RectTransform panelOpciones;

    [Header("Configuración de Posiciones")]
    public float posicionOcultaX; 
    

    public float posicionVisibleX; 

    public float duracionAnimacion = 0.5f;

    private void Start()
    {
        if (panelOpciones != null)
        {
            panelOpciones.anchoredPosition = new Vector2(posicionOcultaX, panelOpciones.anchoredPosition.y);
        }
    }

    public void MostrarOpciones()
    {
        if (panelOpciones != null)
        {
            
            panelOpciones.DOAnchorPosX(posicionVisibleX, duracionAnimacion).SetEase(Ease.OutBack, 0.7f);
        }
    }

    public void OcultarOpciones()
    {
        if (panelOpciones != null)
        {
            panelOpciones.DOAnchorPosX(posicionOcultaX, duracionAnimacion).SetEase(Ease.InBack, 0.7f);
        }
    }
}

