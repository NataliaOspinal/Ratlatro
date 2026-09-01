using UnityEngine;

public class MenuPausaManager : MonoBehaviour
{
    [Header("Menú de Opciones")]
    public GameObject panelOpciones; 

    [Header("Elementos a Ocultar")]
    public GameObject[] elementosAOcultar; 

    private bool estaPausado = false;

    private void Start()
    {
        if (panelOpciones != null) panelOpciones.SetActive(false);
        Time.timeScale = 1f;
    }

    public void AlternarPausa()
    {
        if (estaPausado)
        {
            ReanudarJuego();
        }
        else
        {
            PausarJuego();
        }
    }

    public void PausarJuego()
    {
        estaPausado = true;
        
        Time.timeScale = 0f; 

        if (panelOpciones != null) panelOpciones.SetActive(true);

        foreach (GameObject elemento in elementosAOcultar)
        {
            if (elemento != null) elemento.SetActive(false);
        }
    }

    public void ReanudarJuego()
    {
        estaPausado = false;
        
        Time.timeScale = 1f;

        if (panelOpciones != null) panelOpciones.SetActive(false);

        foreach (GameObject elemento in elementosAOcultar)
        {
            if (elemento != null) elemento.SetActive(true);
        }
    }
}