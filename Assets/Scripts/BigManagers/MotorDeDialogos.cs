using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;

public class MotorDeDialogos : MonoBehaviour
{
    // Escritura y audio
    public IEnumerator EscribirLetraPorLetra(TextMeshProUGUI textoUI, string texto, float velocidadEscritura, AudioSource fuenteAudio, AudioClip sfxDialogo)
    {
        if (fuenteAudio != null && sfxDialogo != null)
        {
            fuenteAudio.clip = sfxDialogo;
            fuenteAudio.Play();
        }

        textoUI.text = texto;
        textoUI.maxVisibleCharacters = 0;
        textoUI.ForceMeshUpdate();
        int totalCaracteres = textoUI.textInfo.characterCount;

        yield return null;

        for (int i = 0; i <= totalCaracteres; i++)
        {
            textoUI.maxVisibleCharacters = i;
            float cronometro = 0f;
            bool saltoDetectado = false;

            while (cronometro < velocidadEscritura)
            {
                cronometro += Time.deltaTime;
                if (BotonSaltarPresionado())
                {
                    saltoDetectado = true;
                    break;
                }
                yield return null;
            }

            if (saltoDetectado)
            {
                textoUI.maxVisibleCharacters = totalCaracteres;
                break;
            }
        }

        if (fuenteAudio != null) fuenteAudio.Stop();
        yield return null;
    }

    // Manejo de Dotween y cambio de sprite
    public Sprite ActualizarYAnimarSprite(Image imagenUI, Sprite nuevoSprite, Sprite spriteAnterior, RectTransform rect, Vector2 posOriginal, float fuerzaSalto, float duracionSalto)
    {
        if (imagenUI == null) return null;

        if (nuevoSprite != null)
        {
            imagenUI.sprite = nuevoSprite;
            imagenUI.gameObject.SetActive(true);

            if (rect != null)
            {
                DOTween.Kill(rect);
                rect.anchoredPosition = posOriginal;
                rect.DOPunchAnchorPos(new Vector2(0, fuerzaSalto), duracionSalto, 1, 0.5f);
            }
            
            return spriteAnterior;
        }
        else
        {
            imagenUI.gameObject.SetActive(false);
            return null;
        }
    }

    // Revisión de inputs
    public bool BotonSaltarPresionado()
    {
        return (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame);
    }
}