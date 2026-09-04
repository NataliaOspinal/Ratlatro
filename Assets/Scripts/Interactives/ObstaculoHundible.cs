using System.Collections;
using UnityEngine;

public class ObstaculoHundible : MonoBehaviour
{
    //Refs
    [Tooltip("Arrastra aqu� el objeto hijo que tiene el SpriteRenderer")]
    public Transform objetoVisual;

    [Header("Efectos de Sonido")]
    public AudioSource fuenteAudio;
    public AudioClip sonidoHundir;

    //Config movement
    public float distanciaY = 4.5f;
    public float duracion = 0.5f;

    private Vector3 posOriginal;
    private Vector3 posOculta;
    private Coroutine rutinaActual;
    private Collider2D colisionadorFisico;

    private void Awake()
    {
        if (objetoVisual != null)
        {
            posOriginal = objetoVisual.localPosition;
            posOculta = posOriginal - new Vector3(0f, distanciaY, 0f);
        }

        colisionadorFisico = GetComponent<Collider2D>();
    }

    // Teletransporta la pared hacia abajo sin animación ni sonido
    public void HundirInstantaneo()
    {
        if (objetoVisual != null) objetoVisual.localPosition = posOculta;
        if (colisionadorFisico != null) colisionadorFisico.enabled = false;
    }

    public void Ocultar()
    {
        // Si la pared está apagada (ej. reiniciando la sala), se hunde al instante y aborta (rip) el resto
        if (!gameObject.activeInHierarchy)
        {
            HundirInstantaneo();
            return;
        }

        // Nos aseguramos de que el AudioSource esté encendido antes de reproducir
        if (fuenteAudio != null && sonidoHundir != null && fuenteAudio.isActiveAndEnabled)
        {
            fuenteAudio.PlayOneShot(sonidoHundir);
        }

        if (rutinaActual != null) StopCoroutine(rutinaActual);
        if (objetoVisual != null) rutinaActual = StartCoroutine(RutinaMovimiento(posOculta, false));
    }

    public void Mostrar()
    {
        // Si la pared está apagada, la restauramos a su posición original al instante
        if (!gameObject.activeInHierarchy)
        {
            if (objetoVisual != null) objetoVisual.localPosition = posOriginal;
            if (colisionadorFisico != null) colisionadorFisico.enabled = true;
            return;
        }

        if (rutinaActual != null) StopCoroutine(rutinaActual);
        if (objetoVisual != null) rutinaActual = StartCoroutine(RutinaMovimiento(posOriginal, true));
    }

    private IEnumerator RutinaMovimiento(Vector3 destino, bool emergiendo)
    {
        if (emergiendo && colisionadorFisico != null) colisionadorFisico.enabled = true;

        Vector3 origen = objetoVisual.localPosition;
        float tiempoPasado = 0f;

        while (tiempoPasado < duracion)
        {
            tiempoPasado += Time.deltaTime;
            float porcentaje = Mathf.SmoothStep(0f, 1f, tiempoPasado / duracion);

            // Movemos solo el objeto visual
            objetoVisual.localPosition = Vector3.Lerp(origen, destino, porcentaje);
            yield return null;
        }

        objetoVisual.localPosition = destino;

        if (!emergiendo && colisionadorFisico != null) colisionadorFisico.enabled = false;
    }
}