using UnityEngine;

public class TransparentWall : MonoBehaviour
{
    [Tooltip("Nivel de transparencia (0 = invisible, 1 = sólido)")]
    public float alphaTransparente = 0.4f;

    private SpriteRenderer spriteRenderer;
    private Color colorOriginal;

    void Start()
    {
        // Busca el SpriteRenderer en los hijos eo
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            colorOriginal = spriteRenderer.color;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && spriteRenderer != null)
        {
            // Si la rata está más arriba en Y, significa que está por detrás
            if (other.transform.position.y > transform.position.y)
            {
                Color nuevoColor = colorOriginal;
                nuevoColor.a = alphaTransparente;
                spriteRenderer.color = nuevoColor;
            }
            else
            {
                // Si está por delante la pared vuelve a ser sólida
                spriteRenderer.color = colorOriginal;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && spriteRenderer != null)
        {
            // Restaura la opacidad al salir del área
            spriteRenderer.color = colorOriginal;
        }
    }
}