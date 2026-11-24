using UnityEngine;
using UnityEngine.InputSystem;

// TEMP: test-only player so zombies have something to chase.
public class TestPlayerMover : MonoBehaviour
{
    [SerializeField] float speed = 5f;

    void Update()
    {
        Keyboard k = Keyboard.current;
        if (k == null) return;

        Vector3 dir = Vector3.zero;
        if (k.wKey.isPressed) dir.z += 1;
        if (k.sKey.isPressed) dir.z -= 1;
        if (k.aKey.isPressed) dir.x -= 1;
        if (k.dKey.isPressed) dir.x += 1;

        transform.position += dir.normalized * speed * Time.deltaTime;
    }
}