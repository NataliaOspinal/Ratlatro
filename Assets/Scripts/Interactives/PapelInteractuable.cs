using UnityEngine;
using UnityEngine.InputSystem;

public class PapelInteractuable : MonoBehaviour
{
    [Header("Contenido de la hoja")]
    [TextArea(3,5)]
    public string textoHoja;

    [Header("Visuales")]
    public GameObject papelOutline;
    public GameObject tecla;

    private bool jugadorCerca=false;

    private void Start()
    {
        if(papelOutline!=null) papelOutline.SetActive(false);
        if(tecla!=null) tecla.SetActive(false);
    }

    private void Update()
    {
        if (jugadorCerca && Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            SuperPaperUI.Instance.MostrarPapel(textoHoja);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorCerca=true;
            if(papelOutline!=null) papelOutline.SetActive(true);
            if(tecla!=null) tecla.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorCerca=false;
            if(papelOutline!=null) papelOutline.SetActive(false);
            if(tecla!=null) tecla.SetActive(false);
        }
    }
}

