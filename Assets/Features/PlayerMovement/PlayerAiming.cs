using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement), typeof(Rigidbody))]
public class PlayerAiming : MonoBehaviour
{
    private Camera aimCamera;
    [SerializeField] private InputActionReference aimActionReference;
    [SerializeField] private InputActionReference cursorPositionActionReference;
    [SerializeField] private Transform aimPivot;

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

        // while aiming, point toward the mouse cursor
        Plane plane = new(Vector3.up, transform.position);
        Ray ray = aimCamera.ScreenPointToRay(cursorPositionActionReference.action.ReadValue<Vector2>());
        if (plane.Raycast(ray, out float distance))
        {
            Vector3 direction = ray.GetPoint(distance) - transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude > 0.25f) // ignore cursor within 0.5 units
            {
                AimDirection = direction.normalized;
            }
        }
    }

    private void LateUpdate()
    {
        Quaternion goal = Quaternion.LookRotation(AimDirection, Vector3.up);
        aimPivot.rotation = Quaternion.Slerp(aimPivot.rotation, goal, 1f - Mathf.Exp(-25f * Time.deltaTime));
    }
}
