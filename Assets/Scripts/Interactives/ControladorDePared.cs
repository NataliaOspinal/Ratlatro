using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Collider2D))]
public class ControladorDePared : MonoBehaviour
{
    //Visuales
    [Tooltip("Arrastra aquí el sprite de la pared (limpia, con slime, etc.)")]
    public Sprite spriteSeleccionado;

    // Referencia al objeto visual que contiene el SpriteRenderer
    public Transform objetoVisual;

    // Bandera para determinar si el obstáculo es hundible o no
    public bool esHundible = false;

    private SpriteRenderer sr;

    // Usamos Update en lugar de OnValidate para evitar el error de Unity
    private void Update()
    {
        // Esto solo se ejecuta en la pestaña Scene del Editor, nunca durante el juego
        if (!Application.isPlaying)
        {
            if (objetoVisual != null && spriteSeleccionado != null)
            {
                // Obtenemos el componente una sola vez para no sobrecargar el editor
                if (sr == null) sr = objetoVisual.GetComponent<SpriteRenderer>();

                if (sr != null && sr.sprite != spriteSeleccionado)
                {
                    sr.sprite = spriteSeleccionado;
                }
            }
        }
    }

    private void Awake()
    {
        // Cuando arranca el juego, solo configuramos si se hunde o no
        if (Application.isPlaying)
        {
            ObstaculoHundible scriptHundible = GetComponent<ObstaculoHundible>();
            if (scriptHundible != null)
            {
                scriptHundible.enabled = esHundible;
            }
        }
    }
}