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

    //[SerializeField]
    //float takeDamageDuration = 0.5f;
    //float takeDamageTimer = 0f;

    EAnimationState currentAnimState = EAnimationState.DEFAULT;
    EViewDirection currentViewDirection = EViewDirection.DEFAULT;

    public void Awake()
    {
        toolbox = FindObjectOfType<Toolbox>();
        em = toolbox.GetComponent<EventManager>();
    }

    private void OnEnable()
    {
        em.OnInitializeGame += OnInitializeGame;
    }

    private void OnDisable()
    {
        em.OnInitializeGame -= OnInitializeGame;
    }

    private void OnInitializeGame()
    {
        cameraTransform = em.BroadcastRequestCameraReference().transform;
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

        //Front
        if (enabledSpritePlayer == frontIdleSpritePlayer)
        {
            frontIdleSpritePlayer.SetActive(true);
        }
        else
        {
            frontIdleSpritePlayer.SetActive(false);
        }

        if (enabledSpritePlayer == frontWalkSpritePlayer)
        {
            frontWalkSpritePlayer.SetActive(true);
        }
        else
        {
            frontWalkSpritePlayer.SetActive(false);
        }

        //Back
        if (enabledSpritePlayer == backIdleSpritePlayer)
        {
            backIdleSpritePlayer.SetActive(true);
        }
        else
        {
            backIdleSpritePlayer.SetActive(false);
        }

        if (enabledSpritePlayer == backWalkSpritePlayer)
        {
            backWalkSpritePlayer.SetActive(true);
        }
        else
        {
            backWalkSpritePlayer.SetActive(false);
        }

        //Right
        if (enabledSpritePlayer == rightIdleSpritePlayer)
        {
            rightIdleSpritePlayer.SetActive(true);
        }
        else
        {
            rightIdleSpritePlayer.SetActive(false);
        }

        if (enabledSpritePlayer == rightWalkSpritePlayer)
        {
            rightWalkSpritePlayer.SetActive(true);
        }
        else
        {
            rightWalkSpritePlayer.SetActive(false);
        }

        //Left
        if (enabledSpritePlayer == leftIdleSpritePlayer)
        {
            leftIdleSpritePlayer.SetActive(true);
        }
        else
        {
            leftIdleSpritePlayer.SetActive(false);
        }

        if (enabledSpritePlayer == leftWalkSpritePlayer)
        {
            leftWalkSpritePlayer.SetActive(true);
        }
        else
        {
            leftWalkSpritePlayer.SetActive(false);
        }

        //Take damage
        if (enabledSpritePlayer == takeDamageSpritePlayer)
        {
            takeDamageSpritePlayer.SetActive(true);
        }
        else
        {
            takeDamageSpritePlayer.SetActive(false);
        }

        //Knockedout
        if (enabledSpritePlayer == knockedOutSpritePlayer)
        {
            knockedOutSpritePlayer.SetActive(true);
        }
        else
        {
            knockedOutSpritePlayer.SetActive(false);
        }
    }

    private void UpdateSpriteState()
    {
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
            if (cameraAngle >= -45 && cameraAngle < 45)
            {
                SetViewDirection(EViewDirection.FRONT);
            }
            //Right
            else if (cameraAngle >= 45 && cameraAngle < 135)
            {
                SetViewDirection(EViewDirection.RIGHT);
            }
            //Back
            else if (cameraAngle >= 135 || cameraAngle < -135)
            {
                SetViewDirection(EViewDirection.BACK);
            }
            //Left
            else if (cameraAngle >= -135 && cameraAngle < -45)
            {
                SetViewDirection(EViewDirection.LEFT);
            }
            else
            {
                Debug.LogWarning("This should not be happening (cameraAngle: " + cameraAngle + ")");
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


