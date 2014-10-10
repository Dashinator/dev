using UnityEngine;
using System.Collections.Generic;

public class FXManager : MonoBehaviour {
    private static FXManager _instance;
    public GameObject SniperBulletFXPrefab;

    public int pooledAmount = 20;
    List<GameObject> FXList;

    public static FXManager instance {
        get {
            if (_instance == null) {
                //Debug.Log("create FXManager");
                _instance = GameObject.FindObjectOfType<FXManager>();
                _instance.createPool();
            }
            //Debug.Log("return FXManager");
            return _instance;
        }
    }

    private void createPool() {
        FXList = new List<GameObject>();
        for (int i = 0; i < pooledAmount; i++) {
            GameObject obj = (GameObject)Instantiate(SniperBulletFXPrefab);
            obj.SetActive(false);
            FXList.Add(obj);
            Debug.Log("made FX");
        }
    }

    [RPC]
    public void SniperBulletFX(Vector3 startPos, Vector3 endPos) {
        for (int i = 0; i < FXList.Count; i++) {
            if (!FXList[i].activeInHierarchy) {

                //Debug.Log("SniperBulletFX(" + startPos.ToString() + ", " + endPos.ToString() + ")");
                if (SniperBulletFXPrefab != null) {
                    //GameObject sniperFX = (GameObject)Instantiate(SniperBulletFXPrefab, startPos, Quaternion.LookRotation(endPos - startPos));
                    FXList[i].transform.position = startPos;
                    FXList[i].transform.rotation = Quaternion.LookRotation(endPos - startPos);
                    LineRenderer lr = FXList[i].transform.Find("LineFX").GetComponent<LineRenderer>();
                    if (lr != null) {
                        lr.SetPosition(0, startPos);
                        lr.SetPosition(1, endPos);
                    } else {
                        Debug.LogError("SniperBulletFXPrefab is missing lr");
                    }
                } else {
                    Debug.LogError("SniperBuleltFXPrefab is missing");
                }

                FXList[i].SetActive(true);
                FXList[i].transform.FindChild("NuzzleFlash").gameObject.GetComponent<ParticleSystem>().enableEmission = true;
                break;
            }
        }
    }
}
