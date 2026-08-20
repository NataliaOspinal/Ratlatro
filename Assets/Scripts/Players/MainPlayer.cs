using UnityEngine;
using UnityEngine.InputSystem;

public class MainPlayer : BaseIsometricPlayer
{
    [Header("Companion Settings")]
    public GameObject companionPrefab;
    private GameObject spawnedCompanion;

    protected override Vector2 GetInput()
    {
        Vector2 input = Vector2.zero;

        if (Keyboard.current != null)
        {
            // Input WASD
            if (Keyboard.current.dKey.isPressed) input.x += 1f;
            if (Keyboard.current.aKey.isPressed) input.x -= 1f;
            if (Keyboard.current.wKey.isPressed) input.y += 1f;
            if (Keyboard.current.sKey.isPressed) input.y -= 1f;

            // Habilidad exclusiva del Main Player
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                ToggleCompanion();
            }
        }

        return input;
    }

    //Mitosis ratita
    private void ToggleCompanion()
    {
        if (spawnedCompanion == null)
        {
            Vector3 spawnPosition = transform.position + new Vector3(1f, 0.5f, 0f);
            spawnedCompanion = Instantiate(companionPrefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            Destroy(spawnedCompanion);
        }
    }
}