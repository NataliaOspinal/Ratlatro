using UnityEngine;

public class PipeTunnel : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        // Solo reacciona si es la rata pequeña
        if (other.GetComponent<CompanionPlayer>() != null)
        {
            // Buscamos el SpriteRenderer y lo apagamos
            SpriteRenderer sr = other.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.enabled = false;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<CompanionPlayer>() != null)
        {
            // Lo volvemos a encender al salir
            SpriteRenderer sr = other.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.enabled = true;
            }
        }
    }
}