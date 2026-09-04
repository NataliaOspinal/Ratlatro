using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TerminalTrailer : MonoBehaviour
{

    [Header("Configuración de Escenas")]
    public string nombreEscenaCreditos = "Creditos";
    public float tiempoEsperaParaSalir = 4f;


    [Header("Referencias Visuales")]
    public TextMeshProUGUI textoTerminal;

    [Header("Configuración de Tiempos")]
    public float velocidadEscritura = 0.03f;
    public float esperaEntreLineas = 0.5f;

    [Header("Efectos de Sonido")]
    public AudioSource audioEfectos;
    public AudioClip sonidoTeclado;
    public AudioClip sonidoAccesoConcedido;
    public AudioClip sonidoGlitch;

    [Header("Textos de la Terminal")]
    private string[] secuenciaArranque = {
        "USTED HA TERMINADO LA BETA DE <color=#00FF00>SLIMIN' OUT</color>\n",
        "Registrando nueva informacion del usuario... [<color=#00FF00>OK</color>]\n",
        
        "SISTEMA R4T-001 <color=yellow>EN LÍNEA.</color>\n"
    };

    private string preguntaFinal = "ARCHIVO DE GUARDADO DETECTADO.\n¿Desea saber sobre el lanzamiento oficial de 'Slimin' Out'? (Y/N):_";

    private bool esperandoRespuesta = false;

    private void Start()
    {
        if (textoTerminal != null)
        {
            textoTerminal.text = "";
            textoTerminal.maxVisibleCharacters = 0;
        }
        
        StartCoroutine(RutinaTerminal());
    }

    private void Update()
    {
        if (esperandoRespuesta && Keyboard.current != null)
        {
            if (Keyboard.current.yKey.wasPressedThisFrame)
            {
                esperandoRespuesta = false;
                StartCoroutine(RespuestaAfirmativa());
            }
            else if (Keyboard.current.nKey.wasPressedThisFrame)
            {
                esperandoRespuesta = false;
                StartCoroutine(RespuestaNegativa());
            }
        }
    }

    private IEnumerator RutinaTerminal()
    {
        yield return new WaitForSeconds(1f); 

        foreach (string linea in secuenciaArranque)
        {
            yield return StartCoroutine(EscribirLinea(linea));
            yield return new WaitForSeconds(esperaEntreLineas);
        }

        yield return new WaitForSeconds(1f); 

        
        yield return StartCoroutine(EscribirLinea(preguntaFinal));
        
        
        esperandoRespuesta = true;
    }

    private IEnumerator EscribirLinea(string lineaNueva)
    {
        textoTerminal.ForceMeshUpdate();
        int letrasVisiblesActuales = textoTerminal.textInfo.characterCount;

        textoTerminal.text += lineaNueva;
        
        textoTerminal.maxVisibleCharacters = letrasVisiblesActuales;
        
        textoTerminal.ForceMeshUpdate();
        int totalLetrasNuevas = textoTerminal.textInfo.characterCount;

        for (int i = letrasVisiblesActuales; i <= totalLetrasNuevas; i++)
        {
            textoTerminal.maxVisibleCharacters = i;
            
            if (audioEfectos != null && sonidoTeclado != null)
            {
                audioEfectos.PlayOneShot(sonidoTeclado, 0.2f); 
            }

            yield return new WaitForSeconds(velocidadEscritura);
        }
    }

    private IEnumerator RespuestaAfirmativa()
    {
        textoTerminal.text = textoTerminal.text.Replace("_", "Y"); // Reemplazamos el guion bajo
        textoTerminal.maxVisibleCharacters++; 
        
        if (audioEfectos != null && sonidoAccesoConcedido != null)
            audioEfectos.PlayOneShot(sonidoAccesoConcedido);

        yield return new WaitForSeconds(1f);

        string anuncio = "\n\n<color=#00FF00>ACCESO CONCEDIDO.</color>\nDESENCRIPTANDO ARCHIVOS CLASIFICADOS...\n\n<color=yellow>>> PRÓXIMAMENTE: MÁS NIVELES <<</color>\n<color=yellow>>> NUEVAS MECÁNICAS Y NUEVA MÚSICA <<</color>\n<color=yellow>>> NUEVO ....ROMANCE? <<</color>\nBUSCANDO MÁS FRAGMENTOS DE MEMORIA FALTANTES...";
        yield return StartCoroutine(EscribirLinea(anuncio));

        yield return new WaitForSeconds(tiempoEsperaParaSalir);
        SceneManager.LoadScene(nombreEscenaCreditos);
    }

    private IEnumerator RespuestaNegativa()
    {
        textoTerminal.text = textoTerminal.text.Replace("_", "N");
        textoTerminal.maxVisibleCharacters++; 
        
        if (audioEfectos != null && sonidoGlitch != null)
            audioEfectos.PlayOneShot(sonidoGlitch);

        yield return new WaitForSeconds(0.5f);

        textoTerminal.text = textoTerminal.text.Substring(0, textoTerminal.text.Length - 1) + "<color=red>Y</color>";
        
        textoTerminal.ForceMeshUpdate();
        textoTerminal.maxVisibleCharacters = textoTerminal.textInfo.characterCount;

        if (audioEfectos != null && sonidoAccesoConcedido != null)
            audioEfectos.PlayOneShot(sonidoAccesoConcedido);

        yield return new WaitForSeconds(1f);

        string anuncio = "\n\n<color=#00FF00>ACCESO CONCEDIDO.</color>\nDESENCRIPTANDO ARCHIVOS CLASIFICADOS...\n\n<color=yellow>>> PRÓXIMAMENTE: MÁS NIVELES <<</color>\n<color=yellow>>> NUEVAS MECÁNICAS Y NUEVA MÚSICA <<</color>\n<color=yellow>>> NUEVO ....ROMANCE? <<</color>\nBUSCANDO MÁS FRAGMENTOS DE MEMORIA FALTANTES...";
        yield return StartCoroutine(EscribirLinea(anuncio));

        yield return new WaitForSeconds(tiempoEsperaParaSalir);
        SceneManager.LoadScene(nombreEscenaCreditos);
    }
}