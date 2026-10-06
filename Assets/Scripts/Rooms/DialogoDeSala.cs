using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;

[System.Serializable]
public struct LineaDialogoSala
{
    [TextArea(2, 4)]
    public string texto;
    public Sprite spritePersonaje;
}

public class DialogoDeSala : MonoBehaviour
{
    [Header("Motor")]
    public MotorDeDialogos motor;

    [Header("Sonidos (SFX)")] 
    public AudioSource fuenteAudio;
    public AudioClip sfxDialogoLinea;

    [Header("UI Narrativa")]
    public GameObject panelNarracion; 
    public TextMeshProUGUI textoNarrativa;
    
    [Header("Personaje UI (DOTween)")]
    public Image imagenPersonajeUI;
    public float fuerzaSalto = 20f; 
    public float duracionSalto = 0.3f;

    [Header("Historia de la Sala")]
    public LineaDialogoSala[] lineasDialogo; 
    public float velocidadEscritura = 0.05f;
    public float esperaInicial = 0.5f; 

    // Variables para el control de animación
    private RectTransform rectPersonaje;
    private Vector2 posOriginalPersonaje; 

    void Update()
    {
        if (Time.timeScale == 0f) return;
    }

    private void Start()
    {
        if (panelNarracion != null)
        {
            panelNarracion.SetActive(false);
        }

        if (imagenPersonajeUI != null)
        {
            imagenPersonajeUI.gameObject.SetActive(false);
            rectPersonaje = imagenPersonajeUI.GetComponent<RectTransform>();
            posOriginalPersonaje = rectPersonaje.anchoredPosition;
        }

        StartCoroutine(RutinaDialogoEntrada());
    }

   private IEnumerator RutinaDialogoEntrada()
    {
        yield return new WaitForSeconds(esperaInicial);

        if (RoomManager.Instance != null)
        {
            RoomManager.Instance.interaccionBloqueada = true;
        }

        MainPlayer scriptRata = FindAnyObjectByType<MainPlayer>();
        if (scriptRata != null) scriptRata.canMove = false;

        if (panelNarracion != null) panelNarracion.SetActive(true);

        Sprite spriteAnterior = null;

        foreach (LineaDialogoSala linea in lineasDialogo)
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
                fuenteAudio, 
                sfxDialogoLinea
            ));

            yield return null; 
            yield return new WaitUntil(() => motor.BotonSaltarPresionado());
            yield return null;
        }

        if (textoNarrativa != null) textoNarrativa.text = "";
        if (panelNarracion != null) panelNarracion.SetActive(false);

        if (imagenPersonajeUI != null)
        {
            if (rectPersonaje != null)
            {
                DOTween.Kill(rectPersonaje);
                rectPersonaje.anchoredPosition = posOriginalPersonaje;
            }
            imagenPersonajeUI.gameObject.SetActive(false);
        }

        if (RoomManager.Instance != null)
        {
            RoomManager.Instance.interaccionBloqueada = false;
        }

        if (scriptRata != null) scriptRata.canMove = true;

        Destroy(gameObject);
    }
}
