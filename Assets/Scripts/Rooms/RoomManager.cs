using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct EventoDeSala
{
    public int numeroDeSala;
    public GameObject prefabSalaEspecial;
}

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }

    [Header("Salas Fijas")]
    public List<EventoDeSala> salaEspecial;

    [Header("Salas Aleatorias")]
    public List<GameObject> roomPrefabs;
    private GameObject currentRoom;

    private int numeroActualDeSala = 0;

    [Header("Transicion de Barrido")]
    public Animator panelAnimator;
    public float tiempoDeEspera = 0.5f;
    private bool isTransitioning = false;

    [Header("Estado del Juego")]
    public bool interaccionBloqueada = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start()
    {
        // Intenta encontrar la sala si la pusiste a mano
        if (currentRoom == null) currentRoom = GameObject.Find("ROOM");

        // Si no hay sala manual, genera la inicial automáticamente
        if (currentRoom == null)
        {
            GenerarSalaInicial();
        }
    }

    private void GenerarSalaInicial()
    {
        GameObject roomToLoad = null;

        // Buscamos si hay sala 0 en el Inspector
        foreach (EventoDeSala evento in salaEspecial)
        {
            if (evento.numeroDeSala == numeroActualDeSala)
            {
                roomToLoad = evento.prefabSalaEspecial;
                break;
            }
        }

        // Si no hay sala especial 0, toma el primer prefab de la lista aleatoria
        if (roomToLoad == null && roomPrefabs.Count > 0)
        {
            roomToLoad = roomPrefabs[0];
        }

        if (roomToLoad != null)
        {
            // Instanciamos el primer nivel
            currentRoom = Instantiate(roomToLoad, Vector3.zero, Quaternion.identity);

            // Ubicamos a la rata en el punto de aparición izquierdo por defecto
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                Transform spawnFolder = currentRoom.transform.Find("SpawnPts");
                if (spawnFolder != null)
                {
                    Transform targetSpawn = spawnFolder.Find("Spawn_L");
                    if (targetSpawn != null)
                    {
                        player.transform.position = targetSpawn.position;
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("Alerta: No hay prefabs asignados en el RoomManager"); //xsiacaso
        }
    }

    public void LoadNextRoom(Door.PuertaDireccion exitDirection, GameObject player)
    {
        if (isTransitioning || interaccionBloqueada) return;

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

        numeroActualDeSala++;
        GameObject roomToLoad = null;

        foreach (EventoDeSala evento in salaEspecial)
        {
            if (evento.numeroDeSala == numeroActualDeSala)
            {
                roomToLoad = evento.prefabSalaEspecial;
                break;
            }
        }

        if (roomToLoad == null)
        {
            int randomIndex = Random.Range(0, roomPrefabs.Count);
            roomToLoad = roomPrefabs[randomIndex];
        }

        currentRoom = Instantiate(roomToLoad, posicionSala, Quaternion.identity);

        Transform spawnFolder = currentRoom.transform.Find("SpawnPts");
        if (spawnFolder != null)
        {
            Transform targetSpawn = (exitDirection == Door.PuertaDireccion.Right) ? spawnFolder.Find("Spawn_L") : spawnFolder.Find("Spawn_R");
            if (targetSpawn != null) player.transform.position = targetSpawn.position;
        }

        yield return new WaitForSeconds(0.1f);
        isTransitioning = false;
    }
}