using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement), typeof(Rigidbody))]
public class PlayerAiming : MonoBehaviour
{
    private Camera aimCamera;
    [SerializeField] private InputActionReference aimActionReference;
    [SerializeField] private InputActionReference cursorPositionActionReference;

    private PlayerMovement movement;
    private Rigidbody body;


    public Vector3 AimDirection { get; private set; }
    public bool IsAiming => aimActionReference.action.IsPressed();

    private void Awake()
    {
        aimCamera = Camera.main;
        movement = GetComponent<PlayerMovement>();
        body = GetComponent<Rigidbody>();
        AimDirection = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;

    }

    private void OnEnable()
    {
        aimActionReference.action.Enable();
        cursorPositionActionReference.action.Enable();
    }

    private void OnDisable()
    {
        aimActionReference.action.Disable();
        cursorPositionActionReference.action.Disable();
    }

    private bool SignificantChange(float f)
    {
        return f > 0.0001f;
    }

    private void Update()
    {
        // if the player is not aiming, point the aiming cone in the
        // direction of their current intended movement direction
        if (!IsAiming)
        {
            Vector2 input = movement.MoveInput;
            if (SignificantChange(input.sqrMagnitude))
            {
                AimDirection = new Vector3(input.x, 0f, input.y).normalized;
            }
            return;
        }

        // while aiming, calculate the direction that the player should be
        // facing so we can write the location to their avatar in a FixedUpdate
        Plane plane = new(Vector3.up, body.position);
        Ray ray = aimCamera.ScreenPointToRay(cursorPositionActionReference.action.ReadValue<Vector2>());
        if (plane.Raycast(ray, out float distance))
        {
            Vector3 direction = ray.GetPoint(distance) - body.position;
            direction.y = 0f;
            if (SignificantChange(direction.sqrMagnitude))
            {
                AimDirection = direction.normalized;
            }
        }
    }

    private void FixedUpdate()
    {
        // physically turn player body in the direction of the aim direction
        body.MoveRotation(Quaternion.LookRotation(AimDirection, Vector3.up));
    }
}
