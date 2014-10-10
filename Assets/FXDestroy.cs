using UnityEngine;
using System.Collections;

public class FXDestroy : MonoBehaviour {

    public float selfDestructTime = 0.0f;

    // Update is called once per frame
    void Update() {
        selfDestructTime -= Time.deltaTime;

        if (selfDestructTime <= 0) {
            selfDestructTime = 0.2f;
            PhotonView pv = GetComponent<PhotonView>();

            if (pv != null && pv.instantiationId != 0) {
                PhotonNetwork.Destroy(gameObject);
            } else {
                Destroy();
            }
        }
    }

    private void Destroy() {
        gameObject.SetActive(false);
    }
}
