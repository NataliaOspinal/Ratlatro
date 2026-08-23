using UnityEngine;
using UnityEngine.Events;

public class FloorButton : MonoBehaviour
{
    public enum TipoActivador { Cualquiera, SoloRataGrande, SoloRataPequeña, SoloBloque, RataGrandeYBloque }

    //Config del botón
    public TipoActivador quienPuedeActivar = TipoActivador.Cualquiera;
    [Tooltip("True: se queda presionado para siempre. False: se levanta al salir.")]
    public bool seQuedaPresionado = true;

    //Animación
    public Animator animatorBoton;
    public string parametroAnim = "IsPressed";

    //Eventos
    public UnityEvent AlPresionar;
    public UnityEvent AlSoltar;

    private bool estaPresionado = false;
    private int objetosEncima = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (EsActivadorValido(collision))
        {
            //Contador de objetos encima duh
            objetosEncima++;

            if (!estaPresionado)
            {
                estaPresionado = true;

                // Disparamos la animación del botón
                if (animatorBoton != null) animatorBoton.SetBool(parametroAnim, true);

                // Abrimos las puertas
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

                    // Levantamos el botón visualmente
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
                return nombreLayer == "RataGrande" || nombreLayer == "RataPequeña" || col.CompareTag("PushableBlock") || col.CompareTag("Player");
            case TipoActivador.SoloRataGrande:
                return nombreLayer == "RataGrande";
            case TipoActivador.SoloRataPequeña:
                return nombreLayer == "RataPequeña";
            case TipoActivador.SoloBloque:
                return col.CompareTag("PushableBlock");
            case TipoActivador.RataGrandeYBloque:
                return nombreLayer == "RataGrande" || col.CompareTag("PushableBlock");
            default:
                return false;
        }
    }
}