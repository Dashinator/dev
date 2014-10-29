using UnityEngine;
using System.Collections.Generic;

public class Enabler : Photon.MonoBehaviour {

	public void enable(GameObject player) {
        MonoBehaviour[] comps = GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour c in comps) {
            c.enabled = true;
        }
        foreach (Transform child in transform) {
            child.gameObject.SetActive(true);
        }

        GameObject.Find("Scripts").GetComponent<PhotonView>().RPC("Enable", PhotonTargets.MasterClient, this.photonView.viewID);
        
        //player.GetComponent<PlayerController>().enabled = true;
        //player.GetComponent<GunController>().enabled = true;
        //player.transform.FindChild("PlayerCamera").gameObject.SetActive(true);
    }
}
