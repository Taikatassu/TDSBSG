using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    Collider capsale;
    Rigidbody rb;

    private void OnCollisionEnter(Collision collision) {
        EnemyBase enemy = collision.gameObject.GetComponent<EnemyBase>();
        if (enemy) {
            // call enemy  knockOut
            enemy.KnockOut();
        }
        GameObject splash = Instantiate(Resources.Load("ParticleEffect/SplashParticle") as GameObject, transform.position, transform.rotation);
        Destroy(splash, 3.0f);
        Destroy(gameObject);
    }
}
