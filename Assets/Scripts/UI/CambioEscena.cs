using UnityEngine;
using UnityEngine.SceneManagement; // ¡Vital para manejar escenas!

public class CambioDeEscena : MonoBehaviour
{
    public string nombreEscenaCreditos = "99_Credits"; 

    public void IrACreditos()
    {
        SceneManager.LoadScene(nombreEscenaCreditos);
    }
}