using UnityEngine;
using System.Collections;


public class NetworkManager : MonoBehaviour {
    public GameObject playerPrefab;
    public bool offlineMode = false;
    private const string roomName = "RoomName";

	// Use this for initialization
	void Start () {
        if (offlineMode) {
            PhotonNetwork.offlineMode = true;
            OnJoinedLobby();
        } else {
        PhotonNetwork.ConnectUsingSettings("0.1");
        PhotonNetwork.logLevel = PhotonLogLevel.Full;
        }
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
        GameObject player = (GameObject)PhotonNetwork.Instantiate(playerPrefab.name, Vector3.up * 5, Quaternion.identity, 0);
        //disable overlook cam

        //enable player scrips
        player.GetComponent<PlayerController>().enabled = true;
        player.GetComponent<GunController>().enabled = true;
        player.transform.FindChild("PlayerCamera").gameObject.SetActive(true);
        


    }
}
