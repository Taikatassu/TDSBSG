﻿using System.Collections;
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
    float cutsceneNavAgentCompleteDistance = 0.1f;
    [SerializeField]
    float endingCutscenWalkSpeed = 1.5f;
    ERobotType lastPossessedRobotType = ERobotType.NONE;

    private void OnEnable()
    {
        toolbox = FindObjectOfType<Toolbox>();
        em = toolbox.GetComponent<EventManager>();

        endTrigger.OnTriggerEntered += OnEndTriggerEntered;
        em.OnSpawnPlayer += OnSpawnPlayer;
    }

    private void OnDisable()
    {
        endTrigger.OnTriggerEntered -= OnEndTriggerEntered;
        em.OnSpawnPlayer -= OnSpawnPlayer;
    }

    private void FixedUpdate()
    {
        if (endingCutscenePlaying)
        {
            if (cutsceneNavAgent != null)
            {
                if (cutsceneNavAgent.remainingDistance <= cutsceneNavAgentCompleteDistance)
                {
                    endingCutscenePlaying = false;
                    int currentSceneIndex = em.BroadcastRequestCurrentSceneIndex();
                    em.BroadcastLevelCompleted(currentSceneIndex, lastPossessedRobotType);
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
                lastPossessedRobotType = enteringPossessable.GetRobotType();
                em.BroadcastPauseActorsStateChange(true);
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
        Instantiate(Resources.Load("Prefabs/Player") as GameObject, playerStart.position, playerStart.rotation);
        
        switch (robotTypeToSpawnPlayerAs)
        {
            case ERobotType.DEBUG:
                Instantiate(Resources.Load("Prefabs/Poss_Mobile") as GameObject, playerStart.position, playerStart.rotation);
                break;
            case ERobotType.NONE:
                //Instantiate(Resources.Load("Prefabs/Mobile") as GameObject, playerStart.position, playerStart.rotation);
                break;
            case ERobotType.DEFAULT:
                Instantiate(Resources.Load("Prefabs/Poss_CleanerBot") as GameObject, playerStart.position, playerStart.rotation);
                break;
            case ERobotType.WORKER:
                Instantiate(Resources.Load("Prefabs/Poss_Forklift") as GameObject, playerStart.position, playerStart.rotation);
                break;
            case ERobotType.SMALL:
                Instantiate(Resources.Load("Prefabs/Poss_MiniCleaner") as GameObject, playerStart.position, playerStart.rotation);
                break;
        }
    }
}
