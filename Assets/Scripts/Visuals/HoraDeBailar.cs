using UnityEngine;

public class HoraDeBailar : MonoBehaviour
{
    [Header("Ritmo ")]
    public float bpm = 120f; 
    
    [Header("Deformación")]
    public float intensidad = 0.15f; 

    private Vector3 escalaOriginal;

    void Start()
    {
        escalaOriginal = transform.localScale;
    }

    void Update()
    {
        float beatsPorSegundo = bpm / 60f;
        
        float rebote = Mathf.Sin(Time.time * beatsPorSegundo * Mathf.PI * 2f);

        float escalaY = escalaOriginal.y + (rebote * intensidad);
        float escalaX = escalaOriginal.x - (rebote * intensidad * 0.5f);

        transform.localScale = new Vector3(escalaX, escalaY, escalaOriginal.z);
    }
}
