using UnityEngine;

public class BotonResetHelper : MonoBehaviour
{
    public void EjecutarReset()
    {
        if (RoomManager.Instance != null)
        {
            RoomManager.Instance.ResetCurrentRoom();
        }
    }
}