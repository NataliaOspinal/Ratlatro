using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RondaSimon
{
    public int[] rutaExacta;
}

public class ManagerSimon : MonoBehaviour
{
    [Header("Efectos de Sonido")]
    public AudioSource fuenteDeAudio;
    public AudioClip sonidoSwitch;
    public AudioClip sonidoRound;
    public AudioClip sonidoFinish;
    public AudioClip sonidoLight;

    [Header("Conexiones")]
    public TILES[] baldosas;

    [Header("Diseño de Niveles")]
    public RondaSimon[] rondas;
    public float velocidadLuces = 0.8f;

    [Header("Tiempos")]
    public float tiempoMaximoEspera = 10f; // Segundos antes de repetir
    private float temporizadorEspera = 0f;

    public GestorParedesAlternas gestorParedes;
    public TorretaElectrica torreta;

    private int rondaActual = 0;
    private int pasoActual = 0;
    private bool puzzleIniciado = false;
    private bool jugadorEnZona = false; // Detecta si la rata sigue ahí

    [HideInInspector]
    public bool esperandoJugador = false;

    void Awake()
    {
        foreach (var baldosa in baldosas)
        {
            if (baldosa != null) baldosa.manager = this;
        }
    }

    void Update()
    {
        // Solo contamos el tiempo si es el turno del jugador y sigue en la zona
        if (esperandoJugador && jugadorEnZona)
        {
            temporizadorEspera += Time.deltaTime;

            if (temporizadorEspera >= tiempoMaximoEspera)
            {
                RepetirSecuenciaActual();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorEnZona = true; // El jugador está cerca

            if (!puzzleIniciado)
            {
                puzzleIniciado = true;
                Invoke("EmpezarNuevaRonda", 1f);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Si la rata se va, pausamos el cronómetro
            jugadorEnZona = false;
            temporizadorEspera = 0f;
        }
    }

    public void EmpezarNuevaRonda()
    {
        if (rondaActual >= rondas.Length)
        {
            if (fuenteDeAudio != null && sonidoFinish != null)
            {
                fuenteDeAudio.PlayOneShot(sonidoFinish);
            }
            Debug.Log("Puzzle completado");
            if (gestorParedes != null) gestorParedes.AlternarGrupos();
            return;
        }

        PrepararYMostrarSecuencia();
    }

    private void RepetirSecuenciaActual()
    {
        Debug.Log("Jugador inactivo. Repitiendo secuencia...");
        PrepararYMostrarSecuencia();
    }

    // Unificamos la lógica de limpiar baldosas para usarla al empezar o al repetir
    private void PrepararYMostrarSecuencia()
    {
        esperandoJugador = false;
        pasoActual = 0; // Borramos el progreso de esta ronda
        temporizadorEspera = 0f; // Reiniciamos el reloj

        foreach (var baldosa in baldosas)
        {
            baldosa.Apagar(); // Regresa todo a su color original
        }

        StartCoroutine(MostrarSecuencia());
    }

    IEnumerator MostrarSecuencia()
    {
        yield return new WaitForSeconds(1f);

        int[] rutaDeEstaRonda = rondas[rondaActual].rutaExacta;

        foreach (int idEnSecuencia in rutaDeEstaRonda)
        {
            baldosas[idEnSecuencia].Brillar();
            if (fuenteDeAudio != null && sonidoLight != null)
            {
                fuenteDeAudio.PlayOneShot(sonidoLight);
            }
            yield return new WaitForSeconds(velocidadLuces);
            baldosas[idEnSecuencia].Apagar();
            yield return new WaitForSeconds(velocidadLuces * 0.5f);
        }

        esperandoJugador = true;
        temporizadorEspera = 0f; // El reloj empieza a correr justo al terminar las luces
    }

    public void ComprobarPaso(int idPisado)
    {
        if (!esperandoJugador) return;

        int[] rutaDeEstaRonda = rondas[rondaActual].rutaExacta;

        if (idPisado == rutaDeEstaRonda[pasoActual])
        {
            if (fuenteDeAudio != null && sonidoSwitch != null)
            {
                fuenteDeAudio.PlayOneShot(sonidoSwitch);
            }

            pasoActual++;
            temporizadorEspera = 0f; // Cada vez que pisa bien, le damos otros 10 segundos

            if (pasoActual >= rutaDeEstaRonda.Length)
            {
                esperandoJugador = false;
                if (fuenteDeAudio != null && sonidoRound != null)
                {
                    fuenteDeAudio.PlayOneShot(sonidoRound);
                }
                Debug.Log("Ronda superada");

                rondaActual++;
                Invoke("EmpezarNuevaRonda", 1.5f);
            }
        }
        else
        {
            esperandoJugador = false;

            if (torreta != null)
            {
                torreta.ActivarTrampa();
            }
        }
    }
}