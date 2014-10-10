using UnityEngine;

public class GunController : MonoBehaviour {

    float cooldown = 0;
    float damage = 25f;
    float fireRate = 0.25f;
    float spreadFactor = 0.05f;

    FXManager fxManager;
    Vector3 startPos;
    GameObject cam;

    public Texture2D crosshairTexture;
    Rect position;

    void Start() {
        Debug.Log("start GunController");
        fxManager = FXManager.instance;
        cam = gameObject.transform.FindChild("PlayerCamera").gameObject;
    }

    void Update() {
        cooldown -= Time.deltaTime;

        if (Input.GetButton("Fire1")) {
            fire();
        }

        position = new Rect((Screen.width - crosshairTexture.width) / 2, (Screen.height -
            crosshairTexture.height) / 2, crosshairTexture.width, crosshairTexture.height);
    }

    void OnGUI() {
        GUI.DrawTexture(position, crosshairTexture);
    }

    void fire() {
        startPos = cam.transform.Find("GunTip").transform.position;
        Debug.Log("Fire");
        if (cooldown > 0) {
            return;
        }
        Vector3 direction = Camera.main.transform.forward;
        direction.x += Random.Range(-spreadFactor, spreadFactor);
        direction.y += Random.Range(-spreadFactor, spreadFactor);
        direction.z += Random.Range(-spreadFactor, spreadFactor);

        Ray ray = new Ray(Camera.main.transform.position, direction);
        Transform hitTransform;
        Vector3 hitPoint;
        Vector3 end = ray.origin + (ray.direction * 100);

        hitTransform = FindClosestHitObject(ray, out hitPoint);

        if (hitTransform != null) {
            if (fxManager != null) {
                DoGunFX(hitPoint);
            }

            Debug.Log("We hit: " + hitTransform.name);

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
                    pv.RPC("TakeDamage", PhotonTargets.AllBuffered, damage, PhotonNetwork.player.name);
                }
            }
        } else {
            //niets geraakt
            if (fxManager != null) {
                DoGunFX(end);
            }
        }

        cooldown = fireRate;
    }

    void DoGunFX(Vector3 hitPoint) {
        fxManager.SniperBulletFX(startPos, hitPoint);
    }

    Transform FindClosestHitObject(Ray ray, out Vector3 hitPoint) {
        RaycastHit[] hits = Physics.RaycastAll(ray);
        Transform closestHit = null;
        float distance = 0;
        hitPoint = Vector3.zero;

        foreach (RaycastHit hit in hits) {
            if (hit.transform != this.transform && (closestHit == null || hit.distance < distance)) {
                closestHit = hit.transform;
                distance = hit.distance;
                hitPoint = hit.point;
            }
        }
        return closestHit;
    }
}


