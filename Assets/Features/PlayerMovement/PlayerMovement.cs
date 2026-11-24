using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movSpeed = 5f; //Top speed for moving
    [SerializeField] private float movAccel = 20f; //How fast the player accelerates to walking speed/deaccelerates to motionless
    [SerializeField] private InputActionReference moveActionReference; //WASD or Arrow

    private Rigidbody playerRB;
    public Vector2 MoveInput { get; private set; } // public get for aiming script
    private Vector3 currentVelocity;

    private void Awake()
    {
        playerRB = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 moveInput3d = new Vector3(MoveInput.x, 0, MoveInput.y); //converting Vector2 into Vector 3
        Vector3 desiredVelocity = Vector3.ClampMagnitude(moveInput3d, 1f) * movSpeed; //The max speed normalized

        //accelerate or deaccelerate from current velocity to desired velocity
        currentVelocity = Vector3.MoveTowards(currentVelocity, desiredVelocity, movAccel * Time.fixedDeltaTime);
        playerRB.linearVelocity = new Vector3(currentVelocity.x, playerRB.linearVelocity.y, currentVelocity.z);
    }

    void OnEnable()
    {
        if(moveActionReference == null) return;
        moveActionReference.action.Enable();
        moveActionReference.action.performed += OnMovePerformed;
        moveActionReference.action.canceled += OnMoveCanceled;
    }

    void OnDisable()
    {
        if(moveActionReference == null) return;
        moveActionReference.action.performed -= OnMovePerformed;
        moveActionReference.action.canceled -= OnMoveCanceled;
        moveActionReference.action.Disable();
    }
    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        MoveInput = ctx.ReadValue<Vector2>(); //get velocity vector
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        MoveInput = Vector2.zero; //when no movement is pressed zero the velocity
    }

}
