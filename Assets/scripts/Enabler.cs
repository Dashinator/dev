using UnityEngine;
using System.Collections.Generic;

public class Enabler : MonoBehaviour {

	public void enable(GameObject player) {
        MonoBehaviour[] comps = GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour c in comps) {
            c.enabled = true;
        }
        foreach (Transform child in transform) {
            child.gameObject.SetActive(true);
        }
        
        //player.GetComponent<PlayerController>().enabled = true;
        //player.GetComponent<GunController>().enabled = true;
        //player.transform.FindChild("PlayerCamera").gameObject.SetActive(true);
    }
}
