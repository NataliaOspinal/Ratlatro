using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FilaEspiral
{
    public string identificador;
    public List<VentiladorSuelo> grupoSeguroInicial; // Ventiladores que inician en verde aka seguros
    public List<VentiladorSuelo> grupoPeligroInicial; // Ventiladores que inician en rojo aka peligrosos
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

    public bool puzzleCompletado = false;

    private Coroutine rutinaActual;
    private int indiceFilaActual = -1;

    private void Start()
    {
        // Al iniciar el nivel, los 25 ventiladores están encendidos en rojo
        SetEstadoGlobal(colorRojo, true);
    }

    public void ActivarFila(int indiceFila)
    {
        if (puzzleCompletado) return;
        if (indiceFila == indiceFilaActual) return;

        if (rutinaActual != null) StopCoroutine(rutinaActual);

        // Si venimos de una fila anterior, disparamos la trampa con retraso
        if (indiceFilaActual != -1)
        {
            FilaEspiral filaAnterior = filasDelPuzzle[indiceFilaActual];
            FilaEspiral filaNueva = filasDelPuzzle[indiceFila];
            StartCoroutine(ConvertirAnteriorEnTrampaConRetraso(filaAnterior, filaNueva));
        }
        else
        {
            SetEstadoGlobal(colorRojo, true);
        }

        //Actualizamos el índice y arrancamos el ritmo para la nueva fila
        indiceFilaActual = indiceFila;
        rutinaActual = StartCoroutine(CicloSemaforo(filasDelPuzzle[indiceFila]));
    }

    private IEnumerator ConvertirAnteriorEnTrampaConRetraso(FilaEspiral anterior, FilaEspiral nueva)
    {
        List<VentiladorSuelo> ventiladoresNuevos = new List<VentiladorSuelo>();
        ventiladoresNuevos.AddRange(nueva.grupoSeguroInicial);
        ventiladoresNuevos.AddRange(nueva.grupoPeligroInicial);

        // Apagamos la fila anterior a Amarillo así no se re muere la rata al toque
        AplicarEstadoExcluyendo(anterior.grupoSeguroInicial, ventiladoresNuevos, colorAmarillo, false);
        AplicarEstadoExcluyendo(anterior.grupoPeligroInicial, ventiladoresNuevos, colorAmarillo, false);

        // Le damos 0.8 segundos a la rata para que termine de entrar a la nueva baldosa
        yield return new WaitForSeconds(0.8f);

        // Volvemos la fila anterior Roja
        AplicarEstadoExcluyendo(anterior.grupoSeguroInicial, ventiladoresNuevos, colorRojo, true);
        AplicarEstadoExcluyendo(anterior.grupoPeligroInicial, ventiladoresNuevos, colorRojo, true);
    }

    private void AplicarEstadoExcluyendo(List<VentiladorSuelo> grupoAnterior, List<VentiladorSuelo> exclusion, Color colorEstado, bool encender)
    {
        foreach (VentiladorSuelo vent in grupoAnterior)
        {
            // Si el ventilador viejo NO pertenece a la nueva fila, le aplicamos el cambio
            if (!exclusion.Contains(vent))
            {
                if (encender) vent.Encender();
                else vent.Detenerse();

                SpriteRenderer sr = vent.GetComponentInChildren<SpriteRenderer>();
                if (sr != null) sr.color = colorEstado;
            }
        }
    }

    public void DesactivarPuzzle()
    {
        puzzleCompletado = true;
        // Frena el reloj interno del semáforo
        if (rutinaActual != null) StopCoroutine(rutinaActual);

        // Pone absolutamente todos los ventiladores en Verde (apagados y seguros)
        SetEstadoGlobal(colorVerde, false);
    }

    private IEnumerator CicloSemaforo(FilaEspiral fila)
    {
        while (true)
        {
            // Grupo Seguro en Verde, Grupo Peligro en Rojo
            AplicarEstado(fila.grupoSeguroInicial, colorVerde, false);
            AplicarEstado(fila.grupoPeligroInicial, colorRojo, true);
            yield return new WaitForSeconds(tiempoConstante);

            // Transición Amarillo
            AplicarEstado(fila.grupoSeguroInicial, colorAmarillo, false);
            AplicarEstado(fila.grupoPeligroInicial, colorAmarillo, false);
            yield return StartCoroutine(TitileoGrupos(fila.grupoSeguroInicial, colorAmarillo, fila.grupoPeligroInicial, colorAmarillo));

            // Inversión (meow)
            AplicarEstado(fila.grupoSeguroInicial, colorRojo, true);
            AplicarEstado(fila.grupoPeligroInicial, colorVerde, false);
            yield return new WaitForSeconds(tiempoConstante);

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