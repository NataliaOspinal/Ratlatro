using UnityEngine;
using UnityEngine.InputSystem;

public class PapelInteractuable : MonoBehaviour
{
    [Header("Contenido de la hoja")]
    
    public PaginaPapel[] contenidoDelPapel;

    [Header("Visuales")]
    public GameObject papelOutline;
    public GameObject tecla;

    private bool jugadorCerca = false;
    private bool yaRecogido = false; 

    private void Start()
    {
        if (papelOutline != null) papelOutline.SetActive(false);
        if (tecla != null) tecla.SetActive(false);
    }

    private void Update()
    {
        if (jugadorCerca && !yaRecogido && Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            yaRecogido = true; 
            
            SuperPaperUI.Instance.MostrarPapel(contenidoDelPapel);

            if (papelOutline != null) papelOutline.SetActive(false);
            if (tecla != null) tecla.SetActive(false);

            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null) sr.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !yaRecogido)
        {
            jugadorCerca = true;
            if (papelOutline != null) papelOutline.SetActive(true);
            if (tecla != null) tecla.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorCerca = false;
            if (papelOutline != null) papelOutline.SetActive(false);
            if (tecla != null) tecla.SetActive(false);
        }
    }
}