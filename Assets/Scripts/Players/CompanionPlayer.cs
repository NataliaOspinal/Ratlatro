using UnityEngine;
using UnityEngine.InputSystem;

public class CompanionPlayer : BaseIsometricPlayer
{
    protected override Vector2 GetInput()
    {
        Vector2 input = Vector2.zero;

        if (Keyboard.current != null)
        {
            // Input Flechas
            if (Keyboard.current.rightArrowKey.isPressed) input.x += 1f;
            if (Keyboard.current.leftArrowKey.isPressed) input.x -= 1f;
            if (Keyboard.current.upArrowKey.isPressed) input.y += 1f;
            if (Keyboard.current.downArrowKey.isPressed) input.y -= 1f;
        }

        return input; // Lo enviamos a la clase base para que procese el movimiento
    }
}
