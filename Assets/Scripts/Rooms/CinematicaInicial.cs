using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;

[System.Serializable]
public struct LineaNarrativa
{
    [TextArea(2, 4)]
    public string texto;
    public Sprite spritePersonaje; 
}

public class CinematicaInicial : MonoBehaviour
{
    [Header("Motor")]
    public MotorDeDialogos motor;

    [Header("Sonidos (SFX)")]
    public AudioSource reproductorAudio; 
    public AudioClip sfxTemblor; 
    public AudioClip sfxRomper;
    public AudioClip sfxDialogoLinea;

    [Header("UI Narrativa")]
    public GameObject panelNubeNegra;
    public TextMeshProUGUI textoNarrativa;
    
    public LineaNarrativa[] lineasDialogo; 
    public float velocidadEscritura = 0.05f;

    [Header("Personaje UI")]
    public Image imagenPersonajeUI; 
    public float fuerzaSalto = 30f; 
    public float duracionSalto = 0.35f;

    [Header("Referencias")]
    public Animator mesaAnimator;
    public Transform mascaraCirculo;
    public GameObject pantallaNegra;
    
    [Header("Posicionamiento")]
    public Transform puntoDeSpawnMesa;
   
    [Header("Configuración de Tiempos")]
    public float tiempoSilencioInicial=1f;
    public float duracionTemblor = 1.5f;
    public float fuerzaTemblor = 0.05f;
    public float tiempoEsperaParaAbrir = 1f;
    public float velocidadApertura = 3f;

    private GameObject jugador;
    private MainPlayer scriptRata; 

    private RectTransform rectPersonaje;
    private Vector2 posOriginalPersonaje;

    void Update()
    {
        if (Time.timeScale == 0f) return;
    }

    private void Start()
    {
        panelNubeNegra.SetActive(false);
        if (imagenPersonajeUI != null)
        {
            imagenPersonajeUI.gameObject.SetActive(false);
            rectPersonaje = imagenPersonajeUI.GetComponent<RectTransform>();
            posOriginalPersonaje = rectPersonaje.anchoredPosition;
        }

        jugador = GameObject.Find("Rata");

        if (jugador != null && puntoDeSpawnMesa != null)
        {
            jugador.transform.position = new Vector3(
                puntoDeSpawnMesa.position.x, 
                puntoDeSpawnMesa.position.y, 
                jugador.transform.position.z
            );
            
            scriptRata = jugador.GetComponent<MainPlayer>();
            if (scriptRata != null) scriptRata.canMove = false;
        }

        StartCoroutine(EjecutarCinematica());
    }

    private IEnumerator EjecutarCinematica()
    {
        yield return new WaitForSeconds(tiempoSilencioInicial);

        if (reproductorAudio != null && sfxTemblor != null)
        {
            reproductorAudio.clip = sfxTemblor;
            reproductorAudio.Play();
        }

        Vector3 posicionOriginalMesa = mesaAnimator.transform.position;
        float tiempo = 0f;
        while (tiempo < duracionTemblor)
        {
            mesaAnimator.transform.position = posicionOriginalMesa + (Vector3)Random.insideUnitCircle * fuerzaTemblor;
            tiempo += Time.deltaTime;
            yield return null;
        }
        mesaAnimator.transform.position = posicionOriginalMesa;

        if (reproductorAudio != null)
        {
            reproductorAudio.Stop(); 
        }

        if (reproductorAudio != null && sfxRomper != null)
        {
            reproductorAudio.PlayOneShot(sfxRomper);
        }

        if (mesaAnimator != null)
        {
            mesaAnimator.SetTrigger("Romper");
        }
        
        yield return new WaitForSeconds(tiempoEsperaParaAbrir);

        Vector3 escalaGigante = new Vector3(80f, 80f, 1f);
        while (mascaraCirculo.localScale.x < 79f)
        {
            mascaraCirculo.localScale = Vector3.Lerp(mascaraCirculo.localScale, escalaGigante, velocidadApertura * Time.deltaTime);
            yield return null;
        }

        Destroy(mascaraCirculo.gameObject);
        Destroy(pantallaNegra);
        
        if (RoomManager.Instance != null)
        {
            RoomManager.Instance.interaccionBloqueada = true;
        }

        panelNubeNegra.SetActive(true);

        Sprite spriteAnterior = null;

        foreach (LineaNarrativa linea in lineasDialogo)
        {
            if (textoNarrativa == null) continue;

            spriteAnterior = motor.ActualizarYAnimarSprite(
                imagenPersonajeUI, 
                linea.spritePersonaje, 
                spriteAnterior, 
                rectPersonaje, 
                posOriginalPersonaje,
                fuerzaSalto,
                duracionSalto
            );

            yield return StartCoroutine(motor.EscribirLetraPorLetra(
                textoNarrativa, 
                linea.texto, 
                velocidadEscritura, 
                reproductorAudio, 
                sfxDialogoLinea
            ));

            yield return null;
            yield return new WaitUntil(() => motor.BotonSaltarPresionado());
            yield return null; 
        }

        panelNubeNegra.SetActive(false);
        if (imagenPersonajeUI != null)
        {
            imagenPersonajeUI.transform.DOKill(); 
            imagenPersonajeUI.gameObject.SetActive(false);
        }
        textoNarrativa.text = "";

        if (RoomManager.Instance != null)
        {
            RoomManager.Instance.interaccionBloqueada = false;
        }

        if (scriptRata != null) scriptRata.canMove = true;

        Door[] puertas = FindObjectsByType<Door>(FindObjectsSortMode.None);
        foreach (Door puerta in puertas)
        {
            Collider2D col = puerta.GetComponent<Collider2D>();
            if (col != null) col.enabled = true;
            puerta.Abrir();
        }
    }
}
