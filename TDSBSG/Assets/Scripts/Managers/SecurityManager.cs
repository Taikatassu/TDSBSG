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

    int alertState = 0; //0 = no alert, 1 = alerted (enemies have sight of player)
                        //, 2 = cautious (timer running for the ending of the alert)
    ERobotType wantedRobot = ERobotType.NONE; //The type of robot the enemies should be looking for
    ERobotType lastDisobeyingRobot = ERobotType.NONE; //The type of robot that last disobeyed the humans (passed restricted door)
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
    }

    private void OnEnable()
    {
        toolbox = FindObjectOfType<Toolbox>();
        em = toolbox.GetComponent<EventManager>();
        maximumOfSecurityPoint = numOfTiers * pointPerTier;

        em.OnRoomEntered += OnRoomEntered;
        em.OnDoorEntered += OnDoorEntered;
        em.OnDisobeyingDetected += OnDisobeyingDetected;
    }

    private void OnDisable()
    {
        em.OnRoomEntered -= OnRoomEntered;
        em.OnDoorEntered -= OnDoorEntered;
        em.OnDisobeyingDetected -= OnDisobeyingDetected;
    }

    private void OnDisobeyingDetected(ERobotType disobeyingRobotType, IPossessable disobeyingRobot)
    {
        wantedRobot = disobeyingRobotType;
        StartAlarm();
    }

    private void OnRoomEntered(int roomSecurityLevel, bool isAllowed, ERobotType enteringRobotType)
    {
        if (isAllowed)
        {
            return;
        }

        lastDisobeyingRobot = enteringRobotType;
        int valueOfIncrease = roomEnterIncrease * roomSecurityLevel;
        IncreaseSecurityPoints(valueOfIncrease);
    }

    private void OnDoorEntered(int doorSecurityLevel, bool isAllowed, ERobotType enteringRobotType)
    {

        if (isAllowed)
        {
            return;
        }

        lastDisobeyingRobot = enteringRobotType;
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
            em.BroadcastSecurityTierChange(securityTier);
            wantedRobot = lastDisobeyingRobot;
            StartAlarm();
        }
    }

    private void StartAlarm()
    {
        alertState = 1;
        em.BroadcastAlertStateChange(alertState, wantedRobot);
    }
}
