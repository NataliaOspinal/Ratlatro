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

    private int rondaActual = 0; 
    private int pasoActual = 0;  
    
    [HideInInspector] 
    public bool esperandoJugador = false;

    void Start()
    {
        Invoke("EmpezarNuevaRonda", 2f); 
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
            MatarRata();
        }
    }

    void MatarRata()
    {
        MainPlayer rata = FindAnyObjectByType<MainPlayer>();
        if (rata != null)
        {
            Debug.Log("rip");
            rata.Morir(); 
        }
    }
}
