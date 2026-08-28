using UnityEngine;
using System.Collections;

public class BotonResetHelper : MonoBehaviour
{
    public float retrasoAntesDeReset = 0.3f;
    public void EjecutarReset()
    {
        StartCoroutine(RutinaResetConRetraso());
    }

    private IEnumerator RutinaResetConRetraso()
    {
        yield return new WaitForSeconds(retrasoAntesDeReset);

        if (RoomManager.Instance != null)
        {
            RoomManager.Instance.ResetCurrentRoom();
        }
    }
}