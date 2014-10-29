using UnityEngine;
using System.Collections;

public class NetworkCharacter : Photon.MonoBehaviour {

    Vector3 realPosition = Vector3.zero;
    Quaternion realRotation = Quaternion.identity;
    Quaternion camRotation = Quaternion.identity;
    GameObject playerCam;
    bool death = false;

    // Use this for initialization
    void Start() {
        death = false;
        playerCam = gameObject.transform.Find("PlayerCamera").gameObject;
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (photonView.isMine) {
            // Do nothing
        } else {
            transform.position = Vector3.Lerp(transform.position, realPosition, 0.1f);
            transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, 0.1f);
            playerCam.transform.rotation = camRotation;
        }

        if (PhotonNetwork.isMasterClient)
        {
            outOfBounds();
        }
    }


    private void outOfBounds()
    {
        if (transform.position.y <= -20 && !death)
        {
            death = true;
            Debug.Log("death: " + death.ToString());
            this.photonView.RPC("Die_RPC", this.photonView.owner, "Gravity");
            GameObject.Find("Scripts").GetComponent<NetworkManager_v2>().Death(this.photonView.owner);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            //Onze speler, stuur data naar andere spelers;
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(playerCam.transform.rotation);
        } else {
            //andere spelers, ontvang data
            realPosition = (Vector3)stream.ReceiveNext();
            realRotation = (Quaternion)stream.ReceiveNext();
            camRotation = (Quaternion)stream.ReceiveNext();
        }

    }
}
