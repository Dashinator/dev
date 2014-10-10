using UnityEngine;
using System.Collections;

public class ResourceManager : MonoBehaviour {
    public float maxHealth = 100f;
    float currentHitPoints;

	void Start () {
        currentHitPoints = maxHealth;
	}
	
    [RPC]
    public void TakeDamage(float amt, string name) {
        currentHitPoints -= amt;
        if(currentHitPoints <= 0){
            Die(name);
        }
    }

    void Die(string name) {
        if( GetComponent<PhotonView>().instantiationId == 0 ) { 
            Destroy(gameObject);
        } else {
            if( GetComponent<PhotonView>().isMine ) {
                if( gameObject.tag == "Player" ) {
                    NetworkManager_v2 nm = GameObject.FindObjectOfType<NetworkManager_v2>();
                    nm.AddChatMessage(name + " just killed " + PhotonNetwork.player.name);
                    nm.standbyCamera.SetActive(true);
                    nm.respawnTimer = 3f;
                }

                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    [RPC]
    void Die_RPC(string name) {
        Die(name);
    }
}
