using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CinematicaInicial : MonoBehaviour
{

    [Header("Sonidos (SFX)")]
    public AudioSource reproductorAudio; 
    public AudioClip sfxTemblor; 
    public AudioClip sfxRomper;

    [Header("UI Narrativa")]
    public GameObject panelNubeNegra;
    public TextMeshProUGUI textoNarrativa;
    [TextArea(2, 4)]
    public string[] lineasDialogo = {
        "Hola soy un laboratorio viviente",
        "Ayudame y te libero y esas cosas",
        "No cuestiones nada por favor"
    };
    public float velocidadEscritura = 0.05f;

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

    private void Start()
    {
        panelNubeNegra.SetActive(false);

        jugador = GameObject.Find("Rata");

       if (jugador != null && puntoDeSpawnMesa != null)
        {
            
            jugador.transform.position = new Vector3(
                puntoDeSpawnMesa.position.x, 
                puntoDeSpawnMesa.position.y, 
                jugador.transform.position.z
            );
        }

        // Desactivar el script de movimiento de la rata
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

        foreach (string linea in lineasDialogo)
        {
            textoNarrativa.text = "";
            
            
            foreach (char letra in linea.ToCharArray())
            {
                textoNarrativa.text += letra;
                yield return new WaitForSeconds(velocidadEscritura);
            }

            yield return new WaitUntil(() => 
                (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) ||
                (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
            );
        }

        panelNubeNegra.SetActive(false);
        textoNarrativa.text = "";

        if (RoomManager.Instance != null)
        {
            RoomManager.Instance.interaccionBloqueada = false;
        }

        Door[] puertas = FindObjectsByType<Door>(FindObjectsSortMode.None);

        foreach (Door puerta in puertas)
        {
            Collider2D col = puerta.GetComponent<Collider2D>();
            if (col != null) col.enabled = true;

            puerta.Abrir();
        }
    }

    private void ControlarPuertas(bool activar)
    {
        
        Door[] puertas = FindObjectsByType<Door>(FindObjectsSortMode.None);
        
        foreach (Door puerta in puertas)
        {
            Collider2D col = puerta.GetComponent<Collider2D>();
            if (col != null)
            {
                col.enabled = activar;
            }
        }
    }
}
