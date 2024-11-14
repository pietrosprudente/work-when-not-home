using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing;

public class PlayerGravity : MonoBehaviour
{
    public InputActionProperty jumpButton;
    public Rigidbody rb;
    public float jumpHeight = 5f;
    private bool playerMoving;
    private ClimbProvider climbProvider;
    public InputActionProperty leftHandMoveAction;
    public Transform forwardSource;
    public float moveSpeed;
    public CapsuleCollider bodyCollider;
    private PhysicsRig physicsRig;
    private Vector3 rayPosition;

    public bool IsGrounded() {
        RaycastHit hit;
        Ray ray = new Ray(bodyCollider.transform.position
        + Vector3.one * 0.1f, -rb.transform.up);
        bool hasHit = Physics.Raycast(ray, out hit, 0.2f);
        return hasHit;
    }

    private void Start() {
        bodyCollider = rb.GetComponent<CapsuleCollider>();
        physicsRig = bodyCollider.GetComponent<PhysicsRig>();
    }

    private void FixedUpdate()
    {
        bool isGrounded = IsGrounded();
        climbProvider = GetComponent<ClimbProvider>();
        if (isGrounded) {
            rayPosition = bodyCollider.transform.position;
        }
        else {
            rayPosition = physicsRig.playerHead.transform.position - Vector3.down * physicsRig.bodyHeightMin;
        }
        if (climbProvider.locomotionState == UnityEngine.XR.Interaction.Toolkit.Locomotion.LocomotionState.Moving || climbProvider.isLocomotionActive) {
            rb.isKinematic = true;
            return;
        }
        else {
            rb.isKinematic = false;
        }

        float x = leftHandMoveAction.action.ReadValue<Vector2>().x;
        float z = leftHandMoveAction.action.ReadValue<Vector2>().y;
        Vector2 moveInput = new Vector2(x, z);

        if (isGrounded) {
            Quaternion yaw = Quaternion.Euler(0, forwardSource.eulerAngles.y, 0);
            Vector3 move = yaw * new Vector3(moveInput.x, 0, moveInput.y);

            rb.MovePosition(rb.position + move * Time.fixedDeltaTime * moveSpeed);
            if (jumpButton.action.WasPressedThisFrame()) {
                Jump();
            }
        }
    }

    private void Jump(float force = 0) {
        rb.linearVelocity = Vector3.up * (jumpHeight + force);
    }
}
