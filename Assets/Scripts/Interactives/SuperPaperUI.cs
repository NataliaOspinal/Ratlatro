using System.Collections;
using TMPro;
using UnityEngine;

public class SuperPaperUI : MonoBehaviour
{
    public static SuperPaperUI Instance;

    [Header("Referencias")]
    public GameObject outlineVerde;
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
        StopAllCoroutines();

        if(textoPapel!=null) textoPapel.text=nuevoTexto;

        transform.position=puntoInicial.position;
        outlineVerde.SetActive(true);
        mouseEncima=false;

        if (outlineVerde != null) outlineVerde.SetActive(true);

        gameObject.SetActive(true);
    }

    private void OnMouseEnter()
    {
        mouseEncima=true;
        if (outlineVerde != null) outlineVerde.SetActive(false);
    }

    private void OnMouseExit()
    {
        mouseEncima = false;
        
    }




}
