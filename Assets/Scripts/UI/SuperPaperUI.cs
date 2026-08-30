using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class SuperPaperUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static SuperPaperUI Instance;

    [Header("Efectos de Sonido")]
    public AudioSource fuenteSonido;
    public AudioClip sonidoEntrar;

    [Header("Referencias Visuales")]
    public Image imagenOutline;
    public TMP_Text textoPapel;

    [Header("Tutorial Flecha")]
    public GameObject flechaTutorial;
    private Vector3 escalaOriginalFlecha;

    [Header("Coordenadas y Tiempo")]
    public RectTransform posicionCentro;
    public RectTransform posicionDerecha;
    public float duracionMovimiento = 0.3f;

    private RectTransform miRectTransform;

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
        if (flechaTutorial != null) 
        {
            escalaOriginalFlecha = flechaTutorial.transform.localScale;
            flechaTutorial.SetActive(false);
        }
    }

    public void MostrarPapel(string nuevoTexto)
    {
        if (textoPapel != null) textoPapel.text = nuevoTexto;

        estaAnimando = false;
        estaEnCentro = false;
        miRectTransform.anchoredPosition = posicionDerecha.anchoredPosition;

        if (imagenOutline != null) imagenOutline.enabled = true;

       if (flechaTutorial != null)
        {
            DOTween.Kill(flechaTutorial.transform); 
            
            flechaTutorial.SetActive(true); 
            flechaTutorial.transform.localScale = escalaOriginalFlecha; 
            
            // La hacemos palpitar un 20% más grande
            Vector3 escalaPalpito = escalaOriginalFlecha * 1.2f;
            flechaTutorial.transform.DOScale(escalaPalpito, 0.4f).SetLoops(-1, LoopType.Yoyo);
        }

        gameObject.SetActive(true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (flechaTutorial != null)
        {
            DOTween.Kill(flechaTutorial.transform);
            flechaTutorial.SetActive(false); 
        }

        if (!estaAnimando && !estaEnCentro)
        {
            if (imagenOutline != null) imagenOutline.enabled = false;

            if (fuenteSonido != null && sonidoEntrar != null)
            {
                fuenteSonido.PlayOneShot(sonidoEntrar);
            }

            StartCoroutine(RutinaMoverPapel(posicionCentro.anchoredPosition, true));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!estaAnimando && estaEnCentro)
        {

            if (fuenteSonido != null && sonidoEntrar != null)
            {
                fuenteSonido.PlayOneShot(sonidoEntrar);
            }
            
            StartCoroutine(RutinaMoverPapel(posicionDerecha.anchoredPosition, false));
        }
    }

    private IEnumerator RutinaMoverPapel(Vector2 destino, bool haciaCentro)
    {
        estaAnimando = true; 
        Vector2 origen = miRectTransform.anchoredPosition;
        float tiempoPasado = 0f;

        while (tiempoPasado < duracionMovimiento)
        {
            tiempoPasado += Time.deltaTime;

            float porcentaje = Mathf.SmoothStep(0f, 1f, tiempoPasado / duracionMovimiento);
            miRectTransform.anchoredPosition = Vector2.Lerp(origen, destino, porcentaje);

            yield return null;
        }

        miRectTransform.anchoredPosition = destino;

        estaEnCentro = haciaCentro;
        estaAnimando = false;
    }
}