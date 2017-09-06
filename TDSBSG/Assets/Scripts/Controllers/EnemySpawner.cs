using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    Toolbox toolbox;
    EventManager em;
    public GameObject enemyToSpawn;
    public List<Transform> patrolPoints = new List<Transform>();

    private void Awake()
    {
        toolbox = FindObjectOfType<Toolbox>();
        em = toolbox.GetComponent<EventManager>();
    }

    private void OnEnable()
    {
        em.OnGameStarted += OnGameStarted;
    }

    private void OnDisable()
    {
        em.OnGameStarted -= OnGameStarted;
    }
    
    private void OnGameStarted()
    {
        SpawnEnemy(enemyToSpawn);
    }

    private void SpawnEnemy(GameObject enemyType)
    {
        GameObject newEnemy = Instantiate(enemyType, transform.position, transform.rotation);
        patrolPoints.Insert(0, transform);
        newEnemy.GetComponent<EnemyBase>().SetPatrolPoints(patrolPoints);
        newEnemy.GetComponent<EnemyBase>().InitializeEnemy();
    }

}
