using UnityEngine;
using UnityEngine.InputSystem;

public class PapelInteractuable : MonoBehaviour
{
    [Header("Contenido de la hoja")]
    [TextArea(3, 5)]
    public string textoHoja;

    [Header("Visuales")]
    public GameObject papelOutline;
    public GameObject tecla;

    private bool jugadorCerca = false;
    private bool yaRecogido = false; // Nueva variable de control

    private void Start()
    {
        if (papelOutline != null) papelOutline.SetActive(false);
        if (tecla != null) tecla.SetActive(false);
    }

    private void Update()
    {
        // Solo permite interactuar si está cerca, no se ha recogido, y pulsa F
        if (jugadorCerca && !yaRecogido && Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            yaRecogido = true; // Lo marcamos como leído
            SuperPaperUI.Instance.MostrarPapel(textoHoja);

            // Apagamos los visuales de interacción
            if (papelOutline != null) papelOutline.SetActive(false);
            if (tecla != null) tecla.SetActive(false);

            // Apagamos el dibujo del papel en el suelo para que parezca que lo guardó
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