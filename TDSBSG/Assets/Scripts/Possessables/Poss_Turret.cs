using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poss_Turret : Poss_Stationary
{

    [SerializeField]
    Transform spawnTransform;
    [SerializeField]
    bool useDefaultProjectiles = false;
    [SerializeField]
    List<GameObject> projectiles = new List<GameObject>();
    List<Vector3> projectileOriginalPositions = new List<Vector3>();
    List<Vector3> projectileOriginalRotations = new List<Vector3>();
    int ammoCount = 10;
    [SerializeField]
    float shootForce = 1.0f;
    [SerializeField]
    float shootCooldown = 1.0f;
    float shootCooldownTimer;
    bool shooting = false;

    private void OnEnable()
    {
        shooting = false;

        if (!useDefaultProjectiles)
        {
            projectileOriginalPositions = new List<Vector3>();
            projectileOriginalRotations = new List<Vector3>();
            ammoCount = projectiles.Count;

            for (int i = 0; i < ammoCount; i++)
            {
                projectileOriginalPositions.Add(projectiles[i].transform.position);
                projectileOriginalRotations.Add(projectiles[i].transform.eulerAngles);
                projectiles[i].GetComponent<Rigidbody>().isKinematic = true;
            }

        }
    }

    private void FixedUpdate()
    {
        shootCooldownTimer -= Time.fixedDeltaTime;

        if(shooting)
        {
            Shoot();
        }
    }

    public override void GiveInput(EInputType newInput)
    {
        base.GiveInput(newInput);
        switch (newInput)
        {
            case EInputType.USE_KEYDOWN:
                shooting = true;
                break;
            case EInputType.USE_KEYUP:
                shooting = false;
                break;
            default:
                break;
        }
    }

    private void Reload()
    {
        int count = projectiles.Count;
        for (int i = 0; i < count; i++)
        {
            GameObject projectile = projectiles[i];
            projectile.GetComponent<Rigidbody>().isKinematic = true;
            projectile.transform.position = projectileOriginalPositions[i];
            projectile.transform.position = projectileOriginalPositions[i];
        }

        ammoCount = projectiles.Count;
    }

    private bool Shoot()
    {
        if (ammoCount > 0)
        {
            if (shootCooldownTimer < 0.0f)
            {
                shootCooldownTimer = shootCooldown;
                
                GameObject projectile = null;
                if (useDefaultProjectiles)
                {
                     projectile = Instantiate(Resources.Load("Prefabs/DefaultProjectile") as GameObject, spawnTransform.position, spawnTransform.rotation);
                }
                else
                {
                    projectile = projectiles[ammoCount - 1];
                    projectile.transform.position = spawnTransform.position;
                    projectile.transform.rotation = spawnTransform.rotation;
                    projectile.GetComponent<Rigidbody>().isKinematic = false;
                }

                projectile.GetComponent<Rigidbody>().AddForce(transform.forward * shootForce, ForceMode.Impulse);
                ammoCount--;

                return true;
            }
        }
        return false;
    }
}
