using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField]
    bool reusable = false;
    bool isActive = false;

    public void SetIsActive(bool newState)
    {
        isActive = newState;
    }

    private void OnEnable()
    {
        isActive = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isActive)
        {
            EnemyBase enemy = collision.gameObject.GetComponent<EnemyBase>();

            if (enemy)
            {
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
}
