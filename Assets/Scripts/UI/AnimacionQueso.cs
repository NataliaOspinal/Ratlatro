using UnityEngine;
using DG.Tweening; 

public class AnimacionQueso : MonoBehaviour
{
    [Header("Referencia")]
    public RectTransform quesoTransform;

    [Header("Sonido")]
    public AudioSource reproductorAudio;
    public AudioClip sfxSquish;

    private Vector3 escalaOriginal; 

    private void Start()
    {
        if (quesoTransform != null)
        {
            escalaOriginal = quesoTransform.localScale;
        }
    }

    public void HacerSquish()
    {
        if (quesoTransform != null)
        {
            quesoTransform.DOKill(true);
            quesoTransform.localScale = escalaOriginal; 

            Vector3 fuerzaRebote = new Vector3(
                escalaOriginal.x * 0.2f,  
                escalaOriginal.y * -0.2f, 
                0f
            );

            quesoTransform.DOPunchScale(fuerzaRebote, 0.4f, 6, 1f);

            if (reproductorAudio != null && sfxSquish != null)
            {
                reproductorAudio.PlayOneShot(sfxSquish);
            }
        }
    }
}