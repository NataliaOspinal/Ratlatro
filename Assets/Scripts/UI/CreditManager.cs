using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsManager : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Arrastra aquí el objeto padre que contiene todos tus textos e imágenes")]
    [SerializeField] RectTransform creditosTransform; 
    [SerializeField] float startingY = -1000f; 
    [SerializeField] float endingY = 2500f;    
    [SerializeField] float timeToMove = 1f;

    [Header("Animation")]
    [SerializeField] float movementSpeed = 150f;
    [SerializeField] float clickMovementMultiplier = 4f; 
    float multiplier;

    [Header("Ending")]
    [SerializeField] string mainMenuSceneName = "00_MainMenu";
    [SerializeField] GameObject clickToLeave; 

    Vector2 actualPos;
    bool canMove = false;

    void Start()
    {
        if (clickToLeave != null) 
            clickToLeave.SetActive(false);

        Vector2 pos = creditosTransform.anchoredPosition;
        pos.y = startingY;

        multiplier = 1;

        creditosTransform.anchoredPosition = pos;
        actualPos = pos;
        
        StartCoroutine(TimerToMove());
    }

    private void Update()
    {
        if (creditosTransform.anchoredPosition.y >= endingY)
        {
            actualPos.y = endingY;
            creditosTransform.anchoredPosition = actualPos;

            if (clickToLeave != null) 
                clickToLeave.SetActive(true);

            // Si hacemos clic cuando ya terminó, cargamos el menú
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                SceneManager.LoadScene(mainMenuSceneName);
            }

            return;
        }

        if (canMove)
        {
            actualPos.y += movementSpeed * multiplier * Time.deltaTime;
            creditosTransform.anchoredPosition = actualPos;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            multiplier = clickMovementMultiplier;
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            multiplier = 1;
        }
    }

    IEnumerator TimerToMove()
    {
        yield return new WaitForSeconds(timeToMove);
        canMove = true;
    }
}