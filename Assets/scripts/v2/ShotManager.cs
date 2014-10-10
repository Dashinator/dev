using UnityEngine;

public class ShotManager : MonoBehaviour {

    FXManager fxManager;

    void Start() {
        fxManager = FXManager.instance;
    }


    [RPC]
    void Fire_Receive(int id) {
        //print("shot received");
        if (PhotonNetwork.isMasterClient) {
            //print("id shot from: " + id);
            PhotonView shooter = PhotonView.Find(id);
            CalcShot(shooter.gameObject);
        }
    }

    private void CalcShot(GameObject shooter) {
        //verkrijg gun informatie uit speler;
        GunController_v2 gunInfo = shooter.GetComponent<GunController_v2>();
        float cooldown = gunInfo.cooldown;
        float spreadFactor = gunInfo.spreadFactor;
        float damage = gunInfo.damage;
        float range = gunInfo.range;

        //verkrijg speler camera en FX start punt
        GameObject playerCam = shooter.transform.Find("PlayerCamera").gameObject;
        Vector3 startPos = playerCam.transform.Find("GunTip").gameObject.transform.position;

        //mag niet schieten als de cooldown nog niet vooribj is
        if (cooldown > 0) {
            return;
        }
        //Debug.Log("fired from: " + startPos.ToString() + ". cooldown: " + gunInfo.cooldown);

        //bereken shot richting door middel van spread
        Vector3 direction = playerCam.transform.forward;
        direction.x += Random.Range(-spreadFactor, spreadFactor);
        direction.y += Random.Range(-spreadFactor, spreadFactor);
        direction.z += Random.Range(-spreadFactor, spreadFactor);

        //bereken shot
        Ray ray = new Ray(playerCam.transform.position, direction);
        Transform hitTransform;
        Vector3 hitPoint;
        Vector3 end = ray.origin + (ray.direction * 100);

        //bekijk wat de dichtsbijzijnde object is, behalve zelf
        hitTransform = FindClosestHitObject(ray, out hitPoint, shooter.transform, range);

        if (hitTransform != null) {
            Debug.Log("shot from: " + startPos + " to: " + hitPoint);
            Debug.Log("We hit: " + hitTransform.name);

            //kijk of target schade kan krijgen
            ResourceManager h = hitTransform.GetComponent<ResourceManager>();

            while (h == null && hitTransform.parent) {
                hitTransform = hitTransform.parent;
                h = hitTransform.GetComponent<ResourceManager>();
            }

            if (h != null) {
                PhotonView pv = h.GetComponent<PhotonView>();
                if (pv == null) {
                    Debug.LogError("No photonView found");
                } else {
                    //geef aan iedereen door dat target schade krijgt
                    pv.RPC("TakeDamage", PhotonTargets.AllBuffered, damage, PhotonNetwork.player.name);
                }
            }

        } else {
            Debug.Log("shot from: " + startPos + " to: " + end);
            //geen hitpoint, verander daarom naar einde van ray
            hitPoint = end;
        }
        //geeft aan iedereen door dat er een gunray getekend moet worden
        shooter.GetComponent<PhotonView>().RPC("gunFX_RPC", PhotonTargets.AllBuffered, startPos, hitPoint);

        gunInfo.cooldown = 0.5f;


    }

    Transform FindClosestHitObject(Ray ray, out Vector3 hitPoint, Transform self, float range) {
        RaycastHit[] hits = Physics.RaycastAll(ray, range);
        Transform closestHit = null;
        float distance = 0;
        hitPoint = Vector3.zero;

        foreach (RaycastHit hit in hits) {
            if (hit.transform != self && (closestHit == null || hit.distance < distance)) {
                closestHit = hit.transform;
                distance = hit.distance;
                hitPoint = hit.point;
            }
        }
        return closestHit;
    }


}
