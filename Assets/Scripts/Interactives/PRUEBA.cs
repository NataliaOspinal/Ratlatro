using UnityEngine;
using UnityEngine.InputSystem; 

public class TestPuerta : MonoBehaviour
{
    [Header("Configuración")]
    public string nombreTrigger = "Abrir"; 

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
        {
            if (anim != null)
            {
                anim.SetTrigger(nombreTrigger);
            }
        }
    }

}