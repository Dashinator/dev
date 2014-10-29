using UnityEngine;

public class GunController_v2 : MonoBehaviour {
    FXManager fxManager;

    public float cooldown = 0f;
    public float spreadFactor = 0.02f;
    public float damage = 10f;
    public float range = 100f;

    // Use this for initialization
    void Start() {
        fxManager = FXManager.instance;
        Debug.Log("start GunController");
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetButton("Fire1")) {
            fire();
        }
        if (PhotonNetwork.isMasterClient) {
            cooldown -= Time.deltaTime;
        }
    }

    void fire() {
        //print("shot");
        int ID = gameObject.GetComponent<PhotonView>().viewID;
        PhotonView master = GameObject.Find("Scripts").GetComponent<PhotonView>();
        master.RPC("Fire_Receive", PhotonTargets.MasterClient, ID);
    }
}
