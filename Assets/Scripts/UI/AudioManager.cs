using UnityEngine;

public class AudioPersistente : MonoBehaviour
{
    public static AudioPersistente instancia;

    [Header("Referencias")]
    public AudioSource reproductorMusica; 

    private void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CambiarMusica(AudioClip nuevaCancion)
    {
        if (reproductorMusica == null || nuevaCancion == null) return;

        if (reproductorMusica.clip == nuevaCancion) return;

        // 3. Cambiamos la canción y le damos a Play
        reproductorMusica.Stop();
        reproductorMusica.clip = nuevaCancion;
        reproductorMusica.Play();
    }
}