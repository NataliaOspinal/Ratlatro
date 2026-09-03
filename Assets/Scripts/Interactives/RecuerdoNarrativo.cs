using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI; 
using DG.Tweening;

[System.Serializable]
public class LineaRecuerdo 
{
    [TextArea(2, 4)]
    public string texto;
    public Sprite spriteRata; 
}

public class RecuerdoNarrativo : MonoBehaviour
{
    [Header("UI Narrativa")]
    public GameObject panelNubeNegra;
    public TextMeshProUGUI textoNarrativa;

    [Header("UI Rata")]
    public Image imagenRataUI;
    public float fuerzaSalto = 20f;
    public float duracionSalto = 0.3f;

    [Header("Diálogo")]
    public LineaRecuerdo[] lineasDialogo;
    public float velocidadEscritura = 0.05f;

    public List<Door> puertasParaAbrir;

    private bool yaRecogido = false;

    private RectTransform rectRata;
    private Vector2 posOriginalRata;

    private void Start()
    {
        if (panelNubeNegra != null) panelNubeNegra.SetActive(false);
        if (textoNarrativa != null) textoNarrativa.text = "";

        if (imagenRataUI != null)
        {
            rectRata = imagenRataUI.GetComponent<RectTransform>();
            posOriginalRata = rectRata.anchoredPosition;
            imagenRataUI.gameObject.SetActive(false);
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

        Sprite spriteAnterior = null;

        foreach (LineaRecuerdo linea in lineasDialogo)
        {
            if (textoNarrativa == null) continue;

            if (imagenRataUI != null)
            {
                if (linea.spriteRata != null)
                {
                    imagenRataUI.sprite = linea.spriteRata;
                    imagenRataUI.gameObject.SetActive(true);

                    if (linea.spriteRata != spriteAnterior)
                    {
                        if (rectRata != null)
                        {
                            rectRata.DOKill(true); 
                            rectRata.anchoredPosition = posOriginalRata; 
                            rectRata.DOPunchAnchorPos(new Vector2(0, fuerzaSalto), duracionSalto, 1, 0.5f);
                        }
                        spriteAnterior = linea.spriteRata;
                    }
                }
                else
                {
                    imagenRataUI.gameObject.SetActive(false);
                    spriteAnterior = null;
                }
            }

            textoNarrativa.text = linea.texto;
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
                    if ((Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame))
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
                (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
            );
            yield return null;
        }

        if (textoNarrativa != null) textoNarrativa.text = "";
        if (panelNubeNegra != null) panelNubeNegra.SetActive(false);
        if (RoomManager.Instance != null) RoomManager.Instance.interaccionBloqueada = false;
        if (scriptRata != null) scriptRata.canMove = true;

        AbrirPuertas();

        Destroy(gameObject);
    } 

    private void AbrirPuertas()
    {
        foreach (Door puerta in puertasParaAbrir)
        {
            if (puerta != null)
            {
                puerta.Abrir();
            }
        }
    }
}