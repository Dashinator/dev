using UnityEngine;
using System.Collections;


public class NetworkManager : MonoBehaviour {
    public GameObject playerPrefab;
    private const string roomName = "RoomName";
    private GameObject player;

	// Use this for initialization
	void Start () {
        PhotonNetwork.ConnectUsingSettings("0.1");
        PhotonNetwork.logLevel = PhotonLogLevel.Full;
	}

    void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }
    void OnJoinedLobby()
    {
        //roomsList = PhotonNetwork.GetRoomList();
        PhotonNetwork.JoinRandomRoom();
    }

    void OnPhotonRandomJoinFailed()
    {
        Debug.Log("Can't join random room!");
        PhotonNetwork.CreateRoom(null);
    }

    void OnJoinedRoom()
    {
        // Spawn player
        player = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.up * 5, Quaternion.identity, 0);
        player.GetComponent<PlayerController>().enabled = true;
        enableCam();


    }

    void enableCam() {
        player.GetComponentInChildren<Camera>().enabled = true;
        player.GetComponentInChildren<AudioListener>().enabled = true;
        player.GetComponentInChildren<GUILayer>().enabled = true;
    }
}
