using UnityEngine;
using UnityEngine.InputSystem;

public class PapelMiniRata : MonoBehaviour
{
    [Header("Contenido de la hoja")]
    public PaginaPapel[] contenidoDelPapel;

    [Header("Visuales Exclusivos (Mini Rata)")]
    public GameObject papelOutline;
    public GameObject teclaMiniRata;

    private bool miniRataCerca = false;
    private bool yaRecogido = false;

    private void Start()
    {
        if (papelOutline != null) papelOutline.SetActive(false);
        if (teclaMiniRata != null) teclaMiniRata.SetActive(false);
    }

    private void Update()
    {
        // Se mantiene la tecla F para recoger, validando que sea la mini rata
        if (miniRataCerca && !yaRecogido && Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            yaRecogido = true;

            SuperPaperUI.Instance.MostrarPapel(contenidoDelPapel);

            if (papelOutline != null) papelOutline.SetActive(false);
            if (teclaMiniRata != null) teclaMiniRata.SetActive(false);

            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null) sr.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Filtramos usando el componente CompanionPlayer
        if (!yaRecogido && collision.GetComponent<CompanionPlayer>() != null)
        {
            miniRataCerca = true;
            if (papelOutline != null) papelOutline.SetActive(true);
            if (teclaMiniRata != null) teclaMiniRata.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<CompanionPlayer>() != null)
        {
            miniRataCerca = false;
            if (papelOutline != null) papelOutline.SetActive(false);
            if (teclaMiniRata != null) teclaMiniRata.SetActive(false);
        }
    }
}