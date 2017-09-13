using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poss_Stationary : MonoBehaviour, IPossessable {
    #region References & variables
    Toolbox toolbox;
    EventManager em;
    PossessableInfo possInfo;
    Rigidbody rb;
    List<GameObject> disobeyingList = new List<GameObject>();
    List<IPossessable> connectedPossessables = new List<IPossessable>();
    protected Interactable interactableObject;

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
    }

    private void OnDisable()
    {
        possInfo.possessables.Remove(this);

        em.OnInitializeGame -= OnInitializeGame;
    }

    protected virtual void OnInitializeGame()
    {
        toolbox = FindObjectOfType<Toolbox>();
        em = toolbox.GetComponent<EventManager>();
        possInfo = toolbox.GetComponent<PossessableInfo>();
        rb = GetComponent<Rigidbody>();
        tag = "Possessable"; //Add this to all possessables
        disobeyingList = new List<GameObject>();

        //ResetAll();
    }

    private void ResetAll()
    {
        UnPossess();
        connectedPossessables = new List<IPossessable>();
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
    }

    public void UnPossess()
    {
        isPossessed = false;
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    public Interactable GetInteractableObject() {
        return interactableObject;
    }

    public void SetInteractableObject(Interactable interactableObject) {
        this.interactableObject = interactableObject;
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
}
