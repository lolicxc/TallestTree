using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private void Update()
    {
        // Esto funciona en mobile y funciona en PC usando mouse
        if (Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            if (MoveCylinder.CurrentCylinder != null)
                MoveCylinder.CurrentCylinder.Stop();
        }
    }
}
