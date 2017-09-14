using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreenController : MonoBehaviour
{
    Toolbox toolbox;
    EventManager em;
    [SerializeField]
    Animator loadAnimation;
    [SerializeField]
    GameObject cleanerBotAnimation;
    [SerializeField]
    GameObject miniCleanerAnimation;

    ERobotType robotTypeToDisplay = ERobotType.NONE;
    [SerializeField]
    float loadingScreenDuration = 2f;
    float loadingScreenTimer = 0f;
    bool loading = false;

    private void OnEnable()
    {
        Debug.Log("LoadingScreenController: OnEnable");
        toolbox = FindObjectOfType<Toolbox>();
        em = toolbox.GetComponent<EventManager>();

        ERobotType robotType = em.BroadcastRequestSpawningRobotType();
        SetRobotDisplay(robotType);
        loadingScreenTimer = loadingScreenDuration;
        loading = true;
    }

    private void SetRobotDisplay(ERobotType newType)
    {
        Debug.Log("LoadingScreenController: SetRobotDisplay");
        robotTypeToDisplay = newType;

        switch (robotTypeToDisplay)
        {
            case ERobotType.DEBUG:
                break;
            case ERobotType.NONE:
                break;
            case ERobotType.DEFAULT:
                cleanerBotAnimation.SetActive(true);
                miniCleanerAnimation.SetActive(false);
                break;
            case ERobotType.WORKER:
                break;
            case ERobotType.SMALL:
                cleanerBotAnimation.SetActive(false);
                miniCleanerAnimation.SetActive(true);
                break;
            default:
                break;
        }
    }

    
}
