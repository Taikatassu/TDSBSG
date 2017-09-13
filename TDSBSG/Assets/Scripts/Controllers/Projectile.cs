using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour {

    [SerializeField]
    bool reusable = false;
    //Collider capsale;
    //Rigidbody rb;

    private void OnCollisionEnter(Collision collision) {
        EnemyBase enemy = collision.gameObject.GetComponent<EnemyBase>();

        if (enemy) {
            enemy.KnockOut();
        }

        GameObject splash = Instantiate(Resources.Load("ParticleEffect/SplashParticle") as GameObject, 
            transform.position, transform.rotation);
        Destroy(splash, 3.0f);

        if (reusable)
        {
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
