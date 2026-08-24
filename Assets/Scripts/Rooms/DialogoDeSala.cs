using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogoDeSala : MonoBehaviour
{
    [Header("UI Narrativa")]
    public GameObject panelNarracion; 
    public TextMeshProUGUI textoNarrativa;
    
    [Header("Historia de la Sala")]
    [TextArea(2, 4)]
    public string[] lineasDialogo = {
        "linea1",
        "linea2"
    };
    public float velocidadEscritura = 0.05f;
    
    public float esperaInicial = 0.5f; 

    private void Start()
    {
        if (panelNarracion != null)
        {
            panelNarracion.SetActive(false);
        }

        StartCoroutine(RutinaDialogoEntrada());
    }

    private IEnumerator RutinaDialogoEntrada()
    {
        yield return new WaitForSeconds(esperaInicial);

        if (RoomManager.Instance != null)
        {
            RoomManager.Instance.interaccionBloqueada = true;
        }

        if (panelNarracion != null) panelNarracion.SetActive(true);

        foreach (string linea in lineasDialogo)
        {
            if (textoNarrativa != null) textoNarrativa.text = "";
            
            foreach (char letra in linea.ToCharArray())
            {
                if (textoNarrativa != null) textoNarrativa.text += letra;
                yield return new WaitForSeconds(velocidadEscritura);
            }

            yield return null; 

            yield return new WaitUntil(() => 
                (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) ||
                (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
            );
        }

        if (textoNarrativa != null) textoNarrativa.text = "";
        if (panelNarracion != null) panelNarracion.SetActive(false);

        // 8. Devolvemos el control al jugador
        if (RoomManager.Instance != null)
        {
            RoomManager.Instance.interaccionBloqueada = false;
        }

        Destroy(gameObject);
    }
}
