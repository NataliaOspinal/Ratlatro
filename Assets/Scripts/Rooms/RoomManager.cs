using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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

    [Header("Sala Aleatoria en Rango")]
    public bool activarSalaSorpresa = false;
    public GameObject prefabSalaSorpresa;
    public int rangoMinimo = 3;
    public int rangoMaximo = 6;

    [Header("Fin del Nivel (Cambio de Escena)")]
    public bool terminarNivel = false;
    public int salaParaSalirDelNivel = 0;
    public string escenaTransicion = "PantallaGuardado";
    public int numeroSiguienteZona = 3;
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
        if (activarSalaSorpresa && prefabSalaSorpresa != null)
        {
            InsertarSalaSorpresa();
        }

        if (currentRoom == null) currentRoom = GameObject.Find("ROOM");

        if (currentRoom == null)
        {
            GenerarSalaInicial();
        }
    }

    private void InsertarSalaSorpresa()
    {
        int salaElegida = Random.Range(rangoMinimo, rangoMaximo + 1);

        for (int i = 0; i < salaEspecial.Count; i++)
        {
            if (salaEspecial[i].numeroDeSala >= salaElegida)
            {
                EventoDeSala eventoModificado = salaEspecial[i];
                eventoModificado.numeroDeSala++; 
                salaEspecial[i] = eventoModificado;
            }
        }

        EventoDeSala nuevaSalaAleatoria = new EventoDeSala
        {
            numeroDeSala = salaElegida,
            prefabSalaEspecial = prefabSalaSorpresa
        };

        salaEspecial.Add(nuevaSalaAleatoria);
        
        Debug.Log("La sala sorpresa ha sido insertada en la posicion: " + salaElegida);
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
            currentRoom = Instantiate(roomToLoad, Vector3.zero, Quaternion.identity);

            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                MainPlayer mainPlayerScript = player.GetComponent<MainPlayer>();
                if (mainPlayerScript != null)
                {
                    mainPlayerScript.ForzarDespawnCompanero();
                    mainPlayerScript.Revivir(); 
                }

                Transform spawnFolder = currentRoom.transform.Find("SpawnPts");
                if (spawnFolder != null)
                {
                    Transform targetSpawn = spawnFolder.Find("Spawn_L");
                    if (targetSpawn != null) player.transform.position = targetSpawn.position;
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

            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                MainPlayer mainPlayerScript = player.GetComponent<MainPlayer>();
                if (mainPlayerScript != null)
                {
                    mainPlayerScript.ForzarDespawnCompanero();
                    mainPlayerScript.Revivir();
                }

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
        numeroActualDeSala++;

        if (terminarNivel && numeroActualDeSala >= salaParaSalirDelNivel)
        {
            MainPlayer scriptRata = player.GetComponent<MainPlayer>();
            if (scriptRata != null) scriptRata.canMove = false;

            PlayerPrefs.SetInt("SiguienteZona", numeroSiguienteZona);

            SceneManager.LoadScene(escenaTransicion);
            yield break; 
        }

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

        currentRoom = Instantiate(roomToLoad, posicionSala, Quaternion.identity);

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