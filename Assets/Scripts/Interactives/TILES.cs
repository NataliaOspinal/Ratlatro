using UnityEngine;

public class TILES : MonoBehaviour
{
    [Header("Configuración")]
    public int id;
    public Color colorNormal = Color.white;
    public Color colorBrillo = Color.cyan;
    public Color colorPisado = Color.gray;

    private SpriteRenderer spriteRenderer;
    public ManagerSimon manager;

    private bool haSidoPisada = false; 

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = colorNormal;
    }

    public void Brillar() => spriteRenderer.color = colorBrillo;
    
    public void Apagar() 
    {
        spriteRenderer.color = colorNormal;
        haSidoPisada = false; 
    }

    public void Oscurecer() 
    {
        spriteRenderer.color = colorPisado;
        haSidoPisada = true; 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && manager.esperandoJugador && !haSidoPisada)
        {
            Oscurecer(); 
            manager.ComprobarPaso(id); 
        }
    }
}
