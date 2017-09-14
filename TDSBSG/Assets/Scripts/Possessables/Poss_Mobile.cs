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
    protected Rigidbody rb;
    protected List<GameObject> disobeyingList = new List<GameObject>();
    List<IPossessable> connectedPossessables = new List<IPossessable>();
    NavMeshAgent navMeshAgent;
    protected Interactable interactableObject;
    protected Vector3 lookDirection = Vector3.zero;
    protected Transform cameraRotatorTransform;
    [SerializeField]
    protected SpriteAnimationController spriteController;

    protected bool canMove = false;
    protected bool isPossessed = false;
    protected bool isDisobeying = false;
    protected bool movingUp = false;
    protected bool movingDown = false;
    protected bool movingRight = false;
    protected bool movingLeft = false;
    protected bool interactionPause = false;
    readonly EPossessableType possessableType = EPossessableType.PRIMARY;
    [SerializeField]
    ERobotType robotType = ERobotType.DEFAULT;
    protected float defaultMovementSpeed = 150f;
    protected float currentMovementSpeedMultiplier = 1.5f;
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

    protected virtual void OnEnable()
    {
        possInfo.possessables.Add(this);

        em.OnInitializeGame += OnInitializeGame;
        em.OnPauseActorsStateChange += OnPauseActorsStateChange;
        em.OnDisobeyingDetected += OnDisobeyingDetected;

    }

    protected virtual void OnDisable()
    {
        possInfo.possessables.Remove(this);

        em.OnInitializeGame -= OnInitializeGame;
        em.OnPauseActorsStateChange -= OnPauseActorsStateChange;
        em.OnDisobeyingDetected -= OnDisobeyingDetected;
    }

    protected virtual void OnInitializeGame()
    {
        toolbox = FindObjectOfType<Toolbox>();
        em = toolbox.GetComponent<EventManager>();
        possInfo = toolbox.GetComponent<PossessableInfo>();
        rb = GetComponent<Rigidbody>();
        tag = "Possessable"; //Add this to all possessables
        disobeyingList = new List<GameObject>();
        connectedPossessables = new List<IPossessable>();
        cameraRotatorTransform = em.BroadcastRequestCameraReference().transform.parent;

        if (spriteController)
        {
            spriteController.SetAnimationState(EAnimationState.IDLE);
        }

        canMove = true;
        ResetAll();
    }

    private void OnPauseActorsStateChange(bool newState)
    {
        if (newState)
        {
            movingUp = false;
            movingDown = false;
            movingRight = false;
            movingLeft = false;
            if (rb != null)
            {
                rb.isKinematic = true;
            }
        }
        else
        {
            movingUp = false;
            movingDown = false;
            movingRight = false;
            movingLeft = false;
            if (isPossessed && rb != null)
            {
                rb.isKinematic = false;
            }
        }
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
        if (!disobeyingList.Contains(go))
        {
            disobeyingList.Add(go);
        }
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

    public virtual void GiveInput(EInputType newInput)
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

    public Interactable GetInteractableObject()
    {
        return interactableObject;
    }

    public void SetInteractableObject(Interactable interactableObject)
    {
        this.interactableObject = interactableObject;
    }

    public void ChangeCanMoveState(bool newState)
    {
        canMove = newState;
        rb.velocity = Vector3.zero;
    }
    #endregion

    protected virtual void FixedUpdate()
    {
        rb.velocity = Vector3.zero;
        //Debug.Log(gameObject.name + ", disobeyingList.Count: " + disobeyingList.Count);

        if (Input.GetKey(KeyCode.LeftShift))
		{
			currentMovementSpeedMultiplier = 3;
		}
		else
		{
			currentMovementSpeedMultiplier = 1.5f;
		}

        if (disobeyingList.Count > 0)
        {
            isDisobeying = true;
        }
        else
        {
            isDisobeying = false;
        }
        if (!interactionPause)
        {
            if (canMove)
            {
                if (isPossessed)
                {
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

                    Vector3 movementVelocity; // = new Vector3(moveXValue, 0, moveZValue).normalized;
                    movementVelocity = (cameraRotatorTransform.forward * moveZValue
                        + cameraRotatorTransform.right * moveXValue).normalized;

                    movementVelocity *= defaultMovementSpeed * currentMovementSpeedMultiplier
                        * Time.fixedDeltaTime;

                    rb.velocity = movementVelocity;
                    if (rb.velocity != Vector3.zero)
                    {
                        lookDirection = Quaternion.LookRotation(rb.velocity, Vector3.up).eulerAngles;
                    }
                }
            }
        }

        if (rb.velocity != Vector3.zero)
        {
            if (spriteController)
            {
                spriteController.SetAnimationState(EAnimationState.WALK);
            }
        }
        else
        {
            if (spriteController)
            {
                spriteController.SetAnimationState(EAnimationState.IDLE);
            }
        }

        transform.rotation = Quaternion.Euler(lookDirection);
    }

    public void SetDestination(Vector3 newDest)
    {
        navMeshAgent.SetDestination(newDest);
    }

    private void OnDisobeyingDetected(ERobotType detectedRobotType, IPossessable detectedRobot) {
        if(this == (UnityEngine.Object)detectedRobot) {
            // cant move
        }
    }
}
