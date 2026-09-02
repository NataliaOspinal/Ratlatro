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
    public Sprite expresionRata;
}

public class SexyCilindro : MonoBehaviour
{
    [Header("Condición de Desbloqueo")]
    public bool necesitaPuertaAbierta = false; 
    public Door[] puertaRequisito;


    [Header("Configuración Visual")]
    public GameObject indicadorTeclaF; 

    [Header("Activación")]
    public GameObject signoInterrogacion;
    public AudioSource fuenteAudio;
    public AudioClip sonidoAparece;


    [Header("El Diálogo")]
    public PaginaDialogo[] paginasDeDialogo; 

    [Header("Referencias UI")]
    public GameObject panelDialogoUI; 
    public TextMeshProUGUI textoDialogoUI;

    [Header("Placeholder para Sexyman")]
    public Image imagenExpresionUI; 
    public Image imagenRataUI;

    [Header("Ajustes de Animación")]
    
    public float fuerzaTemblorGlitch = 2f;
    public float alturaSaltoPersonaje = 20f;
    public float velocidadDeTexto = 0.03f;

    private bool jugadorCerca = false;
    private bool estaDialogando = false;
    private bool escribiendoTexto = false;
    private int paginaActual = 0;

    private bool avisoMostrado = false;
    private bool yaInteractuado = false;

    private RectTransform rectPanel;
    private Vector3 posOriginalPanel;
    
    private RectTransform rectPersonaje;
    private Vector3 posOriginalPersonaje;
    
    private RectTransform rectRata;
    private Vector3 posOriginalRata;

    private Sprite spriteAnteriorDoctor = null;
    private Sprite spriteAnteriorRata = null;

    private Coroutine corrutinaTexto;
    private Coroutine corrutinaGlitch;
    private MainPlayer scriptRata;

    void Start()
    {
        if (indicadorTeclaF != null) indicadorTeclaF.SetActive(false);
        if (signoInterrogacion != null) signoInterrogacion.SetActive(false);
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
        if (imagenRataUI != null)
        {
            rectRata = imagenRataUI.GetComponent<RectTransform>();
            posOriginalRata = rectRata.anchoredPosition;
            imagenRataUI.gameObject.SetActive(false); 
        }
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;
        if (jugadorCerca)
        {
            if (!estaDialogando)
            {
                bool puede = PuedeInteractuar();
                if (indicadorTeclaF != null && indicadorTeclaF.activeSelf != puede) 
                {
                    indicadorTeclaF.SetActive(puede);
                }

                if (puede && !avisoMostrado && !yaInteractuado)
                {
                    avisoMostrado = true; 

                    if (signoInterrogacion != null)
                    {
                        signoInterrogacion.SetActive(true);
                        signoInterrogacion.transform.DOLocalMoveY(signoInterrogacion.transform.localPosition.y + 0.3f, 0.8f)
                            .SetLoops(-1, LoopType.Yoyo)
                            .SetEase(Ease.InOutSine);
                    }
                    
                    if (fuenteAudio != null && sonidoAparece != null)
                    {
                        fuenteAudio.PlayOneShot(sonidoAparece);
                    }
                }

                if (puede && (Input.GetKeyDown(KeyCode.F)))
                {
                    IniciarDialogo();
                }
            }
            else if (!escribiendoTexto)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    AvanzarDialogo();
                }
            }
        }
    }

    void IniciarDialogo()
    {
        if (paginasDeDialogo.Length == 0) return;

        yaInteractuado = true; 
        if (signoInterrogacion != null)
        {
            DOTween.Kill(signoInterrogacion.transform); 
            signoInterrogacion.SetActive(false);
        }

        scriptRata = FindAnyObjectByType<MainPlayer>();
        if (scriptRata != null) scriptRata.canMove = false;

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
        PaginaDialogo pagina = paginasDeDialogo[paginaActual];

        if (imagenExpresionUI != null)
        {
            if (pagina.expresionPersonaje != null)
            {
                imagenExpresionUI.sprite = pagina.expresionPersonaje;
                imagenExpresionUI.gameObject.SetActive(true);

                if (pagina.expresionPersonaje != spriteAnteriorDoctor)
                {
                    if (rectPersonaje != null)
                    {
                        DOTween.Kill(rectPersonaje);
                        rectPersonaje.anchoredPosition = posOriginalPersonaje;
                        rectPersonaje.DOPunchAnchorPos(new Vector2(0, alturaSaltoPersonaje), 0.3f, 1, 0.5f);
                    }
                    spriteAnteriorDoctor = pagina.expresionPersonaje;
                }
            }
            else
            {
                imagenExpresionUI.gameObject.SetActive(false);
                spriteAnteriorDoctor = null;
            }
        }

        if (imagenRataUI != null)
        {
            if (pagina.expresionRata != null)
            {
                bool esNuevoSprite = (pagina.expresionRata != spriteAnteriorRata);

                imagenRataUI.sprite = pagina.expresionRata;
                imagenRataUI.gameObject.SetActive(true);

                if (esNuevoSprite && rectRata != null)
                {
                    rectRata.DOKill(true); 
                    rectRata.anchoredPosition = posOriginalRata;
                    rectRata.DOPunchAnchorPos(new Vector2(0, alturaSaltoPersonaje), 0.3f, 1, 0.5f);
                    
                    spriteAnteriorRata = pagina.expresionRata;
                }
            }
            else
            {
                imagenRataUI.gameObject.SetActive(false);
                spriteAnteriorRata = null;
            }
        }

        if (corrutinaTexto != null) StopCoroutine(corrutinaTexto);
        corrutinaTexto = StartCoroutine(EscribirLetraPorLetra(pagina.texto));
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

        if (scriptRata != null) scriptRata.canMove = true;

        if (corrutinaGlitch != null) StopCoroutine(corrutinaGlitch);

        if (rectPanel != null)
        {
            DOTween.Kill(rectPanel);
            rectPanel.anchoredPosition = posOriginalPanel;
        }

        if (panelDialogoUI != null) panelDialogoUI.SetActive(false);
        if (imagenExpresionUI != null)
        {
            if (rectPersonaje != null)
            {
                DOTween.Kill(rectPersonaje);
                rectPersonaje.anchoredPosition = posOriginalPersonaje;
            }
            imagenExpresionUI.gameObject.SetActive(false);
        }
        
        if (imagenRataUI != null)
        {
            if (rectRata != null)
            {
                DOTween.Kill(rectRata);
                rectRata.anchoredPosition = posOriginalRata;
            }
            imagenRataUI.gameObject.SetActive(false);
        }

        if (jugadorCerca && indicadorTeclaF != null) indicadorTeclaF.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorCerca = true;
           
            
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

    private bool PuedeInteractuar()
    {
        if (!necesitaPuertaAbierta) return true;
        
        foreach (Door puerta in puertaRequisito)
        {
            if (puerta != null && !puerta.estaAbierta)
            {
                return false; 
            }
        }

        return true; 
    }
}