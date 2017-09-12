using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region References & variables
    Toolbox toolbox;
    EventManager em;
    PossessableInfo possInfo;
    IPossessable primaryPossession;
    List<IPossessable> secondaryPossessions = new List<IPossessable>();
    Transform primaryPossessionTransform;
    public LayerMask selectionMask;
    public LayerMask possessBlockinMask;

    bool controllable = false; //Set to false during cutscenes etc., true while player is supposed to be able to move around
    float possessionRange = 5;
    #endregion

    private void Awake()
    {
        toolbox = FindObjectOfType<Toolbox>();
        em = toolbox.GetComponent<EventManager>();
        possInfo = toolbox.GetComponent<PossessableInfo>();
    }

    private void OnEnable()
    {
        em.OnInitializeGame += OnInitializeGame;
        em.OnStartGame += OnStartGame;
        em.OnInputEvent += OnInputEvent;
        em.OnRequestPlayerReference += OnRequestPlayerReference;
        em.OnMouseInputEvent += OnMouseInputEvent;
        em.OnPauseActorsStateChange += OnPauseActorsStateChange;
    }

    private void OnDisable()
    {
        em.OnInitializeGame -= OnInitializeGame;
        em.OnStartGame -= OnStartGame;
        em.OnInputEvent -= OnInputEvent;
        em.OnRequestPlayerReference -= OnRequestPlayerReference;
        em.OnMouseInputEvent -= OnMouseInputEvent;
        em.OnPauseActorsStateChange -= OnPauseActorsStateChange;
    }

    private void OnInitializeGame()
    {
        ResetAll();
    }

    private void OnStartGame()
    {
        TryPossessClosestPossessable();
        controllable = true;
    }

    private void OnPauseActorsStateChange(bool newState)
    {
        ChangeControllableState(!newState);
    }
    private GameObject OnRequestPlayerReference()
    {
        return gameObject;
    }

    private bool TryPossessClosestPossessable()
    {
        if (possInfo.possessables.Count > 0)
        {
            IPossessable closestPossessable = possInfo.possessables[0];
            float distanceToClosest = -1;
            bool first = true;
            int count = possInfo.possessables.Count;
            for (int i = 0; i < count; i++)
            {
                if (possInfo.possessables[i] != primaryPossession)
                {
                    float distanceToThis = (transform.position -
                        possInfo.possessables[i].GetGameObject().transform.position).magnitude;
                    if (first || distanceToThis < distanceToClosest)
                    {
                        first = false;
                        closestPossessable = possInfo.possessables[i];

                        distanceToClosest = distanceToThis;
                    }
                }
            }

            if (distanceToClosest > possessionRange)
            {
                return false;
            }

            PossessPossessable(closestPossessable);
            return true;
        }
        Debug.LogWarning("No possessables found");
        return false;
    }

    private void ChangeControllableState(bool newState)
    {
        controllable = newState;
    }


    private void FixedUpdate()
    {
        if (primaryPossession != null)
        {
            transform.position = primaryPossessionTransform.position;
        }
    }

    private void PossessPossessable(IPossessable newPossession)
    {
        switch (newPossession.GetPossessableType())
        {
            case EPossessableType.PRIMARY:
                if (primaryPossession != null)
                {
                    primaryPossession.UnPossess();
                }
                newPossession.Possess();
                primaryPossession = newPossession;
                primaryPossessionTransform = primaryPossession.GetGameObject().transform;
                break;
            case EPossessableType.SECONDARY:
                newPossession.Possess();
                secondaryPossessions.Add(newPossession);
                break;
            default:
                break;
        }
    }

    private void OnMouseInputEvent(int button, bool down, Vector3 mousePosition)
    {
        if (controllable)
        {
            if (button == 0 && down)
            {
                Ray ray = Camera.main.ScreenPointToRay(mousePosition);
                RaycastHit hit;
                Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.red, 5f);
                if (Physics.Raycast(ray, out hit, 1000f, selectionMask))
                {
                    if (hit.collider.GetComponent(typeof(IPossessable)))
                    {
                        IPossessable hitPossessable = hit.collider.GetComponent<IPossessable>();
                        Vector3 hitObjectDirection = hit.collider.transform.position - transform.position;
                        Vector3 raycastOrigin = transform.position;
                        raycastOrigin.y++;

                        if (primaryPossession.GetConnectedPossessablesList().Contains(hitPossessable))
                        {
                            PossessPossessable(hitPossessable);
                            return;
                        }
                        else if (Physics.Raycast(raycastOrigin, hitObjectDirection, out hit, possessionRange, possessBlockinMask))
                        {
                            if (hit.collider.gameObject == hitPossessable.GetGameObject())
                            {
                                PossessPossessable(hitPossessable);
                                return;
                            }
                        }
                    }
                    else if (hit.collider.GetComponent(typeof(Interactable)))
                    {
                        ////Try to interact
                        Debug.Log("Hit InteractableObject");

                        Interactable currentInteractableObject = hit.collider.GetComponent<Interactable>();

                        if (currentInteractableObject.ContainPermissionList(primaryPossession.GetRobotType()))
                        {
                            primaryPossession.SetInteractableObject(currentInteractableObject);
                            currentInteractableObject.StartInteraction(primaryPossession);
                        }
                    }
                }
            }
            else if (button == 1 && down)
            {
                Interactable currentInteractableObject = primaryPossession.GetInteractableObject();
                if (currentInteractableObject)
                {
                    currentInteractableObject.EndInteraction(primaryPossession);
                    primaryPossession.SetInteractableObject(null);
                }
            }
        }
    }

    private void OnInputEvent(EInputType newInput)
    {
        //if (newInput == EInputType.POSSESS_KEYDOWN)
        //{
        //    TryPossessClosestPossessable();
        //}

        /*else */
        if (controllable)
        {
            if (primaryPossession != null)
            {
                primaryPossession.GiveInput(newInput);
            }
        }

        //if (currentSecondaryPossessions.Count > 0)
        //{
        //    for (int i = 0; i < currentSecondaryPossessions.Count; i++)
        //    {
        //        currentSecondaryPossessions[i].GiveInput(newInput);
        //    }
        //}

        //TODO: Handle player only inputs if neccessary
    }

    private void ResetAll()
    {
        if (secondaryPossessions.Count > 0)
        {
            for (int i = 0; i < secondaryPossessions.Count; i++)
            {
                if (secondaryPossessions[i] != null)
                {
                    secondaryPossessions[i].UnPossess();
                }
            }
        }

        secondaryPossessions = new List<IPossessable>();
        primaryPossession = null;
        primaryPossessionTransform = null;
    }
}
