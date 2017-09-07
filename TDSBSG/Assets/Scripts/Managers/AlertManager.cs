using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertManager : MonoBehaviour
{
    public static AlertManager instance;
    Toolbox toolbox;
    EventManager em;
    List<EnemyBase> enemyRegister = new List<EnemyBase>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);


        toolbox = FindObjectOfType<Toolbox>();
        em = toolbox.GetComponent<EventManager>();
    }

    private void OnEnable()
    {
        em.OnInitializeGame += OnInitializeGame;
        em.OnRegisterEnemy += OnRegisterEnemy;
        em.OnStartAlarm += OnStartAlarm;
    }

    private void OnDisable()
    {
        em.OnInitializeGame -= OnInitializeGame;
        em.OnRegisterEnemy -= OnRegisterEnemy;
        em.OnStartAlarm -= OnStartAlarm;
    }

    private void OnInitializeGame()
    {
        enemyRegister = new List<EnemyBase>();
    }

    private void OnRegisterEnemy(GameObject newEnemy)
    {
        enemyRegister.Add(newEnemy.GetComponent<EnemyBase>());
    }

    private void OnStartAlarm()
    {
        //TODO: send alarm to all registered enemies
    }

}
