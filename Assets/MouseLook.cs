using UnityEngine;
using System.Collections;

public class MouseLook : Photon.MonoBehaviour {
    public float mouseSensitivity = 5.0f;
    float verticalRotation = 0;
    public float upDownRange = 60.0f;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void FixedUpdate() {
        if (photonView.isMine) {
            InputMovement();
        }
    }
    void InputMovement() {

        float rotLeftRight = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(0, rotLeftRight, 0);

        verticalRotation -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -upDownRange, upDownRange);
        Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }
}
