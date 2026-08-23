using UnityEngine;

public class TransparentWall : MonoBehaviour
{
    //config
    [Tooltip("Desmarca esta casilla si quieres que esta pared siempre sea sólida")]
    public bool permiteTransparencia = true;

    [Tooltip("Nivel de transparencia (0 = invisible, 1 = sólido)")]
    public float alphaTransparente = 0.4f;

    private SpriteRenderer spriteRenderer;
    private Color colorOriginal;

    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            colorOriginal = spriteRenderer.color;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        // Si el interruptor está apagado, cancelamos la función inmediatamente
        if (!permiteTransparencia) return;

        if (other.CompareTag("Player") && spriteRenderer != null)
        {
            if (other.transform.position.y > transform.position.y)
            {
                Color nuevoColor = colorOriginal;
                nuevoColor.a = alphaTransparente;
                spriteRenderer.color = nuevoColor;
            }
            else
            {
                spriteRenderer.color = colorOriginal;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Si el interruptor está apagado, cancelamos la función inmediatamente
        if (!permiteTransparencia) return;

        if (other.CompareTag("Player") && spriteRenderer != null)
        {
            spriteRenderer.color = colorOriginal;
        }
    }
}