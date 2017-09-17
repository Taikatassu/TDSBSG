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
    float topAngleMin = 60;

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
    [SerializeField]
    GameObject topFrontIdleWalkSpritePlayer;
    [SerializeField]
    GameObject topBackIdleWalkSpritePlayer;

    List<GameObject> spritePlayers = new List<GameObject>();
    [SerializeField]
    float takeDamageDuration = 0.5f;
    float takeDamageTimer = 0f;
    bool top = false;


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

        spritePlayers = new List<GameObject>
        {
            frontIdleSpritePlayer,
            frontWalkSpritePlayer,
            backIdleSpritePlayer,
            backWalkSpritePlayer,

            rightIdleSpritePlayer,
            rightWalkSpritePlayer,
            leftIdleSpritePlayer,
            leftWalkSpritePlayer,

            takeDamageSpritePlayer,
            knockedOutSpritePlayer,
            topFrontIdleWalkSpritePlayer,
            topBackIdleWalkSpritePlayer
        };
    }

    public void StartKnockout()
    {
        takeDamageTimer = takeDamageDuration;
        SetAnimationState(EAnimationState.TAKEDAMAGE);
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
        enabledSpritePlayer = newSpritePlayer;
        int count = spritePlayers.Count;
        for (int i = 0; i < count; i++)
        {
            if (spritePlayers[i] != null)
            {
                if (enabledSpritePlayer == spritePlayers[i])
                {
                    spritePlayers[i].SetActive(true);
                }
                else
                {
                    spritePlayers[i].SetActive(false);
                }
            }
        }
    }

    private void UpdateSpriteState()
    {
        if (currentViewDirection == EViewDirection.TOP)
        {
            SetEnabledSpritePlayer(topFrontIdleWalkSpritePlayer);
        }

        else if (currentAnimState == EAnimationState.TAKEDAMAGE)
        {
            if (top)
            {
                //TODO: Change this to top view take damage sprite once they are ready
                SetEnabledSpritePlayer(takeDamageSpritePlayer);
            }
            else
            {
                SetEnabledSpritePlayer(takeDamageSpritePlayer);
            }
        }
        else if (currentAnimState == EAnimationState.KNOCKEDOUT)
        {
            if (top)
            {
                //TODO: Change this to top view knocked out sprite once they are ready
                SetEnabledSpritePlayer(knockedOutSpritePlayer);
            }
            else
            {
                SetEnabledSpritePlayer(knockedOutSpritePlayer);
            }
        }
        else
        {
            switch (currentViewDirection)
            {
                case EViewDirection.DEFAULT:
                    break;
                case EViewDirection.FRONT:
                    if (top)
                    {
                        SetEnabledSpritePlayer(topFrontIdleWalkSpritePlayer);
                    }
                    else
                    {
                        switch (currentAnimState)
                        {
                            case EAnimationState.IDLE:
                                SetEnabledSpritePlayer(frontIdleSpritePlayer);
                                break;
                            case EAnimationState.WALK:
                                SetEnabledSpritePlayer(frontWalkSpritePlayer);
                                break;
                        }
                    }

                    break;
                case EViewDirection.BACK:
                    if (top)
                    {
                        SetEnabledSpritePlayer(topBackIdleWalkSpritePlayer);
                    }
                    else
                    {
                        switch (currentAnimState)
                        {
                            case EAnimationState.IDLE:
                                SetEnabledSpritePlayer(backIdleSpritePlayer);
                                break;
                            case EAnimationState.WALK:
                                SetEnabledSpritePlayer(backWalkSpritePlayer);
                                break;
                        }
                    }
                    break;
                case EViewDirection.RIGHT:
                    if (top)
                    {
                        SetEnabledSpritePlayer(topFrontIdleWalkSpritePlayer);
                    }
                    else
                    {
                        switch (currentAnimState)
                        {
                            case EAnimationState.IDLE:
                                SetEnabledSpritePlayer(rightIdleSpritePlayer);
                                break;
                            case EAnimationState.WALK:
                                SetEnabledSpritePlayer(rightWalkSpritePlayer);
                                break;
                        }
                    }
                    break;
                case EViewDirection.LEFT:
                    if (top)
                    {
                        SetEnabledSpritePlayer(topFrontIdleWalkSpritePlayer);
                    }
                    else
                    {
                        switch (currentAnimState)
                        {
                            case EAnimationState.IDLE:
                                SetEnabledSpritePlayer(leftIdleSpritePlayer);
                                break;
                            case EAnimationState.WALK:
                                SetEnabledSpritePlayer(leftWalkSpritePlayer);
                                break;
                        }
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
            Vector3 cameraDirectionHorizontal = cameraDirection;
            cameraDirectionHorizontal.y = 0;
            float cameraAngleHorizontal = Vector3.SignedAngle(transform.forward, cameraDirectionHorizontal, Vector3.up);
            
            Vector3 cameraDirectionVertical = cameraDirection;
            cameraDirectionVertical.z = new Vector2(cameraDirectionVertical.x, cameraDirectionVertical.z).magnitude;
            cameraDirectionVertical.x = 0;
            float cameraAngleVertical = Mathf.Abs(Vector3.SignedAngle(Vector3.forward, cameraDirectionVertical, Vector3.right));

            if (cameraAngleVertical >= topAngleMin)
            {
                top = true;
            }
            else
            {
                top = false;
            }


            //Front
            if (cameraAngleHorizontal >= frontAngleMinMax.x && cameraAngleHorizontal < frontAngleMinMax.y)
            {
                SetViewDirection(EViewDirection.FRONT);
            }
            //Back
            else if (cameraAngleHorizontal >= backAngleMinMax.y || cameraAngleHorizontal < backAngleMinMax.x)
            {
                SetViewDirection(EViewDirection.BACK);
            }
            //Right
            else if (cameraAngleHorizontal >= rightAngleMinMax.x && cameraAngleHorizontal < rightAngleMinMax.y)
            {
                SetViewDirection(EViewDirection.RIGHT);
            }
            //Left
            else if (cameraAngleHorizontal >= leftAngleMinMax.x && cameraAngleHorizontal < leftAngleMinMax.y)
            {
                SetViewDirection(EViewDirection.LEFT);
            }
            else
            {
                Debug.LogWarning(gameObject.name + "This should not be happening (cameraAngle: " + cameraAngleHorizontal + ")");
            }
        }

        if (currentAnimState == EAnimationState.TAKEDAMAGE)
        {
            takeDamageTimer -= Time.fixedDeltaTime;

            if (takeDamageTimer <= 0)
            {
                SetAnimationState(EAnimationState.KNOCKEDOUT);
            }
        }
        UpdateSpriteState();
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
    TOP,

}


