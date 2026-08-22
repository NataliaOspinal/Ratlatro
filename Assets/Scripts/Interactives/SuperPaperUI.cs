using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class SuperPaperUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static SuperPaperUI Instance;

    [Header("Referencias Visuales")]
    public Image imagenOutline;
    public TMP_Text textoPapel;

    [Header("Coordenadas y Tiempo")]
    public RectTransform posicionCentro;
    public RectTransform posicionDerecha;
    [Tooltip("Segundos que tarda en hacer el viaje completo")]
    public float duracionMovimiento = 0.3f;

    private RectTransform miRectTransform;

    // Máquina de estados para bloquear el temblor
    private bool estaAnimando = false;
    private bool estaEnCentro = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        miRectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void MostrarPapel(string nuevoTexto)
    {
        if (textoPapel != null) textoPapel.text = nuevoTexto;

        // Reiniciamos los estados y lo ponemos a la derecha
        estaAnimando = false;
        estaEnCentro = false;
        miRectTransform.anchoredPosition = posicionDerecha.anchoredPosition;

        if (imagenOutline != null) imagenOutline.enabled = true;

        gameObject.SetActive(true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Solo viaja al centro si está quieto a la derecha
        if (!estaAnimando && !estaEnCentro)
        {
            if (imagenOutline != null) imagenOutline.enabled = false;
            StartCoroutine(RutinaMoverPapel(posicionCentro.anchoredPosition, true));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Solo regresa a la derecha si está quieto en el centro
        if (!estaAnimando && estaEnCentro)
        {
            if (imagenOutline != null) imagenOutline.enabled = true;
            StartCoroutine(RutinaMoverPapel(posicionDerecha.anchoredPosition, false));
        }
    }

    // La Corrutina que fuerza a terminar el movimiento
    private IEnumerator RutinaMoverPapel(Vector2 destino, bool haciaCentro)
    {
        estaAnimando = true; // Bloqueamos el input
        Vector2 origen = miRectTransform.anchoredPosition;
        float tiempoPasado = 0f;

        while (tiempoPasado < duracionMovimiento)
        {
            tiempoPasado += Time.deltaTime;

            // SmoothStep da un efecto de ease-in / ease-out muy pulido
            float porcentaje = Mathf.SmoothStep(0f, 1f, tiempoPasado / duracionMovimiento);
            miRectTransform.anchoredPosition = Vector2.Lerp(origen, destino, porcentaje);

            yield return null; // Esperamos al siguiente frame
        }

        // Aseguramos que llegue exactamente a la coordenada final
        miRectTransform.anchoredPosition = destino;

        // Actualizamos el estado y abrimos el candado
        estaEnCentro = haciaCentro;
        estaAnimando = false;
    }
}