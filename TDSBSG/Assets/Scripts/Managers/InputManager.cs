using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    #region References & variables
    public static InputManager _instance;
    Toolbox toolbox;
    EventManager em;

    private KeyCode moveUpKey;
    private KeyCode moveDownKey;
    private KeyCode moveRightKey;
    private KeyCode moveLeftKey;
    private KeyCode pauseKey;
    private KeyCode useKey;
    private KeyCode rotateCameraClockwise;
    private KeyCode rotateCameraCounterClockwise;
    #endregion

    void Awake()
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
    }

    void Start()
    {
        //Initialize input keys
        moveUpKey = KeyCode.W;
        moveDownKey = KeyCode.S;
        moveRightKey = KeyCode.D;
        moveLeftKey = KeyCode.A;
        pauseKey = KeyCode.Escape;
        useKey = KeyCode.Space;
        rotateCameraClockwise = KeyCode.Q;
        rotateCameraCounterClockwise = KeyCode.E;
    }

    void Update()
    {
        #region Keyboard input
        //Movement
        //Up
        if (Input.GetKeyDown(moveUpKey))
        {
            em.BroadcastInputEvent(EInputType.MOVEUP_KEYDOWN);
        }
        if (Input.GetKeyUp(moveUpKey))
        {
            em.BroadcastInputEvent(EInputType.MOVEUP_KEYUP);
        }
        //Down
        if (Input.GetKeyDown(moveDownKey))
        {
            em.BroadcastInputEvent(EInputType.MOVEDOWN_KEYDOWN);
        }
        if (Input.GetKeyUp(moveDownKey))
        {
            em.BroadcastInputEvent(EInputType.MOVEDOWN_KEYUP);
        }
        //Right
        if (Input.GetKeyDown(moveRightKey))
        {
            em.BroadcastInputEvent(EInputType.MOVERIGHT_KEYDOWN);
        }
        if (Input.GetKeyUp(moveRightKey))
        {
            em.BroadcastInputEvent(EInputType.MOVERIGHT_KEYUP);
        }
        //Left
        if (Input.GetKeyDown(moveLeftKey))
        {
            em.BroadcastInputEvent(EInputType.MOVELEFT_KEYDOWN);
        }
        if (Input.GetKeyUp(moveLeftKey))
        {
            em.BroadcastInputEvent(EInputType.MOVELEFT_KEYUP);
        }

        //Pause
        if (Input.GetKeyDown(pauseKey))
        {
            em.BroadcastInputEvent(EInputType.PAUSE_KEYDOWN);
        }
        if (Input.GetKeyUp(pauseKey))
        {
            em.BroadcastInputEvent(EInputType.PAUSE_KEYUP);
        }

        //Use (possessable ability)
        if (Input.GetKeyDown(useKey))
        {
            em.BroadcastInputEvent(EInputType.USE_KEYDOWN);
        }
        if (Input.GetKeyUp(useKey))
        {
            em.BroadcastInputEvent(EInputType.USE_KEYUP);
        }
        #endregion

        #region Mouse input
        //Mouse input
        if (Input.GetMouseButtonDown(0))
        {
            em.BroadcastMouseInputEvent(0, true, Input.mousePosition);
        }
        if (Input.GetMouseButtonUp(0))
        {
            em.BroadcastMouseInputEvent(0, false, Input.mousePosition);
        }

        if (Input.GetMouseButtonDown(1))
        {
            em.BroadcastMouseInputEvent(1, true, Input.mousePosition);
        }
        if (Input.GetMouseButtonUp(1))
        {
            em.BroadcastMouseInputEvent(1, false, Input.mousePosition);
        }

        if (Input.GetMouseButtonDown(2))
        {
            em.BroadcastMouseInputEvent(2, true, Input.mousePosition);
        }
        if (Input.GetMouseButtonUp(2))
        {
            em.BroadcastMouseInputEvent(2, false, Input.mousePosition);
        }

        //Broadcast mouse position
        em.BroadcastMousePositionChange(Input.mousePosition);

        //Rotate camera
        if (Input.GetKeyDown(rotateCameraClockwise))
        {
            em.BroadcastInputEvent(EInputType.ROTATECAMERACLOCKWISE_KEYDOWN);
        }
        if (Input.GetKeyUp(rotateCameraClockwise))
        {
            em.BroadcastInputEvent(EInputType.ROTATECAMERACLOCKWISE_KEYUP);
        }

        if (Input.GetKeyDown(rotateCameraCounterClockwise))
        {
            em.BroadcastInputEvent(EInputType.ROTATECAMERACOUNTERCLOCKWISE_KEYDOWN);
        }
        if (Input.GetKeyUp(rotateCameraCounterClockwise))
        {
            em.BroadcastInputEvent(EInputType.ROTATECAMERACOUNTERCLOCKWISE_KEYUP);
        }
        #endregion
    }
}

public enum EInputType
{
    MOVEUP_KEYDOWN,
    MOVEUP_KEYUP,
    MOVEDOWN_KEYDOWN,
    MOVEDOWN_KEYUP,
    MOVERIGHT_KEYDOWN,
    MOVERIGHT_KEYUP,
    MOVELEFT_KEYDOWN,
    MOVELEFT_KEYUP,
    PAUSE_KEYDOWN,
    PAUSE_KEYUP,
    USE_KEYDOWN,
    USE_KEYUP,
    ROTATECAMERACLOCKWISE_KEYDOWN,
    ROTATECAMERACLOCKWISE_KEYUP,
    ROTATECAMERACOUNTERCLOCKWISE_KEYDOWN,
    ROTATECAMERACOUNTERCLOCKWISE_KEYUP,

}
