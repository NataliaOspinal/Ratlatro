using UnityEngine;

public class DynamicVisual : MonoBehaviour
{
    // Variable para almacenar el SpriteRenderer del objeto
    public SpriteRenderer sr;

    // Sprites para el estado opaco y transparente
    public Sprite opaqueSprite;
    public Sprite transparentSprite;

    void Start()
    {
        // sprite renderer se asigna automáticamente si no se ha hecho desde el Inspector
        if (sr == null)
        {
            sr = GetComponentInChildren<SpriteRenderer>();
        }

        if (sr != null && opaqueSprite != null)
        {
            sr.sprite = opaqueSprite;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && sr != null)
        {
            // Comparamos la altura del jugador contra la base del cilindro (el padre)
            if (other.transform.position.y > transform.position.y)
            {
                if (transparentSprite != null) sr.sprite = transparentSprite;
            }
            else
            {
                if (opaqueSprite != null) sr.sprite = opaqueSprite;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && sr != null)
        {
            if (opaqueSprite != null) sr.sprite = opaqueSprite;
        }
    }
}