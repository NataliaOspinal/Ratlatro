using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class VentiladorPared : MonoBehaviour
{
    public float fuerzaBase = 500f;
    public float distanciaMaximaViento = 40f;
    public float multiplicadorRataPequena = 2.0f;
    public Transform objetivoDireccion;

    // Capa de obstáculos para comprobar línea de visión
    public LayerMask capaObstaculos;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (objetivoDireccion == null) return;

        bool esRataGrande = collision.GetComponent<MainPlayer>() != null;
        bool esRataPequena = collision.GetComponent<CompanionPlayer>() != null;

        if (esRataGrande || esRataPequena)
        {
            // Trazamos un vector exacto desde el ventilador hasta el centro de la rata
            Vector2 direccionHaciaRata = collision.transform.position - transform.position;
            float distanciaActual = direccionHaciaRata.magnitude;

            // Lanzamos el rayo para comprobar línea de visión
            RaycastHit2D impacto = Physics2D.Raycast(transform.position, direccionHaciaRata.normalized, distanciaActual, capaObstaculos);

            // Si el rayo choca con la capa de obstáculos antes de tocar a la rata, cortamos el viento
            if (impacto.collider != null)
            {
                return;
            }

            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                float intensidad = Mathf.Clamp01(1f - (distanciaActual / distanciaMaximaViento));
                float fuerzaFinal = fuerzaBase * intensidad;
                if (esRataPequena) fuerzaFinal *= multiplicadorRataPequena;

                Vector2 direccionViento = (objetivoDireccion.position - transform.position).normalized;
                rb.AddForce(direccionViento * fuerzaFinal, ForceMode2D.Force);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (objetivoDireccion != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, objetivoDireccion.position);
        }
    }
}