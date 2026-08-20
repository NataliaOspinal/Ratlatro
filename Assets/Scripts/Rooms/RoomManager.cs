using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }

    [Header("Room Data")]
    public List<GameObject> roomPrefabs;
    private GameObject currentRoom;

    [Header("Transicion de Barrido")]
    public Animator panelAnimator;
    public float tiempoDeEspera = 0.5f; 

    private bool isTransitioning = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start()
    {
        if (currentRoom == null) currentRoom = GameObject.Find("ROOM");
    }

    public void LoadNextRoom(Door.PuertaDireccion exitDirection, GameObject player)
    {
        if (isTransitioning) return; 
        
        StartCoroutine(RutinaCambioSala(exitDirection, player));
    }

    private IEnumerator RutinaCambioSala(Door.PuertaDireccion exitDirection, GameObject player)
    {
        isTransitioning = true;

        if (panelAnimator != null)
        {
            panelAnimator.SetTrigger("CambiarSala");
        }

        yield return new WaitForSeconds(tiempoDeEspera);

        Vector3 posicionSala = Vector3.zero;
        if (currentRoom != null)
        {
            posicionSala = currentRoom.transform.position;
            currentRoom.SetActive(false); 
            Destroy(currentRoom);
        }

        int randomIndex = Random.Range(0, roomPrefabs.Count);
        GameObject roomToLoad = roomPrefabs[randomIndex];
        currentRoom = Instantiate(roomToLoad, posicionSala, Quaternion.identity);

        Transform spawnFolder = currentRoom.transform.Find("SpawnPts");
        if (spawnFolder != null)
        {
            Transform targetSpawn = (exitDirection == Door.PuertaDireccion.Right) ? spawnFolder.Find("Spawn_L") : spawnFolder.Find("Spawn_R");

            if (targetSpawn != null)
            {
                player.transform.position = targetSpawn.position;
            }
        }

        yield return new WaitForSeconds(0.1f);
        isTransitioning = false;
    }
}