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

    public GestorParedesAlternas gestorParedes;
    public TorretaElectrica torreta;

    private int rondaActual = 0; 
    private int pasoActual = 0;
    private bool puzzleIniciado = false;
    [HideInInspector] 
    public bool esperandoJugador = false;

    void Awake()
    {
        // El gestor se presenta a sí mismo exclusivamente ante sus propias baldosas
        foreach (var baldosa in baldosas)
        {
            if (baldosa != null) baldosa.manager = this;
        }
    }
    void Start()
    {
        //Invoke("EmpezarNuevaRonda", 2f); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si el jugador entra a la zona y el puzzle está apagado, lo arrancamos
        if (!puzzleIniciado && collision.CompareTag("Player"))
        {
            puzzleIniciado = true;
            // Le damos 1 segundo de cortesía para que la rata termine de acercarse antes de brillar
            Invoke("EmpezarNuevaRonda", 1f);
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

        esperandoJugador = false;
        pasoActual = 0;
        
        foreach(var baldosa in baldosas) 
        {
            baldosa.Apagar();
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
            if (fuenteDeAudio != null && sonidoLight!= null)
                {
                    fuenteDeAudio.PlayOneShot(sonidoLight);
                } 
            yield return new WaitForSeconds(velocidadLuces);
            baldosas[idEnSecuencia].Apagar();
            yield return new WaitForSeconds(velocidadLuces * 0.5f); 
        }

        esperandoJugador = true; 
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

            // En lugar de matar instantáneamente, encendemos la trampa
            if (torreta != null)
            {
                torreta.ActivarTrampa();
            }
        }
    }
}
