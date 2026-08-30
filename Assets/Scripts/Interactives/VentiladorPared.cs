using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class VentiladorPared : MonoBehaviour
{
    // Configuración de la fuerza del viento
    public float fuerzaBase = 30f;
    // Distancia máxima a la que el viento tiene efecto (en unidades de Unity)
    public float distanciaMaximaViento = 5f;

    // Físicas de la Rata Pequeña
    // Multiplicador de fuerza para simular que la rata pequeña pesa menos
    public float multiplicadorRataPequena = 2.0f;

    //Dirección hacia la que sopla el viento
    //Crear un objeto vacío en la escena y colocarlo en la dirección deseada, luego asignarlo a esta variable
    public Transform objetivoDireccion;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (objetivoDireccion == null) return;

        bool esRataGrande = collision.GetComponent<MainPlayer>() != null;
        bool esRataPequena = collision.GetComponent<CompanionPlayer>() != null;

        if (esRataGrande || esRataPequena)
        {
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Calculamos la proximidad (1 = pegado al ventilador, 0 = al borde de la zona)
                float distanciaActual = Vector2.Distance(transform.position, collision.transform.position);
                float intensidad = Mathf.Clamp01(1f - (distanciaActual / distanciaMaximaViento));

                // Aplicamos la fuerza base y el multiplicador de masa simulada
                float fuerzaFinal = fuerzaBase * intensidad;
                if (esRataPequena) fuerzaFinal *= multiplicadorRataPequena;

                // Empujamos en la dirección isométrica configurada
                Vector2 direccion = (objetivoDireccion.position - transform.position).normalized;
                rb.AddForce(direccion * fuerzaFinal, ForceMode2D.Force);
            }
        }
    }

    // Dibuja una línea celeste en el editor para se vea exactamente hacia dónde sopla
    private void OnDrawGizmosSelected()
    {
        if (objetivoDireccion != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, objetivoDireccion.position);
        }
    }
}