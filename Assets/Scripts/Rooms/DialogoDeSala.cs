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

        // Buscamos a la rata y la congelamos
        MainPlayer scriptRata = FindAnyObjectByType<MainPlayer>();
        if (scriptRata != null) scriptRata.canMove = false;

        if (panelNarracion != null) panelNarracion.SetActive(true);

        foreach (string linea in lineasDialogo)
        {
            if (textoNarrativa == null) continue;

            textoNarrativa.text = linea;
            textoNarrativa.maxVisibleCharacters = 0;
            textoNarrativa.ForceMeshUpdate();
            int totalCaracteres = textoNarrativa.textInfo.characterCount;

            yield return null;

            for (int i = 0; i <= totalCaracteres; i++)
            {
                textoNarrativa.maxVisibleCharacters = i;

                float cronometro = 0f;
                bool saltoDetectado = false;

                while (cronometro < velocidadEscritura)
                {
                    cronometro += Time.deltaTime;

                    if ((Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) ||
                        (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame))
                    {
                        saltoDetectado = true;
                        break;
                    }
                    
                    yield return null; 
                }

                if (saltoDetectado)
                {
                    textoNarrativa.maxVisibleCharacters = totalCaracteres;
                    break; 
                }
            }

            yield return null; 

            yield return new WaitUntil(() => 
                (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) ||
                (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
            );

            yield return null;
        }

        if (textoNarrativa != null) textoNarrativa.text = "";
        if (panelNarracion != null) panelNarracion.SetActive(false);

        if (RoomManager.Instance != null)
        {
            RoomManager.Instance.interaccionBloqueada = false;
        }

        if (scriptRata != null) scriptRata.canMove = true;

        Destroy(gameObject);
    }
}
