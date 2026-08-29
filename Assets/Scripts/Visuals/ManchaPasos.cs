using System.Collections;
using UnityEngine;

public class ManchaPasos : MonoBehaviour
{
    public float tiempoVisible = 0.5f; // Segundos que dura 100% visible
    public float tiempoDesvanecimiento = 1.5f; // Segundos que tarda en borrarse

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        StartCoroutine(RutinaDesvanecer());
    }

    private IEnumerator RutinaDesvanecer()
    {
        yield return new WaitForSeconds(tiempoVisible);

        float tiempoPasado = 0f;
        Color colorInicial = sr.color;

        while (tiempoPasado < tiempoDesvanecimiento)
        {
            tiempoPasado += Time.deltaTime;
            // Interpola el canal Alfa (transparencia) de 1 a 0
            float alfa = Mathf.Lerp(1f, 0f, tiempoPasado / tiempoDesvanecimiento);
            sr.color = new Color(colorInicial.r, colorInicial.g, colorInicial.b, alfa);
            yield return null;
        }

        // Destruye el objeto para liberar memoria
        Destroy(gameObject);
    }
}