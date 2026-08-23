using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class RecuerdoPowerUp : MonoBehaviour
{
    [Header("UI Narrativa (Como en la cinemática)")]
    public GameObject panelNubeNegra;
    public TextMeshProUGUI textoNarrativa;
    
    [TextArea(2, 4)]
    public string[] lineasDialogo = {
        "linea1",
        "linea2"
    };
    public float velocidadEscritura = 0.05f;

    private bool yaRecogido = false;

    private void Start()
    {
        if (panelNubeNegra != null)
        {
            panelNubeNegra.SetActive(false);
        }
        
        if (textoNarrativa != null)
        {
            textoNarrativa.text = "";
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !yaRecogido)
        {
            MainPlayer jugador = collision.GetComponent<MainPlayer>();

            if (jugador != null)
            {
                yaRecogido = true; 

                // 1. Desbloqueamos el compañero
                jugador.DesbloquearCompanero();
                Debug.Log("¡Poder desbloqueado!");

                SpriteRenderer sr = GetComponent<SpriteRenderer>();
                if (sr != null) sr.enabled = false;
                
                Collider2D col = GetComponent<Collider2D>();
                if (col != null) col.enabled = false;

                StartCoroutine(RutinaDialogoRecuerdo());
            }
        }
    }

    private IEnumerator RutinaDialogoRecuerdo()
    {
        if (RoomManager.Instance != null)
        {
            RoomManager.Instance.interaccionBloqueada = true;
        }

        if (panelNubeNegra != null) panelNubeNegra.SetActive(true);

        foreach (string linea in lineasDialogo)
        {
            textoNarrativa.text = "";
            
            foreach (char letra in linea.ToCharArray())
            {
                textoNarrativa.text += letra;
                yield return new WaitForSeconds(velocidadEscritura);
            }

            yield return new WaitUntil(() => 
                (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) ||
                (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
            );
        }

        if (textoNarrativa != null) textoNarrativa.text = "";
        if (panelNubeNegra != null) panelNubeNegra.SetActive(false);

        if (RoomManager.Instance != null)
        {
            RoomManager.Instance.interaccionBloqueada = false;
        }

        Destroy(gameObject);
    }
}