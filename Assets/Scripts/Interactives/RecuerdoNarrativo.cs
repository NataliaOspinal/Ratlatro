using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class RecuerdoNarrativo : MonoBehaviour
{
    [Header("UI Narrativa")]
    public GameObject panelNubeNegra;
    public TextMeshProUGUI textoNarrativa;

    [TextArea(2, 4)]
    public string[] lineasDialogo = {
        "linea1",
        "linea2"
    };
    public float velocidadEscritura = 0.05f;

    // Lista de puertas que se abrirán al finalizar el diálogo
    public List<Door> puertasParaAbrir;

    private bool yaRecogido = false;

    private void Start()
    {
        if (panelNubeNegra != null) panelNubeNegra.SetActive(false);
        if (textoNarrativa != null) textoNarrativa.text = "";
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !yaRecogido)
        {
            MainPlayer jugador = collision.GetComponent<MainPlayer>();

            if (jugador != null)
            {
                yaRecogido = true;

                // Solo ocultamos el recuerdo, ya NO otorgamos el poder
                SpriteRenderer sr = GetComponent<SpriteRenderer>();
                if (sr != null) sr.enabled = false;

                Collider2D col = GetComponent<Collider2D>();
                if (col != null) col.enabled = false;

                StartCoroutine(RutinaDialogoRecuerdo(jugador));
            }
        }
    }

    private IEnumerator RutinaDialogoRecuerdo(MainPlayer scriptRata)
    {
        if (RoomManager.Instance != null) RoomManager.Instance.interaccionBloqueada = true;
        if (scriptRata != null) scriptRata.canMove = false;
        if (panelNubeNegra != null) panelNubeNegra.SetActive(true);

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
        if (panelNubeNegra != null) panelNubeNegra.SetActive(false);
        if (RoomManager.Instance != null) RoomManager.Instance.interaccionBloqueada = false;
        if (scriptRata != null) scriptRata.canMove = true;

        // Abre puertas al finalizar el diálogo
        AbrirPuertas();

        Destroy(gameObject);
    }

    private void AbrirPuertas()
    {
        foreach (Door puerta in puertasParaAbrir)
        {
            if (puerta != null)
            {
                // EO
                puerta.Abrir();
            }
        }
    }
}