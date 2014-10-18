using UnityEngine;
using System.Collections.Generic;

public class NetworkManager_v2 : MonoBehaviour {
    public GameObject playerPrefab;
    public bool offlineMode = false;
    private const string roomName = "Room";
    private bool connecting = false;
    public GameObject standbyCamera;

    private bool locked = false;

    List<PlayerStats> playerList;

    List<string> chatMessages;
    public int maxMessages = 5;

    SpawnSpot[] spawnSpots;

    public float respawnTimer = 0;

	void Start () {
        spawnSpots = GameObject.FindObjectsOfType<SpawnSpot>();
        PhotonNetwork.player.name = PlayerPrefs.GetString("Username", "Player");
        chatMessages = new List<string>();
        playerList = new List<PlayerStats>();
	}
	
	void OnDestroy () {
        PlayerPrefs.SetString("Username", PhotonNetwork.player.name);
	}

    public void AddChatMessage(string m) {
        GetComponent<PhotonView>().RPC("AddChatMessage_RPC", PhotonTargets.All, m);
    }

    [RPC]
    void AddChatMessage_RPC(string m) {
        while (chatMessages.Count >= maxMessages) {
            chatMessages.RemoveAt(0);
        }
        chatMessages.Add(m);
    }

    private void Connect() {
        PhotonNetwork.ConnectUsingSettings("1.0");
        PhotonNetwork.logLevel = PhotonLogLevel.ErrorsOnly;
    }

    void OnMasterClientSwitched() {
        PhotonNetwork.Disconnect();
        standbyCamera.SetActive(true);
    }

    void OnGUI() {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());

        if (PhotonNetwork.connected == false && !connecting) {
            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.Label("username");
            PhotonNetwork.player.name = GUILayout.TextField(PhotonNetwork.player.name, GUILayout.Width(100));
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Singleplayer")) {
                connecting = true;
                PhotonNetwork.offlineMode = true;
                OnJoinedLobby();

            }
            if (GUILayout.Button("Multiplayer")) {
                connecting = true;
                Connect();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        if (PhotonNetwork.connected && !connecting) {
            if (!PhotonNetwork.isMasterClient) {
                GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
                foreach (string message in chatMessages) {
                    GUILayout.Label(message);
                }



                GUILayout.EndVertical();
                GUILayout.EndArea();
            } else {
                GUILayout.BeginArea(new Rect(Screen.width / 4, Screen.height / 4, Screen.width / 2, Screen.height / 2), GUIContent.none, "box");
                GUILayout.BeginHorizontal();
                foreach (PlayerStats player in playerList) {
                    GUILayout.Label(player.player.name);
                    GUILayout.Label(player.kill.ToString());
                    GUILayout.Label(player.death.ToString());
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndArea();
            }
        }
    }

    void OnJoinedLobby() {
        if (PhotonNetwork.player.name == "server") {
            RoomOptions roomOp = new RoomOptions();
            roomOp.maxPlayers = 20;
            PhotonNetwork.CreateRoom(roomName, roomOp, null);
        } else {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    void OnPhotonRandomJoinFailed() {
        Debug.Log("Can't join random room!");
        PhotonNetwork.Disconnect();
        connecting = false;
    }

    void OnJoinedRoom() {
        connecting = false;
        if (PhotonNetwork.player.name != "server" || PhotonNetwork.offlineMode) {
            AddChatMessage(PhotonNetwork.player.name + " has joined");
            GetComponent<PhotonView>().RPC("PlayerJoined", PhotonTargets.MasterClient, PhotonNetwork.player);
            spawnMyPlayer();
        }
    }

    void OnPhotonPlayerDisconnected(PhotonPlayer other) {
        if (PhotonNetwork.isMasterClient) {
            foreach (PlayerStats playerS in playerList) {
                if (playerS.player.name == other.name) {
                    playerList.Remove(playerS);
                }
            }
        }
    }

    [RPC]
    void PlayerJoined(PhotonPlayer pPlayer) {
        playerList.Add(new PlayerStats() { player = pPlayer, kill = 0, death = 0});
    }

    void spawnMyPlayer() {
        if (spawnSpots == null) {
            Debug.LogError("spawn crash");
            return;
        }
        SpawnSpot  mySpawnSpot = spawnSpots[Random.Range(0, spawnSpots.Length)];
        // Spawn player
        GameObject player = (GameObject)PhotonNetwork.Instantiate(playerPrefab.name, (mySpawnSpot.transform.position + Vector3.up), mySpawnSpot.transform.rotation, 0);
        if (player != null) {
            AddChatMessage(PhotonNetwork.player.name + " has spawned");

            //disable overlook cam
            standbyCamera.SetActive(false);

            player.GetComponent<Enabler>().enable(player);
            Screen.lockCursor = true;
            locked = true;
        }
    }

    void Update() {
        if (respawnTimer > 0) {
            respawnTimer -= Time.deltaTime;
            Screen.lockCursor = false;
            locked = false;

            if (respawnTimer <= 0) {
                spawnMyPlayer();
            }
        }
    }

}

public class PlayerStats {
    public PhotonPlayer player;
    public int kill;
    public int death;
}