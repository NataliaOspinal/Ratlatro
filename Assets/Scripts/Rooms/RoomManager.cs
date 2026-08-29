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

    public List<EventoDeSala> salaEspecial;
    public List<GameObject> roomPrefabs;
    private GameObject currentRoom;

    private int numeroActualDeSala = 0;

    public Animator panelAnimator;
    public float tiempoDeEspera = 0.5f;
    private bool isTransitioning = false;
    public bool interaccionBloqueada = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start()
    {
        if (currentRoom == null) currentRoom = GameObject.Find("ROOM");

        if (currentRoom == null)
        {
            GenerarSalaInicial();
        }
    }

    private void GenerarSalaInicial()
    {
        GameObject roomToLoad = null;

        foreach (EventoDeSala evento in salaEspecial)
        {
            if (evento.numeroDeSala == numeroActualDeSala)
            {
                roomToLoad = evento.prefabSalaEspecial;
                break;
            }
        }

        if (roomToLoad == null && roomPrefabs.Count > 0)
        {
            int indice = ObtenerIndiceSecuencial();
            roomToLoad = roomPrefabs[indice];
        }

        if (roomToLoad != null)
        {
            // La sala inicial siempre nace en el centro (Vector3.zero)
            currentRoom = Instantiate(roomToLoad, Vector3.zero, Quaternion.identity);

            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                // Desaparecer a la rata al iniciar si quedó alguna residual
                MainPlayer mainPlayerScript = player.GetComponent<MainPlayer>();
                if (mainPlayerScript != null) mainPlayerScript.ForzarDespawnCompanero();

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
            Debug.LogWarning("Alerta: No hay prefabs asignados en el RoomManager");
        }
    }

    public void LoadNextRoom(Door.PuertaDireccion exitDirection, GameObject player)
    {
        if (isTransitioning || interaccionBloqueada) return;
        StartCoroutine(RutinaCambioSala(exitDirection, player));
    }

    public void ResetCurrentRoom()
    {
        if (isTransitioning || interaccionBloqueada) return;
        StartCoroutine(RutinaResetSala());
    }

    private IEnumerator RutinaResetSala()
    {
        isTransitioning = true;

        if (panelAnimator != null) panelAnimator.SetTrigger("CambiarSala");

        yield return new WaitForSeconds(tiempoDeEspera);

        Vector3 posicionSala = Vector3.zero;
        if (currentRoom != null)
        {
            posicionSala = currentRoom.transform.position;
            currentRoom.SetActive(false);
            Destroy(currentRoom);
        }

        GameObject roomToLoad = null;

        foreach (EventoDeSala evento in salaEspecial)
        {
            if (evento.numeroDeSala == numeroActualDeSala)
            {
                roomToLoad = evento.prefabSalaEspecial;
                break;
            }
        }

        if (roomToLoad == null && roomPrefabs.Count > 0)
        {
            int indice = ObtenerIndiceSecuencial();
            roomToLoad = roomPrefabs[indice];
        }

        if (roomToLoad != null)
        {
            currentRoom = Instantiate(roomToLoad, posicionSala, Quaternion.identity);

            // En el reset, buscamos al jugador manualmente y reaparecemos en Spawn_L
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                MainPlayer mainPlayerScript = player.GetComponent<MainPlayer>();
                if (mainPlayerScript != null) mainPlayerScript.ForzarDespawnCompanero();

                Transform spawnFolder = currentRoom.transform.Find("SpawnPts");
                if (spawnFolder != null)
                {
                    Transform targetSpawn = spawnFolder.Find("Spawn_L");
                    if (targetSpawn != null) player.transform.position = targetSpawn.position;
                }
            }
        }

        yield return new WaitForSeconds(0.1f);
        isTransitioning = false;
    }

    private IEnumerator RutinaCambioSala(Door.PuertaDireccion exitDirection, GameObject player)
    {
        isTransitioning = true;

        if (panelAnimator != null) panelAnimator.SetTrigger("CambiarSala");

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

        if (roomToLoad == null && roomPrefabs.Count > 0)
        {
            int indice = ObtenerIndiceSecuencial();
            roomToLoad = roomPrefabs[indice];
        }

        currentRoom = Instantiate(roomToLoad, posicionSala, Quaternion.identity);

        //Desaparecer a la rata usando el 'player' que llega por parámetro
        MainPlayer mainPlayerScript = player.GetComponent<MainPlayer>();
        if (mainPlayerScript != null) mainPlayerScript.ForzarDespawnCompanero();

        Transform spawnFolder = currentRoom.transform.Find("SpawnPts");
        if (spawnFolder != null)
        {
            Transform targetSpawn = (exitDirection == Door.PuertaDireccion.Right) ? spawnFolder.Find("Spawn_L") : spawnFolder.Find("Spawn_R");
            if (targetSpawn != null) player.transform.position = targetSpawn.position;
        }

        yield return new WaitForSeconds(0.1f);
        isTransitioning = false;
    }

    private int ObtenerIndiceSecuencial()
    {
        int salasEspecialesPasadas = 0;
        foreach (EventoDeSala evento in salaEspecial)
        {
            if (evento.numeroDeSala < numeroActualDeSala)
            {
                salasEspecialesPasadas++;
            }
        }
        return (numeroActualDeSala - salasEspecialesPasadas) % roomPrefabs.Count;
    }
}