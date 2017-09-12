using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelManager : MonoBehaviour
{
    Toolbox toolbox;
    EventManager em;
    [SerializeField]
    LevelTrigger endTrigger;
    [SerializeField]
    Transform levelEndingPosition;
    [SerializeField]
    Transform playerStart;
    NavMeshAgent cutsceneNavAgent = null;

    bool endingCutscenePlaying = false;
    bool startingCutscenePlaying = false;
    float cutsceneNavAgentCompleteDistance = 0.1f;
    float endingCutscenWalkSpeed = 1f;
    ERobotType lastPossessedRobotType = ERobotType.NONE;

    private void Awake()
    {
        toolbox = FindObjectOfType<Toolbox>();
        em = toolbox.GetComponent<EventManager>();
    }

    private void OnEnable()
    {
        endTrigger.OnTriggerEntered += OnEndTriggerEntered;
        em.OnStartGame += OnStartGame;
        em.OnSpawnPlayer += OnSpawnPlayer;
    }

    private void OnDisable()
    {
        endTrigger.OnTriggerEntered -= OnEndTriggerEntered;
        em.OnStartGame -= OnStartGame;
        em.OnSpawnPlayer -= OnSpawnPlayer;
    }

    private void FixedUpdate()
    {
        if (endingCutscenePlaying)
        {
            if(cutsceneNavAgent != null)
            {
                if(cutsceneNavAgent.remainingDistance <= cutsceneNavAgentCompleteDistance)
                {
                    endingCutscenePlaying = false;
                    em.BroadcastLevelCompleted(lastPossessedRobotType);
                }
            }
        }
    }

    private void OnEndTriggerEntered(GameObject enteringObject)
    {
        if (enteringObject.GetComponent(typeof(IPossessable)))
        {
            IPossessable enteringPossessable = enteringObject.GetComponent<IPossessable>();
            if (enteringPossessable.GetIsPossessed())
            {
                Debug.Log("LevelManager: Player entered level ending trigger");
                lastPossessedRobotType = enteringPossessable.GetRobotType();
                em.BroadcastPauseActorsStateChange(true);
                //TODO: Play the ending animation (set navAgent destination to endPoint, close elevator doors)
                if (enteringPossessable.GetGameObject().GetComponent<NavMeshAgent>())
                {
                    cutsceneNavAgent = enteringPossessable.GetGameObject().GetComponent<NavMeshAgent>();
                    cutsceneNavAgent.speed = endingCutscenWalkSpeed;
                    cutsceneNavAgent.SetDestination(levelEndingPosition.position);
                    endingCutscenePlaying = true;
                }
            }
        }
    }

    private void OnSpawnPlayer(ERobotType robotTypeToSpawnPlayerAs)
    {
        GameObject newPlayer = Instantiate(Resources.Load("Prefabs/Player") as GameObject);
        newPlayer.transform.position = playerStart.position;
        newPlayer.transform.rotation = playerStart.rotation;

        GameObject newPossessable = null;

        switch (robotTypeToSpawnPlayerAs)
        {
            case ERobotType.DEBUG:
                newPossessable = Instantiate(Resources.Load("Prefabs/Poss_Mobile") as GameObject);
                break;
            case ERobotType.NONE:
                //newPossessable = Instantiate(Resources.Load("Prefabs/Mobile") as GameObject);
                break;
            case ERobotType.DEFAULT:
                newPossessable = Instantiate(Resources.Load("Prefabs/Poss_Mobile") as GameObject);
                break;
            case ERobotType.WORKER:
                //newPossessable = Instantiate(Resources.Load("Prefabs/Worker") as GameObject);
                break;
            case ERobotType.SMALL:
                newPossessable = Instantiate(Resources.Load("Prefabs/Poss_Small") as GameObject);
                break;
        }
        if (newPossessable != null)
        {
            newPossessable.transform.position = playerStart.position;
            newPossessable.transform.rotation = playerStart.rotation;
        }
    }

    private void OnStartGame()
    {
        //TODO: Play the starting animation
        Debug.Log("LevelManager: OnStartGame");
    }
}
