using System.Collections;
using UnityEngine;

public class AnimacionConEspera : MonoBehaviour
{
    [Header("Referencias")]
    public Animator miAnimator;
    
    [Header("Configuración")]
    public float segundosDeEspera = 5f;
    public string nombreDelTrigger = "HacerAnim";

    private void Start()
    {
        StartCoroutine(RutinaDeEspera());
    }

    private IEnumerator RutinaDeEspera()
    {
        while (true)
        {   
           
            yield return new WaitForSeconds(segundosDeEspera);
            
            if (miAnimator != null)
            {
                miAnimator.SetTrigger(nombreDelTrigger);
            }

            
        }
    }
}