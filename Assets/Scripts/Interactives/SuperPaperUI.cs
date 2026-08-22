using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SuperPaperUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static SuperPaperUI Instance;

    [Header("Referencias")]
    public Image imagenOutline;
    public TMP_Text textoPapel;

    [Header("Posiciones")]
    public Transform puntoInicial;
    public Transform puntoFinal;
    public float suavidadMovimiento = 8f;

    private bool mouseEncima=false;

    private void Awake()
    {
        if(Instance==null) Instance=this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }


    private void Update()
    {
        Transform destino = mouseEncima ? puntoFinal : puntoInicial;

        transform.position = Vector3.Lerp(transform.position, destino.position, suavidadMovimiento * Time.deltaTime);
    
    }

    public void MostrarPapel(string nuevoTexto)
    {
        if(textoPapel != null) textoPapel.text = nuevoTexto;

        transform.position = puntoInicial.position;
        mouseEncima = false; 
        
        if (imagenOutline != null) imagenOutline.enabled = true;

        gameObject.SetActive(true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseEncima=true;
        if (imagenOutline != null) imagenOutline.enabled = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseEncima = false;
        
    }




}
