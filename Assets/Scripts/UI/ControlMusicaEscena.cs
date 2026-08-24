using UnityEngine;

public class ControladorMusicaEscena : MonoBehaviour
{
    [Header("Música de esta zona")]
    public AudioClip musicaDeEstaEscena;

    private void Start()
    {
        if (AudioPersistente.instancia != null)
        {
            AudioPersistente.instancia.CambiarMusica(musicaDeEstaEscena);
        }
    }
}