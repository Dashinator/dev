using UnityEngine;
using System.Collections;

public class ResourceManager : MonoBehaviour {
    public float maxHealth = 100f;
    float currentHitPoints;

	void Start () {
        currentHitPoints = maxHealth;
	}
	
    [RPC]
    public void TakeDamage(float amt) {
        currentHitPoints -= amt;
        if(currentHitPoints <= 0){
            Die();
        }
    }

    void Die() {
        if( GetComponent<PhotonView>().instantiationId == 0 ) { 
            Destroy(gameObject);
        } else {
            if( PhotonNetwork.isMasterClient ) {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
