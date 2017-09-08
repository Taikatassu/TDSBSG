using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityManager : MonoBehaviour
{
    #region References & variables
    public static SecurityManager _instance;

    Toolbox toolbox;
    EventManager em;

    int securityPoints = 0; // Current total security points
    int securityTier = 0; // Current security tier
    [SerializeField]
    int numOfTiers = 5; // Maximum security tier (tiers will range between 0 and this value)
    [SerializeField]
    int pointPerTier = 100; // How many points are required for a the security tier to progress to the next tier
    [SerializeField, Range(0, 100)]
    int roomEnterIncrease = 20; // Value of Increase for entering restricted a room
    [SerializeField, Range(0, 100)]
    int doorEnterIncrease = 20; // Value of Increase for passing through a restricted door

    int maximumOfSecurityPoint = -1;
    #endregion

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        toolbox = FindObjectOfType<Toolbox>();
        if(toolbox.GetComponent<EventManager>())
        {
            Debug.Log("find Event Manager");
        }
        em = toolbox.GetComponent<EventManager>();

        maximumOfSecurityPoint = numOfTiers * pointPerTier;
    }

    private void OnEnable()
    {
        em.OnRoomEntered += OnRoomEntered;
        em.OnDoorEntered += OnDoorEntered;
    }

    private void OnDisable()
    {
        em.OnRoomEntered -= OnRoomEntered;
        em.OnDoorEntered -= OnDoorEntered;
    }

    private void OnRoomEntered(int roomSecurityLevel, bool isAllowed)
    {
        if (isAllowed)
        {
            return;
        }

        int valueOfIncrease = roomEnterIncrease * roomSecurityLevel;
        IncreaseSecurityPoints(valueOfIncrease);
    }

    private void OnDoorEntered(int doorSecurityLevel, bool isAllowed)
    {

        if (isAllowed)
        {
            return;
        }
        
        int valueOfIncrease = doorEnterIncrease * doorSecurityLevel;
        IncreaseSecurityPoints(valueOfIncrease);
    }

    private void IncreaseSecurityPoints(int valueOfIncrease)
    {
        securityPoints += valueOfIncrease;

        if (securityPoints >= maximumOfSecurityPoint)
        {
            securityPoints = maximumOfSecurityPoint;
        }

        int previousSecurityTier = securityTier;
        securityTier = securityPoints / pointPerTier;

        if (securityTier > previousSecurityTier)
        {
            StartAlarm();
        }

        Debug.Log("SecurityPoints increased! SecurityPoints: " + securityPoints + ", Security tier: " + securityTier);
    }

    private void StartAlarm()
    {
        em.BroadcastStartAlarm();
        Debug.Log("Start alarm");
    }
}
