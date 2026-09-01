using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FilaEspiral
{
    public string identificador; // Ej: "Fila 1 (Borde Inferior)"
    public List<VentiladorSuelo> grupoSeguroInicial; // Ventiladores que inician en VERDE
    public List<VentiladorSuelo> grupoPeligroInicial; // Ventiladores que inician en ROJO
}

public class GestorGrillaVentiladores : MonoBehaviour
{
    // Tiempos y colores del semáforo
    public float tiempoConstante = 4f;
    public float tiempoTitileo = 0.2f;
    public Color colorVerde = new Color(0f, 1f, 0f, 0.7f);
    public Color colorAmarillo = new Color(1f, 0.9f, 0f, 0.7f);
    public Color colorRojo = new Color(1f, 0f, 0f, 0.7f);

    // Listas de ventiladores y filas del puzzle
    public List<VentiladorSuelo> todosLosVentiladores; // Específico para los 25 ventiladores del nivel 4 zona2
    public List<FilaEspiral> filasDelPuzzle;

    private Coroutine rutinaActual;
    private int indiceFilaActual = -1;

    private void Start()
    {
        // Al iniciar el nivel, los 25 ventiladores están encendidos en rojo
        SetEstadoGlobal(colorRojo, true);
    }

    public void ActivarFila(int indiceFila)
    {
        if (indiceFila == indiceFilaActual) return; // Evita reiniciar si ya estamos en esta fila

        indiceFilaActual = indiceFila;
        if (rutinaActual != null) StopCoroutine(rutinaActual);

        // Apagamos todo el tablero a rojo mortal
        SetEstadoGlobal(colorRojo, true);

        // Encendemos el patrón rítmico solo para la fila donde pisó la rata
        rutinaActual = StartCoroutine(CicloSemaforo(filasDelPuzzle[indiceFila]));
    }

    private IEnumerator CicloSemaforo(FilaEspiral fila)
    {
        while (true)
        {
            // Grupo Seguro en Verde, Grupo Peligro en Rojo
            AplicarEstado(fila.grupoSeguroInicial, colorVerde, false);
            AplicarEstado(fila.grupoPeligroInicial, colorRojo, true);
            yield return new WaitForSeconds(tiempoConstante);

            // Titilan 2 veces antes del cambio
            yield return StartCoroutine(TitileoGrupos(fila.grupoSeguroInicial, colorVerde, fila.grupoPeligroInicial, colorRojo));

            // Transición Amarillo
            AplicarEstado(fila.grupoSeguroInicial, colorAmarillo, false);
            AplicarEstado(fila.grupoPeligroInicial, colorAmarillo, false);
            yield return StartCoroutine(TitileoGrupos(fila.grupoSeguroInicial, colorAmarillo, fila.grupoPeligroInicial, colorAmarillo));

            // Inversión
            AplicarEstado(fila.grupoSeguroInicial, colorRojo, true);
            AplicarEstado(fila.grupoPeligroInicial, colorVerde, false);
            yield return new WaitForSeconds(tiempoConstante);

            //Titilan 2 veces
            yield return StartCoroutine(TitileoGrupos(fila.grupoSeguroInicial, colorRojo, fila.grupoPeligroInicial, colorVerde));

            // Transición Amarillo
            AplicarEstado(fila.grupoSeguroInicial, colorAmarillo, false);
            AplicarEstado(fila.grupoPeligroInicial, colorAmarillo, false);
            yield return StartCoroutine(TitileoGrupos(fila.grupoSeguroInicial, colorAmarillo, fila.grupoPeligroInicial, colorAmarillo));
        }
    }

    private void SetEstadoGlobal(Color color, bool encendido)
    {
        AplicarEstado(todosLosVentiladores, color, encendido);
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