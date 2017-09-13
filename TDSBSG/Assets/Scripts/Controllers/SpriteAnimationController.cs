using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimationController : MonoBehaviour
{
    Toolbox toolbox;
    EventManager em;
    Transform cameraTransform = null;
    GameObject enabledSpritePlayer = null;

    [SerializeField]
    Vector2 frontAngleMinMax = new Vector2(-45, 45);
    [SerializeField]
    Vector2 backAngleMinMax = new Vector2(-135, 135);
    [SerializeField]
    Vector2 rightAngleMinMax = new Vector2(45, 135);
    [SerializeField]
    Vector2 leftAngleMinMax = new Vector2(-135, -45);

    [SerializeField]
    GameObject frontIdleSpritePlayer;
    [SerializeField]
    GameObject frontWalkSpritePlayer;
    [SerializeField]
    GameObject backIdleSpritePlayer;
    [SerializeField]
    GameObject backWalkSpritePlayer;
    [SerializeField]
    GameObject rightIdleSpritePlayer;
    [SerializeField]
    GameObject rightWalkSpritePlayer;
    [SerializeField]
    GameObject leftIdleSpritePlayer;
    [SerializeField]
    GameObject leftWalkSpritePlayer;
    [SerializeField]
    GameObject takeDamageSpritePlayer;
    [SerializeField]
    GameObject knockedOutSpritePlayer;

    List<GameObject> spritePlayers = new List<GameObject>();
    //[SerializeField]
    //float takeDamageDuration = 0.5f;
    //float takeDamageTimer = 0f;

    EAnimationState currentAnimState = EAnimationState.IDLE;
    EViewDirection currentViewDirection = EViewDirection.DEFAULT;

    public void Awake()
    {
        toolbox = FindObjectOfType<Toolbox>();
        em = toolbox.GetComponent<EventManager>();
    }

    private void OnEnable()
    {
        em.OnStartGame += OnStartGame;
    }

    private void OnDisable()
    {
        em.OnStartGame -= OnStartGame;
    }

    private void OnStartGame()
    {
        cameraTransform = em.BroadcastRequestCameraReference().transform;

        spritePlayers = new List<GameObject>();
        spritePlayers.Add(frontIdleSpritePlayer);
        spritePlayers.Add(frontWalkSpritePlayer);
        spritePlayers.Add(backIdleSpritePlayer);
        spritePlayers.Add(backWalkSpritePlayer);

        spritePlayers.Add(rightIdleSpritePlayer);
        spritePlayers.Add(rightWalkSpritePlayer);
        spritePlayers.Add(leftIdleSpritePlayer);
        spritePlayers.Add(leftWalkSpritePlayer);

        spritePlayers.Add(takeDamageSpritePlayer);
        spritePlayers.Add(knockedOutSpritePlayer);
    }

    public void SetAnimationState(EAnimationState newState)
    {
        if (currentAnimState != newState)
        {
            currentAnimState = newState;

            UpdateSpriteState();
        }
    }

    private void SetViewDirection(EViewDirection newViewDirection)
    {
        if (currentViewDirection != newViewDirection)
        {
            currentViewDirection = newViewDirection;

            UpdateSpriteState();
        }
    }

    private void SetEnabledSpritePlayer(GameObject newSpritePlayer)
    {
        //Debug.Log("SetEnabledSpritePlayer, newSpritePlayer: " + newSpritePlayer);
        enabledSpritePlayer = newSpritePlayer;
        int count = spritePlayers.Count;
        Debug.Log(gameObject.name + ", SetEnabledSpritePlayer, count: " + count);
        for (int i = 0; i < count; i++)
        {
            if(spritePlayers[i] != null)
            {
                if (enabledSpritePlayer == spritePlayers[i])
                {
                    Debug.Log(gameObject.name + ", spritePlayers[i].SetActive(true), spritePlayers[i].name: " + spritePlayers[i]);
                    spritePlayers[i].SetActive(true);
                }
                else
                {
                    Debug.Log(gameObject.name + ", spritePlayers[i].SetActive(false), spritePlayers[i].name: " + spritePlayers[i]);
                    spritePlayers[i].SetActive(false);
                }
            }
        }
    }

    private void UpdateSpriteState()
    {
        //Debug.Log(gameObject.name + ", UpdateSpriteState, currentAnimState: " 
        //    + currentAnimState + ", currentViewDirection: " + currentViewDirection);
        if (currentAnimState == EAnimationState.TAKEDAMAGE)
        {
            SetEnabledSpritePlayer(takeDamageSpritePlayer);
        }
        else if (currentAnimState == EAnimationState.KNOCKEDOUT)
        {
            SetEnabledSpritePlayer(knockedOutSpritePlayer);
        }
        else
        {
            switch (currentViewDirection)
            {
                case EViewDirection.DEFAULT:
                    break;
                case EViewDirection.FRONT:
                    switch (currentAnimState)
                    {
                        case EAnimationState.IDLE:
                            SetEnabledSpritePlayer(frontIdleSpritePlayer);
                            break;
                        case EAnimationState.WALK:
                            SetEnabledSpritePlayer(frontWalkSpritePlayer);
                            break;
                    }
                    break;
                case EViewDirection.BACK:
                    switch (currentAnimState)
                    {
                        case EAnimationState.IDLE:
                            SetEnabledSpritePlayer(backIdleSpritePlayer);
                            break;
                        case EAnimationState.WALK:
                            SetEnabledSpritePlayer(backWalkSpritePlayer);
                            break;
                    }
                    break;
                case EViewDirection.RIGHT:
                    switch (currentAnimState)
                    {
                        case EAnimationState.IDLE:
                            SetEnabledSpritePlayer(rightIdleSpritePlayer);
                            break;
                        case EAnimationState.WALK:
                            SetEnabledSpritePlayer(rightWalkSpritePlayer);
                            break;
                    }
                    break;
                case EViewDirection.LEFT:
                    switch (currentAnimState)
                    {
                        case EAnimationState.IDLE:
                            SetEnabledSpritePlayer(leftIdleSpritePlayer);
                            break;
                        case EAnimationState.WALK:
                            SetEnabledSpritePlayer(leftWalkSpritePlayer);
                            break;
                    }
                    break;
            }
        }
    }

    private void FixedUpdate()
    {
        if (cameraTransform == null)
        {
            cameraTransform = em.BroadcastRequestCameraReference().transform;
        }

        if (cameraTransform != null)
        {
            Vector3 cameraDirection = cameraTransform.position - transform.position;
            cameraDirection.y = 0;
            float cameraAngle = Vector3.SignedAngle(transform.forward, cameraDirection, Vector3.up);

            //Vector3 rayOrigin = transform.position;
            //rayOrigin.y++;
            //Debug.DrawRay(rayOrigin, cameraDirection, Color.red, 1f);
            //Debug.DrawRay(rayOrigin, transform.forward * 10, Color.blue, 1f);

            //Front
            if (cameraAngle >= frontAngleMinMax.x && cameraAngle < frontAngleMinMax.y)
            {
                SetViewDirection(EViewDirection.FRONT);
            }
            //Back
            else if (cameraAngle >= backAngleMinMax.y || cameraAngle < backAngleMinMax.x)
            {
                SetViewDirection(EViewDirection.BACK);
            }
            //Right
            else if (cameraAngle >= rightAngleMinMax.x && cameraAngle < rightAngleMinMax.y)
            {
                SetViewDirection(EViewDirection.RIGHT);
            }
            //Left
            else if (cameraAngle >= leftAngleMinMax.x && cameraAngle < leftAngleMinMax.y)
            {
                SetViewDirection(EViewDirection.LEFT);
            }
            else
            {
                Debug.LogWarning(gameObject.name + "This should not be happening (cameraAngle: " + cameraAngle + ")");
            }
        }
    }
}

public enum EAnimationState
{
    DEFAULT,
    IDLE,
    WALK,
    TAKEDAMAGE,
    KNOCKEDOUT,

}

public enum EViewDirection
{
    DEFAULT,
    FRONT,
    BACK,
    RIGHT,
    LEFT,

}


