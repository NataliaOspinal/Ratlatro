using UnityEngine; 
using System.Collections.Generic;
using System.ComponentModel;

public class RoomManager : MonoBehaviour
{

    public static RoomManager Instance { get; private set; }

    [Header("Room Data")]
    public List<GameObject> roomPrefabs; 
    private GameObject currentRoom;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance=this;
        }
    }

    private void Start()
    {
        currentRoom=GameObject.Find("ROOM");
    }

    public void LoadNextRoom(Door.PuertaDireccion exitDirection, GameObject player)
    {
        if (currentRoom != null)
        {
            Destroy(currentRoom);
        }

        // Elegir una sala aleatoria de la lista
        int randomI=Random.Range(0,roomPrefabs.Count);
        GameObject roomToload = roomPrefabs[randomI];

        currentRoom=Instantiate(roomToload, Vector3.zero, UnityEngine.Quaternion.identity);

        Transform spawnFolder = currentRoom.transform.Find("SpawnPts");
        Transform targetSpawn = null;

        if (exitDirection == Door.PuertaDireccion.Right)
        {
            targetSpawn = spawnFolder.Find("Spawn_L"); 
        }
        else if (exitDirection == Door.PuertaDireccion.Left)
        {
            targetSpawn = spawnFolder.Find("Spawn_R"); 

        }

        if (targetSpawn != null)
        {
            player.transform.position = targetSpawn.position;
        }
        else
        {
            Debug.LogWarning("Faltan Spawns");
        }
    }
}