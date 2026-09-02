using UnityEngine;

public class MusicaDisco : MonoBehaviour
{
    [Header("Música de esta Sala (Local)")]
    public AudioSource fuenteMusicaDisco;          

    private AudioSource fuenteMusicaGlobal; 
    private int ratasEnLaSala = 0;

    void Start()
    {
        GameObject objetoMusicaGlobal = GameObject.Find("AudioMusica");
        
        if (objetoMusicaGlobal != null)
        {
            fuenteMusicaGlobal = objetoMusicaGlobal.GetComponent<AudioSource>();
        }
        else
        {
            Debug.LogWarning("No se encontró el objeto 'AudioMusica' en la escena.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ratasEnLaSala++;
            
            if (ratasEnLaSala == 1)
            {
                if (fuenteMusicaGlobal != null) fuenteMusicaGlobal.Pause();
                if (fuenteMusicaDisco != null) fuenteMusicaDisco.Play();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ratasEnLaSala--;
            
            if (ratasEnLaSala <= 0)
            {
                ratasEnLaSala = 0;
                
                if (fuenteMusicaDisco != null) fuenteMusicaDisco.Stop();
                if (fuenteMusicaGlobal != null) fuenteMusicaGlobal.UnPause();
            }
        }
    }
}
