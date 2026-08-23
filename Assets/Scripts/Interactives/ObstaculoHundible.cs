using System.Collections;
using UnityEngine;

public class ObstaculoHundible : MonoBehaviour
{
    //Refs
    [Tooltip("Arrastra aquí el objeto hijo que tiene el SpriteRenderer")]
    public Transform objetoVisual;

    //Config movement
    public float distanciaY = 4f;
    public float duracion = 0.5f;

    private Vector3 posOriginal;
    private Vector3 posOculta;
    private Coroutine rutinaActual;
    private Collider2D colisionadorFisico;

    private void Start()
    {
        // Guardamos las posiciones locales del DIBUJO, no del padre
        if (objetoVisual != null)
        {
            posOriginal = objetoVisual.localPosition;
            posOculta = posOriginal - new Vector3(0f, distanciaY, 0f);
        }

        colisionadorFisico = GetComponent<Collider2D>();
    }

    public void Ocultar()
    {
        if (rutinaActual != null) StopCoroutine(rutinaActual);
        if (objetoVisual != null) rutinaActual = StartCoroutine(RutinaMovimiento(posOculta, false));
    }

    public void Mostrar()
    {
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