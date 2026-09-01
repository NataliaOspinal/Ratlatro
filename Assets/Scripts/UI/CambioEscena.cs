using UnityEngine;
using UnityEngine.SceneManagement; 

public class CambioDeEscena : MonoBehaviour
{
    [Header("Configuración de Escenas")]
    public string nombreEscena = "99_Credits"; 
    
    public string nombrePrimerNivel = "01_Zona1"; 

    public void BotonContinuar()
    {
        
        string nivelACargar = PlayerPrefs.GetString("NivelGuardado", nombrePrimerNivel);
        
        SceneManager.LoadScene(nivelACargar);
    }

    public void BotonNuevaPartida()
    {
        PlayerPrefs.DeleteKey("NivelGuardado");
        PlayerPrefs.DeleteKey("SiguienteZona");
        PlayerPrefs.DeleteKey("MitosisDesbloqueada");

        SceneManager.LoadScene(nombrePrimerNivel);
    }

    public void IrACreditos()
    {
        SceneManager.LoadScene(nombreEscena);
    }
}