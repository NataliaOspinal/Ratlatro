using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        
        #else
            Application.Quit();
        #endif
    }
}