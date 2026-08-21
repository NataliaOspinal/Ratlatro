using UnityEngine;

public class PushableBlock : MonoBehaviour
{
    public enum PusherType
    {
        Cualquiera,
        SoloRataGrande,
        SoloRataPequena
    }

    //Empuje
    public PusherType quienPuedeEmpujar = PusherType.Cualquiera;

    private Rigidbody2D rb;
    private float normalMass = 1f; //Peso empujable
    private float heavyMass = 10000f; //Peso que lo ancla al piso

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.mass = heavyMass; // Empezamos anclados al piso
    }

    // Mientras la rata correcta esté tocando el bloque
    void OnCollisionStay2D(Collision2D collision)
    {
        if (CanPush(collision.collider))
        {
            rb.mass = normalMass; // Lo volvemos ligero
        }
    }

    // Cuando la rata se aleja del bloque
    void OnCollisionExit2D(Collision2D collision)
    {
        if (CanPush(collision.collider))
        {
            rb.mass = heavyMass; // Lo volvemos imposible de mover
            rb.linearVelocity = Vector2.zero; // Frenamos cualquier inercia
        }
    }

    private bool CanPush(Collider2D other)
    {
        if (!other.CompareTag("Player")) return false;

        switch (quienPuedeEmpujar)
        {
            case PusherType.Cualquiera:
                return true;
            case PusherType.SoloRataGrande:
                return other.GetComponent<MainPlayer>() != null;
            case PusherType.SoloRataPequena:
                return other.GetComponent<CompanionPlayer>() != null;
            default:
                return false;
        }
    }
}