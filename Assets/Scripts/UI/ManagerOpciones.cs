using UnityEngine;
using UnityEngine.UI;  
using UnityEngine.Audio; 

public class OpcionesManager : MonoBehaviour
{
    [Header("Pantalla")]
    public Toggle togglePantallaCompleta;

    [Header("Audio")]
    public AudioMixer mezcladorAudio;

    private void Start()
    {
        
        if (togglePantallaCompleta != null)
        {
            togglePantallaCompleta.isOn = Screen.fullScreen;
        }
    }


    public void ActivarPantallaCompleta(bool esPantallaCompleta)
    {
        Screen.fullScreen = esPantallaCompleta;
    }

    public void CambiarVolumenMusica(float volumen)
    {
        mezcladorAudio.SetFloat("VolMusica", Mathf.Log10(volumen) * 20);
    }

    public void CambiarVolumenEfectos(float volumen)
    {
        mezcladorAudio.SetFloat("VolEfectos", Mathf.Log10(volumen) * 20);
    }
}
