using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poss_Stationary : MonoBehaviour, IPossessable
{
    #region References & variables
    Toolbox toolbox;
    EventManager em;
    PossessableInfo possInfo;
    Rigidbody rb;
    List<GameObject> disobeyingList = new List<GameObject>();
    List<IPossessable> connectedPossessables = new List<IPossessable>();
    protected Interactable interactableObject;
    protected Poss_ConnectedMaster connectionMaster = null;

    bool canMove = false;
    bool isPossessed = false;
    bool isDisobeying = false;
    readonly EPossessableType possessableType = EPossessableType.PRIMARY;
    readonly ERobotType robotType = ERobotType.DEFAULT;
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
        gameObject.layer = LayerMask.NameToLayer("Possessable"); //Add this to all possessables
    }

    private void OnEnable()
    {
        possInfo.possessables.Add(this);

        em.OnInitializeGame += OnInitializeGame;
        em.OnDisobeyingDetected += OnDisobeyingDetected;
    }

    private void OnDisable()
    {
        possInfo.possessables.Remove(this);

        em.OnInitializeGame -= OnInitializeGame;
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
        canMove = true;

        ResetAll();
    }

    private void ResetAll()
    {
        UnPossess();
    }

    public void SetConnectionMaster(Poss_ConnectedMaster newConnectionMaster)
    {
        connectionMaster = newConnectionMaster;
    }

    #region IPossessable implementation
    public GameObject GetGameObject()
    {
        return gameObject;
    }

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

    public virtual void GiveInput(EInputType newInput)
    {
        switch (newInput)
        {
            default:
                break;
        }
    }

    public void Possess()
    {
        isPossessed = true;
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        if(connectionMaster != null)
        {
            connectionMaster.SetConnectionIndicatorState(true);
        }
    }

    public void UnPossess()
    {
        isPossessed = false;
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        if (connectionMaster != null)
        {
            connectionMaster.SetConnectionIndicatorState(false);
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

    private void FixedUpdate()
    {
        if (disobeyingList.Count > 0)
        {
            isDisobeying = true;
        }
        else
        {
            isDisobeying = false;
        }
    }

    private void OnDisobeyingDetected(ERobotType detectedRobotType, IPossessable detectedRobot)
    {
        if (this == (UnityEngine.Object)detectedRobot)
        {
            // cant move
        }
    }
}
