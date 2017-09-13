using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poss_Turret : Poss_Stationary {

    [SerializeField]
    Transform spawnPostion;
    [SerializeField]
    int ammoCount = 10;
    [SerializeField]
    float shootForce = 1.0f;
    [SerializeField]
    float cooldownDuration = 1.0f;
    float cooldownTimer;

    private void FixedUpdate() {
        cooldownTimer -= Time.fixedDeltaTime;
    }

    public override void GiveInput(EInputType newInput) {
        base.GiveInput(newInput);
        switch (newInput) {
            case EInputType.USE_KEYDOWN:
            Shoot();
            break;
            case EInputType.USE_KEYUP:
            break;
            default:
            break;
        }
    }

    private bool Shoot() {
        if (ammoCount > 0) {
            if (cooldownTimer < 0.0f) {
                --ammoCount;
                cooldownTimer = cooldownDuration;
                GameObject projectile = Instantiate(Resources.Load("Prefabs/Projectile") as GameObject, spawnPostion.position, spawnPostion.rotation);
                projectile.GetComponent<Rigidbody>().AddForce(transform.forward * shootForce, ForceMode.Impulse);
                return true;
            }
        }
        return true;
    }
}
