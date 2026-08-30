using UnityEngine;
using UnityEngine.Events;

public class FloorButton : MonoBehaviour
{
    public enum TipoActivador { Cualquiera, SoloRataGrande, SoloRataPequena, SoloBloque, RataGrandeYBloque }

    public TipoActivador quienPuedeActivar = TipoActivador.Cualquiera;
    [Tooltip("True: se queda presionado para siempre. False: se levanta al salir.")]
    public bool seQuedaPresionado = true;

    [Header("Configuración de Fosa")]
    public bool esFosa = false;
    public AudioSource fuenteDeAudio;
    public AudioClip sonidoFosaDefault;
    public AudioClip sonidoFosaClick;

    [Header("Configuración Botón Normal")]
    public AudioClip sonidoBotonClick;
    
    public Animator animatorBoton;
    public string parametroAnim = "IsPressed";

    //Eventos
    public UnityEvent AlPresionar;
    public UnityEvent AlSoltar;

    private bool estaPresionado = false;
    private int objetosEncima = 0;

    private void Start()
    {
        if (esFosa && fuenteDeAudio != null && sonidoFosaDefault != null)
        {
            Debug.Log("Iniciando sonido default de fosa en bucle.");

            fuenteDeAudio.clip = sonidoFosaDefault;
            fuenteDeAudio.loop = true; 
            fuenteDeAudio.Play();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (EsActivadorValido(collision))
        {
            objetosEncima++;

            if (!estaPresionado)
            {
                estaPresionado = true;

                if (animatorBoton != null) animatorBoton.SetBool(parametroAnim, true);

                if (esFosa && fuenteDeAudio != null && sonidoFosaClick != null)
                {
                    fuenteDeAudio.PlayOneShot(sonidoFosaClick);
                }
                else if (!esFosa && fuenteDeAudio != null && sonidoBotonClick != null)
                {
                    fuenteDeAudio.PlayOneShot(sonidoBotonClick);
                }

                AlPresionar.Invoke();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (EsActivadorValido(collision))
        {
            objetosEncima--;
            if (objetosEncima <= 0)
            {
                objetosEncima = 0;

                if (!seQuedaPresionado && estaPresionado)
                {
                    estaPresionado = false;

                    if (animatorBoton != null) animatorBoton.SetBool(parametroAnim, false);

                    AlSoltar.Invoke();
                }
            }
        }
    }

    private bool EsActivadorValido(Collider2D col)
    {
        string nombreLayer = LayerMask.LayerToName(col.gameObject.layer);

        switch (quienPuedeActivar)
        {
            case TipoActivador.Cualquiera:
                return nombreLayer == "RataGrande" || nombreLayer == "RataPeque�a" || col.CompareTag("PushableBlock") || col.CompareTag("Player");
            case TipoActivador.SoloRataGrande:
                return nombreLayer == "RataGrande";
            case TipoActivador.SoloRataPequena:
                return nombreLayer == "RataPeque�a";
            case TipoActivador.SoloBloque:
                return col.CompareTag("PushableBlock");
            case TipoActivador.RataGrandeYBloque:
                return nombreLayer == "RataGrande" || col.CompareTag("PushableBlock");
            default:
                return false;
        }
    }
}