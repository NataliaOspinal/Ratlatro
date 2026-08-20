using UnityEngine;
using UnityEngine.SceneManagement; // cambio de escenas

public class MainMenuManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject creditsPopup;
    public GameObject quitButton; // Referencia para ocultarlo en WebGL aka para que no salga en el itch.io

    void Start()
    {
        // Ocultar el popup al iniciar (x si acaso)
        if (creditsPopup != null)
        {
            creditsPopup.SetActive(false);
        }

        // Si es un build de WebGL, desactiva el botón Quit
#if UNITY_WEBGL
        if (quitButton != null)
        {
            quitButton.SetActive(false);
        }
#endif
    }

    // Método para el botón Start
    public void PlayGame()
    {
        // Carga la escena por su nombre exacto
        SceneManager.LoadScene("01_Level1");
    }

    // Método para abrir los créditos
    public void ShowCredits()
    {
        if (creditsPopup != null)
        {
            creditsPopup.SetActive(true);
        }
    }

    // Método para el botón de cerrar los créditos
    public void HideCredits()
    {
        if (creditsPopup != null)
        {
            creditsPopup.SetActive(false);
        }
    }

    // Método para salir del juego
    public void QuitGame()
    {
        Application.Quit();
    }
}