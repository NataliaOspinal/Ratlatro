using UnityEngine;
using UnityEngine.InputSystem;

public class CameraSway : MonoBehaviour
{
    [Header("Referencias")]
    public Transform jugador;

    [Header("Configuración de Movimiento")]
    public float cantidadMovimiento = 0.5f; 
    public float suavidad=3f;

    [Header("Configuracion de Zoom")]
    public float zoomNormal=10f;
    public float zoomAcercado=6f;
    public float suavidadZoom=5f;

    [Range(0f,1f)]
    public float fuerzaEnfoque=1f;

    private Vector3 posicionInicial;
    private Camera cam;

    void Start()
    {
        posicionInicial=transform.position;
        cam = GetComponent<Camera>();

        if (cam != null) zoomNormal = cam.orthographicSize;
    }

    void Update()
    {
        if (Mouse.current == null || Keyboard.current == null || cam == null) return;
        bool estaAcercado = Keyboard.current.leftShiftKey.isPressed;

        float zoomDestino = estaAcercado ? zoomAcercado : zoomNormal;
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, zoomDestino, suavidadZoom * Time.deltaTime);

        Vector3 centroDestino = posicionInicial;

        if (estaAcercado)
        {
            if (jugador != null)
            {
               centroDestino = new Vector3(jugador.position.x, jugador.position.y, posicionInicial.z);
            }
            else
            {
                Debug.LogWarning("Falta rata");
            }
        }


        Vector2 mousePos = Mouse.current.position.ReadValue();
        float mouseX=(mousePos.x/Screen.width)-0.5f;
        float mouseY=(mousePos.y/Screen.height)-0.5f;

      Vector3 posicionDestino = new Vector3(
            centroDestino.x + (mouseX * cantidadMovimiento),
            centroDestino.y + (mouseY * cantidadMovimiento),
            posicionInicial.z
        );
    
        transform.position = Vector3.Lerp(transform.position, posicionDestino, suavidad * Time.deltaTime);    }

}
