using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestorSemaforoVentiladores : MonoBehaviour
{
    //Tiempos
    public float tiempoConstante = 4f;
    public float tiempoTitileo = 0.2f;

    // Colores del semáforo
    public Color colorVerde = new Color(0f, 1f, 0f, 0.7f);
    public Color colorAmarillo = new Color(1f, 0.9f, 0f, 0.7f);
    public Color colorRojo = new Color(1f, 0f, 0f, 0.7f);

    // Grupos de Ventiladores
    public List<VentiladorSuelo> grupoSincronizado; // Ventilador 1 y 3
    public List<VentiladorSuelo> grupoOpuesto;      // Ventilador 2

    private void Start()
    {
        StartCoroutine(CicloSemaforo());
    }

    private IEnumerator CicloSemaforo()
    {
        while (true)
        {
            // Grupo Sincronizado verde, Grupo Opuesto rojo
            AplicarEstado(grupoSincronizado, colorVerde, false);
            AplicarEstado(grupoOpuesto, colorRojo, true);
            yield return new WaitForSeconds(tiempoConstante);

            // Transición Amarillo (todos se detienen y titilan 2 veces)
            AplicarEstado(grupoSincronizado, colorAmarillo, false);
            AplicarEstado(grupoOpuesto, colorAmarillo, false);
            yield return StartCoroutine(TitileoGrupos(grupoSincronizado, colorAmarillo, grupoOpuesto, colorAmarillo));

            // Sincronizados rojo, Opuestos verde
            AplicarEstado(grupoSincronizado, colorRojo, true);
            AplicarEstado(grupoOpuesto, colorVerde, false);
            yield return new WaitForSeconds(tiempoConstante);

            // Transición Amarillo (todos se detienen y titilan 2 veces)
            AplicarEstado(grupoSincronizado, colorAmarillo, false);
            AplicarEstado(grupoOpuesto, colorAmarillo, false);
            yield return StartCoroutine(TitileoGrupos(grupoSincronizado, colorAmarillo, grupoOpuesto, colorAmarillo));
        }
    }

    private void AplicarEstado(List<VentiladorSuelo> grupo, Color colorSemaforo, bool encender)
    {
        foreach (VentiladorSuelo vent in grupo)
        {
            if (encender) vent.Encender();
            else vent.Detenerse();

            SpriteRenderer sr = vent.GetComponentInChildren<SpriteRenderer>();
            if (sr != null) sr.color = colorSemaforo;
        }
    }

    private IEnumerator TitileoGrupos(List<VentiladorSuelo> grupoA, Color colorA, List<VentiladorSuelo> grupoB, Color colorB)
    {
        // Alterna entre el color asignado y blanco para simular 
        for (int i = 0; i < 2; i++)
        {
            SetColorGrupo(grupoA, Color.white);
            SetColorGrupo(grupoB, Color.white);
            yield return new WaitForSeconds(tiempoTitileo);

            SetColorGrupo(grupoA, colorA);
            SetColorGrupo(grupoB, colorB);
            yield return new WaitForSeconds(tiempoTitileo);
        }
    }

    private void SetColorGrupo(List<VentiladorSuelo> grupo, Color color)
    {
        foreach (VentiladorSuelo vent in grupo)
        {
            SpriteRenderer sr = vent.GetComponentInChildren<SpriteRenderer>();
            if (sr != null) sr.color = color;
        }
    }
}