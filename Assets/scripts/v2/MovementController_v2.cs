using UnityEngine;
using System.Collections;

public class MovementController_v2 : Photon.MonoBehaviour {
    public float speed = 10f;

    public float gravity = 10.0f;
    public float maxVelocityChange = 10.0f;
    public float jumpHeight = 2.0f;
    public bool canJump = true;
    private bool grounded = false;

    void Start() {
        rigidbody.freezeRotation = true;
        rigidbody.useGravity = false;
    }

    void FixedUpdate() {
        if (photonView.isMine) {
            InputMovement();
            outOfBoulds();
        }
    }

    private void outOfBoulds() {
        if (transform.position.y <= -20 && photonView.isMine) {
            this.photonView.RPC("Die_RPC", PhotonTargets.AllBuffered, "Gravity");
        }
    }

    void InputMovement() {

        Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        targetVelocity = transform.TransformDirection(targetVelocity);
        targetVelocity *= speed;

        Vector3 velocity = rigidbody.velocity;
        Vector3 velocityChange = (targetVelocity - velocity);

        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0;
        rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

        if (canJump && Input.GetButton("Jump") && grounded) {
            rigidbody.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
        }

        rigidbody.AddForce(new Vector3(0, -gravity * rigidbody.mass, 0));


        grounded = false;
    }

    private float CalculateJumpVerticalSpeed() {
        return Mathf.Sqrt(2 * jumpHeight * gravity);
    }

    void OnCollisionStay(Collision collisionInfo) {
        if (photonView.isMine) {
            if (collisionInfo.gameObject.tag == "Floor") {
                grounded = true;
            }
        }
    }
}
