﻿using UnityEngine;
using System.Collections;

public class ResourceManager : MonoBehaviour {
    public float maxHealth = 100f;
    float currentHitPoints;

	void Start () {
        currentHitPoints = maxHealth;
	}
	
    public void TakeDamage(float amt) {
        currentHitPoints -= amt;
        if(currentHitPoints <= 0){
            Die();
        }
    }

    void Die() {
        Destroy(gameObject);
    }
}
