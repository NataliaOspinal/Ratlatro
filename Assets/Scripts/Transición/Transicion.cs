using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PantallaGuardado : MonoBehaviour
{
    [Header("Referencias Visuales")]
    public Transform rata;
    public Transform puntoIzquierda;
    public Transform puntoDerecha;
    public SpriteRenderer iconoIzquierda;
    public SpriteRenderer iconoDerecha;
    
    [Header("Textos UI")]
    public TextMeshProUGUI textoPregunta;
    public GameObject textoGuardando;
    public TextMeshProUGUI textoNombreIzquierda;
    public TextMeshProUGUI textoNombreDerecha;

    [Header("Configuración de Zonas")]
    [Tooltip("Los nombres de las zonas en orden.")]
    public string[] nombresPorZona; 
    [Tooltip("Los dibujos de las zonas en orden. 0 = Zona 1, 1 = Zona 2, etc.")]
    public Sprite[] spritesPorZona;
    [Tooltip("El nombre exacto de la escena de cada zona.")]
    public string[] escenasPorZona;

    [Header("Audio")]
    public AudioSource fuenteDeAudio;
    public AudioClip sonidoYes;
    public AudioClip sonidoNo;
    public AudioClip sonidoEscribir;

    [Header("Ajustes")]
    public float tiempoViajeRata = 2f;
    public string nombreEscenaMenu = "00_MainMenu";
    public float velocidadTexto = 0.05f;

    private bool esperandoRespuesta = false;
    private int zonaDestino = 1;

    void Start()
    {
        textoPregunta.gameObject.SetActive(false);
        textoGuardando.SetActive(true);

        zonaDestino = PlayerPrefs.GetInt("SiguienteZona", 1);
        int zonaOrigen = zonaDestino - 1;
        if (zonaOrigen < 0) zonaOrigen = 0;

        if (zonaOrigen < spritesPorZona.Length) 
        {
            iconoIzquierda.sprite = spritesPorZona[zonaOrigen];
            if (zonaOrigen < nombresPorZona.Length) textoNombreIzquierda.text = nombresPorZona[zonaOrigen];
        }
        
        if (zonaDestino < spritesPorZona.Length) 
        {
            iconoDerecha.sprite = spritesPorZona[zonaDestino];
            if (zonaDestino < nombresPorZona.Length) textoNombreDerecha.text = nombresPorZona[zonaDestino];
        }

        PlayerPrefs.SetString("NivelGuardado", escenasPorZona[zonaDestino]);
        PlayerPrefs.Save(); 

        StartCoroutine(AnimacionRata());
    }

    IEnumerator AnimacionRata()
    {
        rata.position = puntoIzquierda.position;
        float tiempo = 0;

        while (tiempo < tiempoViajeRata)
        {
            tiempo += Time.deltaTime;
            rata.position = Vector3.Lerp(puntoIzquierda.position, puntoDerecha.position, Mathf.SmoothStep(0, 1, tiempo / tiempoViajeRata));
            yield return null;
        }

        textoGuardando.SetActive(false);
        // En lugar de activarlo de golpe, llamamos a la nueva animación de texto
        StartCoroutine(MostrarTextoLetraPorLetra());
    }

    void Update()
    {
        if (!esperandoRespuesta) return;

        if (Input.GetKeyDown(KeyCode.Y))
        {
            esperandoRespuesta = false; 
            
            textoPregunta.text += " Y";
            textoPregunta.maxVisibleCharacters = 9999; 
            
            StartCoroutine(ProcesarSeleccion(sonidoYes, escenasPorZona[zonaDestino]));
        }
        // Si presiona la N
        else if (Input.GetKeyDown(KeyCode.N))
        {
            esperandoRespuesta = false; // Bloqueamos
            
            // --- NUEVO: Añadimos la N al final del texto ---
            textoPregunta.text += " N";
            textoPregunta.maxVisibleCharacters = 9999; 
            
            StartCoroutine(ProcesarSeleccion(sonidoNo, nombreEscenaMenu));
        }
    }

    IEnumerator ProcesarSeleccion(AudioClip sonido, string escenaACargar)
    {
        if (fuenteDeAudio != null && sonido != null)
        {
            fuenteDeAudio.PlayOneShot(sonido);
            yield return new WaitForSeconds(sonido.length); 
        }

        SceneManager.LoadScene(escenaACargar);
    }

    IEnumerator MostrarTextoLetraPorLetra()
    {
        textoPregunta.gameObject.SetActive(true);
        textoPregunta.maxVisibleCharacters = 0;
        
        textoPregunta.ForceMeshUpdate();
        int totalLetras = textoPregunta.textInfo.characterCount;

        for (int i = 0; i <= totalLetras; i++)
        {
            textoPregunta.maxVisibleCharacters = i;

            
            if (i > 0 && fuenteDeAudio != null && sonidoEscribir != null)
            {
                char letraRevelada = textoPregunta.textInfo.characterInfo[i - 1].character;
                
                if (letraRevelada != ' ')
                {
                    fuenteDeAudio.pitch = Random.Range(0.95f, 1.05f);
                    fuenteDeAudio.PlayOneShot(sonidoEscribir);
                }
            }

            yield return new WaitForSeconds(velocidadTexto); 
        }

        if (fuenteDeAudio != null) fuenteDeAudio.pitch = 1f;

        esperandoRespuesta = true;
    }
}