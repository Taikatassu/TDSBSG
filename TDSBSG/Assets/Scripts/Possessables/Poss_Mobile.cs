using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Poss_Mobile : MonoBehaviour, IPossessable
{
    #region References & variables
    Toolbox toolbox;
    EventManager em;
    PossessableInfo possInfo;
    Rigidbody rb;
    List<GameObject> disobeyingList = new List<GameObject>();
    List<IPossessable> connectedPossessables = new List<IPossessable>();
    NavMeshAgent navMeshAgent;
    protected Interactable interactableObject;

    bool isPossessed = false;
    bool isDisobeying = false;
    bool movingUp = false;
    bool movingDown = false;
    bool movingRight = false;
    bool movingLeft = false;
    readonly EPossessableType possessableType = EPossessableType.PRIMARY;
    readonly ERobotType robotType = ERobotType.DEFAULT;
    float defaultMovementSpeed = 150f;
    float currentMovementSpeedMultiplier = 1;
    #endregion

    private void Awake()
    {
        toolbox = FindObjectOfType<Toolbox>();
        em = toolbox.GetComponent<EventManager>();
        possInfo = toolbox.GetComponent<PossessableInfo>();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        tag = "Possessable"; //Add this to all possessables
        gameObject.layer = LayerMask.NameToLayer("Possessable");
    }

    private void OnEnable()
    {
        possInfo.possessables.Add(this);

        em.OnInitializeGame += OnInitializeGame;
    }

    private void OnDisable()
    {
        possInfo.possessables.Remove(this);

        em.OnInitializeGame -= OnInitializeGame;
    }

    private void OnInitializeGame()
    {
        toolbox = FindObjectOfType<Toolbox>();
        em = toolbox.GetComponent<EventManager>();
        possInfo = toolbox.GetComponent<PossessableInfo>();
        rb = GetComponent<Rigidbody>();
        tag = "Possessable"; //Add this to all possessables
        disobeyingList = new List<GameObject>();
        connectedPossessables = new List<IPossessable>();

        ResetAll();
    }

    private void ResetAll()
    {
        UnPossess();
        currentMovementSpeedMultiplier = 1;
    }

    #region IPossessable implementation
    public bool GetIsPossessed()
    {
        return isPossessed;
    }

    public bool GetIsDisobeying()
    {
        return isDisobeying;
    }

    public void AddDisobeyingToList(GameObject go)
    {
        disobeyingList.Add(go);
    }

    public void RemoveDisobeyingFromList(GameObject go)
    {
        if (disobeyingList.Contains(go))
        {
            disobeyingList.Remove(go);
        }
        else
        {
            Debug.LogWarning("RemoveDisobeyingFromList: Object to remove not found!");
        }
    }

    public void AddToConnectedPossessablesList(IPossessable newConnection)
    {
        connectedPossessables.Add(newConnection);
    }

    public List<IPossessable> GetConnectedPossessablesList()
    {
        return connectedPossessables;
    }

    public EPossessableType GetPossessableType()
    {
        return possessableType;
    }
    public ERobotType GetRobotType()
    {
        return robotType;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void Possess()
    {
        isPossessed = true;
        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }

    public void UnPossess()
    {
        isPossessed = false;
        movingUp = false;
        movingDown = false;
        movingRight = false;
        movingLeft = false;
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    public void GiveInput(EInputType newInput)
    {
        switch (newInput)
        {
            case EInputType.MOVEUP_KEYDOWN:
                movingUp = true;
                break;
            case EInputType.MOVEDOWN_KEYDOWN:
                movingDown = true;
                break;
            case EInputType.MOVERIGHT_KEYDOWN:
                movingRight = true;
                break;
            case EInputType.MOVELEFT_KEYDOWN:
                movingLeft = true;
                break;
            case EInputType.MOVEUP_KEYUP:
                movingUp = false;
                break;
            case EInputType.MOVEDOWN_KEYUP:
                movingDown = false;
                break;
            case EInputType.MOVERIGHT_KEYUP:
                movingRight = false;
                break;
            case EInputType.MOVELEFT_KEYUP:
                movingLeft = false;
                break;
            default:
                break;
        }
    }

    public Interactable GetInteractableObject() {
        return interactableObject;
    }

    public void SetInteractableObject(Interactable interactableObject) {
        this.interactableObject = interactableObject;
    }
    #endregion

    protected virtual void FixedUpdate()
    {
        if (disobeyingList.Count > 0)
        {
            isDisobeying = true;
        }
        else
        {
            isDisobeying = false;
        }

        //Movement by player
        float moveZValue = 0;
        float moveXValue = 0;

        if (movingUp)
        {
            moveZValue++;
        }
        if (movingDown)
        {
            moveZValue--;
        }

        if (movingRight)
        {
            moveXValue++;
        }
        if (movingLeft)
        {
            moveXValue--;
        }
        //
        Vector3 movementVelocity = new Vector3(moveXValue, 0, moveZValue).normalized;
        movementVelocity *= defaultMovementSpeed * currentMovementSpeedMultiplier
            * Time.fixedDeltaTime;
        rb.velocity = movementVelocity;

    }

   public void SetDestination(Vector3 newDest) {
        navMeshAgent.SetDestination(newDest);
    }
}
