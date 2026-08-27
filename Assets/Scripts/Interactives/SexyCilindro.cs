using UnityEngine;
using TMPro;
using UnityEngine.UI; 
using DG.Tweening;
using System.Collections;


[System.Serializable]
public class PaginaDialogo
{
    [TextArea(2, 5)]
    public string texto;
    public Sprite expresionPersonaje; 
}

public class SexyCilindro : MonoBehaviour
{
    [Header("Configuración Visual")]
    public GameObject indicadorTeclaF; 


    [Header("El Diálogo")]
    public PaginaDialogo[] paginasDeDialogo; 

    [Header("Referencias UI")]
    public GameObject panelDialogoUI; 
    public TextMeshProUGUI textoDialogoUI;

    [Header("Placeholder para Sexyman")]
    public Image imagenExpresionUI; 

    [Header("Ajustes de Animación")]
    
    public float fuerzaTemblorGlitch = 2f;
    public float alturaSaltoPersonaje = 20f;
    public float velocidadDeTexto = 0.03f;

    private bool jugadorCerca = false;
    private bool estaDialogando = false;
    private bool escribiendoTexto = false;
    private int paginaActual = 0;

    private Vector3 posOriginalPanel;
    private Vector3 posOriginalPersonaje;
    private RectTransform rectPanel;
    private RectTransform rectPersonaje;
    private Coroutine corrutinaTexto;
    private Coroutine corrutinaGlitch;

    void Start()
    {
        if (indicadorTeclaF != null) indicadorTeclaF.SetActive(false);
       if (panelDialogoUI != null)
        {
            panelDialogoUI.SetActive(false);
            rectPanel = panelDialogoUI.GetComponent<RectTransform>();
            posOriginalPanel = rectPanel.anchoredPosition;
        }
        if (imagenExpresionUI != null)
        {
            imagenExpresionUI.gameObject.SetActive(false);
            rectPersonaje = imagenExpresionUI.GetComponent<RectTransform>();
            posOriginalPersonaje = rectPersonaje.anchoredPosition;
        }
    }

    void Update()
    {
        if (jugadorCerca && (Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(0)))
        {
            if (!estaDialogando)
            {
                IniciarDialogo();
            }
            else
            if (!escribiendoTexto)
            {
                AvanzarDialogo();
            }
        }
    }

    void IniciarDialogo()
    {
        if (paginasDeDialogo.Length == 0) return; 

        estaDialogando = true;
        paginaActual = 0;
        
        if (indicadorTeclaF != null) indicadorTeclaF.SetActive(false);
        if (panelDialogoUI != null) panelDialogoUI.SetActive(true);
        
        
        if (corrutinaGlitch != null) StopCoroutine(corrutinaGlitch);
        corrutinaGlitch = StartCoroutine(RutinaGlitchPeriodico());

        MostrarPaginaActual();
    }

    void AvanzarDialogo()
    {
        paginaActual++;
        if (paginaActual < paginasDeDialogo.Length)
        {
            MostrarPaginaActual();
        }
        else
        {
            TerminarDialogo();
        }
    }

    void MostrarPaginaActual()
    {
        if (imagenExpresionUI != null)
        {
            Sprite caraActual = paginasDeDialogo[paginaActual].expresionPersonaje;
            
            if (caraActual != null)
            {
                imagenExpresionUI.sprite = caraActual;
                imagenExpresionUI.gameObject.SetActive(true);
            }
            else
            {
                imagenExpresionUI.gameObject.SetActive(false);
            }
        }

        EfectoSaltoPersonaje();

        if (corrutinaTexto != null) StopCoroutine(corrutinaTexto);
        corrutinaTexto = StartCoroutine(EscribirLetraPorLetra(paginasDeDialogo[paginaActual].texto));
    }

    IEnumerator EscribirLetraPorLetra(string textoCompleto)
    {
        escribiendoTexto = true; 
        
        textoDialogoUI.text = textoCompleto;
        textoDialogoUI.maxVisibleCharacters = 0;
        textoDialogoUI.ForceMeshUpdate();
        int totalCaracteres = textoDialogoUI.textInfo.characterCount;

    
        yield return null;

        for (int i = 0; i <= totalCaracteres; i++)
        {
            textoDialogoUI.maxVisibleCharacters = i;

            float cronometro = 0f;
            bool saltoDetectado = false;

            while (cronometro < velocidadDeTexto)
            {
                cronometro += Time.deltaTime;

                if (Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(0))
                {
                    saltoDetectado = true;
                    break;
                }
                
                yield return null; 
            }
            if (saltoDetectado)
            {
                textoDialogoUI.maxVisibleCharacters = totalCaracteres;
                break; 
            }
        }
        yield return null;
        escribiendoTexto = false; 
    }

IEnumerator RutinaGlitchPeriodico()
    {
        while (estaDialogando)
        {
            yield return new WaitForSeconds(3f);

            if (rectPanel != null)
            {
                DOTween.Kill(rectPanel);
                rectPanel.anchoredPosition = posOriginalPanel;
                rectPanel.DOShakeAnchorPos(0.3f, fuerzaTemblorGlitch, 30, 90, false, true);
            }

            if (rectPersonaje != null && imagenExpresionUI.gameObject.activeSelf)
            {
                DOTween.Kill(rectPersonaje);
                rectPersonaje.anchoredPosition = posOriginalPersonaje;
                rectPersonaje.DOShakeAnchorPos(0.3f, fuerzaTemblorGlitch, 30, 90, false, true);
            }
        }
    }

    void EfectoSaltoPersonaje()
    {
        if (rectPersonaje != null && imagenExpresionUI.gameObject.activeSelf)
        {
            DOTween.Kill(rectPersonaje);
            rectPersonaje.anchoredPosition = posOriginalPersonaje;
            rectPersonaje.DOPunchAnchorPos(new Vector2(0, alturaSaltoPersonaje), 0.3f, 1, 0.5f);
        }
    }

    void TerminarDialogo()
    {
        estaDialogando = false;

        if (corrutinaGlitch != null) StopCoroutine(corrutinaGlitch);
        
        if (rectPanel != null)
        {
            DOTween.Kill(rectPanel);
            rectPanel.anchoredPosition = posOriginalPanel;
        }

        if (panelDialogoUI != null) panelDialogoUI.SetActive(false);
        if (imagenExpresionUI != null) imagenExpresionUI.gameObject.SetActive(false);
        if (jugadorCerca && indicadorTeclaF != null) indicadorTeclaF.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorCerca = true;
            if (indicadorTeclaF != null && !estaDialogando) indicadorTeclaF.SetActive(true);
            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorCerca = false;
            TerminarDialogo();
            if (indicadorTeclaF != null) indicadorTeclaF.SetActive(false);
            
        }
    }
}