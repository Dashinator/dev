using UnityEngine;
using System.Collections.Generic;


public class NetworkManager : MonoBehaviour {
    public GameObject playerPrefab;
    public bool offlineMode = false;
    private const string roomName = "RoomName";
    private bool connecting = false;
    public GameObject standbyCamera;

    List<string> chatMessages;
    public int maxMessages = 5;

    SpawnSpot[] spawnSpots;

    public float respawnTimer = 0;

    // Use this for initialization
    void Start() {
        spawnSpots = GameObject.FindObjectsOfType<SpawnSpot>();
        PhotonNetwork.player.name = PlayerPrefs.GetString("Username", "Player");
        chatMessages = new List<string>();
    }

    void OnDestroy() {
        PlayerPrefs.SetString("Username", PhotonNetwork.player.name);
    }

    public void AddChatMessage( string m ) {
        GetComponent<PhotonView>().RPC("AddChatMessage_RPC", PhotonTargets.All, m);
    }

    [RPC]
    void AddChatMessage_RPC( string m ) {
        while( chatMessages.Count >= maxMessages ) {
            chatMessages.RemoveAt(0);
        }
        chatMessages.Add(m);
    }

    private void Connect() {
        PhotonNetwork.ConnectUsingSettings("0.1");
        PhotonNetwork.logLevel = PhotonLogLevel.Full;
    }

    void OnGUI() {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());

        if( PhotonNetwork.connected == false && !connecting ) {
            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.Label("username");
            PhotonNetwork.player.name = GUILayout.TextField(PhotonNetwork.player.name, GUILayout.Width(100));
            GUILayout.EndHorizontal();

            if( GUILayout.Button("Singleplayer") ) {
                connecting = true;
                PhotonNetwork.offlineMode = true;
                OnJoinedLobby();

            }
            if( GUILayout.Button("Multiplayer") ) {
                connecting = true;
                Connect();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        if( PhotonNetwork.connected && !connecting ) {
            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            foreach( string message in chatMessages ) {
                GUILayout.Label(message);
            }



            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
    void OnJoinedLobby() {
        //roomsList = PhotonNetwork.GetRoomList();
        PhotonNetwork.JoinRandomRoom();
    }

    void OnPhotonRandomJoinFailed() {
        Debug.Log("Can't join random room!");
        PhotonNetwork.CreateRoom(null);
    }

    void OnJoinedRoom() {
        connecting = false;
        AddChatMessage(PhotonNetwork.player.name + " has joined");
        spawnMyPlayer();
    }

    void spawnMyPlayer() {
        if( spawnSpots == null ) {
            Debug.LogError("spawn crash");
            return;
        }
        SpawnSpot mySpawnSpot = spawnSpots[Random.Range(0, spawnSpots.Length)];
        // Spawn player
        GameObject player = (GameObject)PhotonNetwork.Instantiate(playerPrefab.name, (mySpawnSpot.transform.position+Vector3.up), mySpawnSpot.transform.rotation, 0);
        if( player != null ) {
            AddChatMessage(PhotonNetwork.player.name + " has spawned");

            //disable overlook cam
            standbyCamera.SetActive(false);

            //enable player scrips
            player.GetComponent<PlayerController>().enabled = true;
            player.GetComponent<GunController>().enabled = true;
            player.transform.FindChild("PlayerCamera").gameObject.SetActive(true);
        }
    }

    void Update() {
        if( respawnTimer > 0 ) {
            respawnTimer -= Time.deltaTime;

            if( respawnTimer <= 0 ) {
                spawnMyPlayer();
            }
        }
    }
}
