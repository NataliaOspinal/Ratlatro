using UnityEngine;

public class Door : MonoBehaviour
{
    public enum PuertaDireccion {Right, Left}
    public PuertaDireccion direccion;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            RoomManager.Instance.LoadNextRoom(direccion, collision.gameObject);
        }
    }
}
