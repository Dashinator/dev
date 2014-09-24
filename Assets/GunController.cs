using UnityEngine;
using System.Collections;

public class GunController : MonoBehaviour {

    float cooldown = 0;
    float damage = 25f;
    float fireRate = 0.5f;

    public Texture2D crosshairTexture;
    Rect position;

    void Update() {
        cooldown -= Time.deltaTime;

        if( Input.GetButtonDown("Fire1") ) {
            fire();
        }

        position = new Rect((Screen.width - crosshairTexture.width) / 2, (Screen.height -
            crosshairTexture.height) / 2, crosshairTexture.width, crosshairTexture.height);
    }

    void OnGUI() {
        GUI.DrawTexture(position, crosshairTexture);
    }

    void fire() {
        Debug.Log("Fire");
        if( cooldown > 0 ) {
            return;
        }

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        Transform hitTransform;
        Vector3 hitPoint;

        hitTransform = FindClosestHitObject(ray, out hitPoint);

        if( hitTransform != null ) {
            Debug.Log("We hit: " + hitTransform.name);

            ResourceManager h = hitTransform.GetComponent<ResourceManager>();

            while( h == null && hitTransform.parent ) {
                hitTransform = hitTransform.parent;
                h = hitTransform.GetComponent<ResourceManager>();
            }

            if( h != null ) {
                PhotonView pv = h.GetComponent<PhotonView>();
                if( pv == null ) {
                    Debug.LogError("No photonView found");
                } else {
                    pv.RPC("TakeDamage", PhotonTargets.AllBuffered, damage);
                }
            }
        }

        cooldown = fireRate;
    }

    Transform FindClosestHitObject( Ray ray, out Vector3 hitPoint ) {
        RaycastHit[] hits = Physics.RaycastAll(ray);
        Transform closestHit = null;
        float distance = 0;
        hitPoint = Vector3.zero;

        foreach( RaycastHit hit in hits ) {
            if( hit.transform != this.transform && (closestHit == null || hit.distance < distance) ) {
                closestHit = hit.transform;
                distance = hit.distance;
                hitPoint = hit.point;
            }
        }
        return closestHit;
    }
}


