using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poss_CleanerBot : MonoBehaviour, IPossessable
{
    #region References & variables
    Toolbox toolbox;
    EventManager em;
    PossessableInfo possInfo;
    Rigidbody rb;

    bool isPossessed = false;
    bool movingUp = false;
    bool movingDown = false;
    bool movingRight = false;
    bool movingLeft = false;
    readonly EPossessableType possessableType = EPossessableType.PRIMARY;
	readonly ERobotType robotType = ERobotType.CLEANING;
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
    }

    private void OnEnable()
    {
        possInfo.possessables.Add(this);

        em.OnGameStarted += OnGameStarted;
    }

    private void OnDisable()
    {
        possInfo.possessables.Remove(this);

        em.OnGameStarted -= OnGameStarted;
    }

    private void OnGameStarted()
    {
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

    public EPossessableType GetPossessableType()
    {
        return possessableType;
    }
	public ERobotType GetRobotType() {
		return robotType;
	}

	public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void Possess()
    {
        isPossessed = true;
        if(rb != null)
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
        if(rb != null)
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
    #endregion

    private void FixedUpdate()
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
        //
        Vector3 movementVelocity = new Vector3(moveXValue, 0, moveZValue).normalized;
        movementVelocity *= defaultMovementSpeed * currentMovementSpeedMultiplier 
            * Time.fixedDeltaTime;
        rb.velocity = movementVelocity;

    }

	
}
