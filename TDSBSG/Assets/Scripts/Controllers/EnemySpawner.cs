using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PatrolPoint))]
public class EnemySpawner : MonoBehaviour
{
    Toolbox toolbox;
    EventManager em;
    public GameObject enemyToSpawn;
    public List<PatrolPoint> patrolPoints = new List<PatrolPoint>();
     
    private void Awake()
    {
        toolbox = FindObjectOfType<Toolbox>();
        em = toolbox.GetComponent<EventManager>();
    }

    private void OnEnable()
    {
        em.OnInitializeGame += OnInitializeGame;
    }

    private void OnDisable()
    {
        em.OnInitializeGame -= OnInitializeGame;
    }
    
    private void OnInitializeGame()
    {
        SpawnEnemy(enemyToSpawn);
    }

    private void SpawnEnemy(GameObject enemyType)
    {
        GameObject newEnemy = Instantiate(enemyType, transform.position, transform.rotation);
        patrolPoints.Insert(0, GetComponent<PatrolPoint>());
        newEnemy.GetComponent<EnemyBase>().SetPatrolPoints(patrolPoints);
        newEnemy.GetComponent<EnemyBase>().InitializeEnemy();
    }

}
