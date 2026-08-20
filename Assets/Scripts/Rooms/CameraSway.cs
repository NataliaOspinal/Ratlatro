using UnityEngine;
using UnityEngine.InputSystem;

public class CameraSway : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float cantidadMovimiento = 0.5f; 
    public float suavidad=3f;

    private Vector3 posicionInicial;

    void Start()
    {
        posicionInicial=transform.position;
    }

    void Update()
    {
        if (Mouse.current == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        float mouseX=(mousePos.x/Screen.width)-0.5f;
        float mouseY=(mousePos.y/Screen.height)-0.5f;

      Vector3 posicionDestino = new Vector3(
            posicionInicial.x + (mouseX * cantidadMovimiento),
            posicionInicial.y + (mouseY * cantidadMovimiento),
            posicionInicial.z
        );
    
        transform.position = Vector3.Lerp(transform.position, posicionDestino, suavidad * Time.deltaTime);    }

}
