using UnityEngine;
using UnityEngine.UI; 

public class LuzDisco : MonoBehaviour
{
    [Header("Configuración del Panel Canvas")]
    public Image panelTransparente; 
    
    [Tooltip("Qué tan rápido cambia de color")]
    public float velocidadColor = 0.5f;
    
    [Tooltip("Qué tan transparente es el color (0 = invisible, 1 = sólido)")]
    [Range(0f, 1f)] 
    public float transparencia = 0.3f; 
    
    private float hue = 0f;

    void Update()
    {
        if (panelTransparente != null)
        {
            hue += velocidadColor * Time.deltaTime;
            if (hue > 1f) hue -= 1f;

            Color colorDisco = Color.HSVToRGB(hue, 1f, 1f);
            colorDisco.a = transparencia; 
            
            panelTransparente.color = colorDisco;
        }
    }
}