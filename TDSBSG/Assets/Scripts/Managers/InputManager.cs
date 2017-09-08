using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    #region References & variables
    Toolbox toolbox;
    EventManager em;

    private KeyCode moveUpKey;
    private KeyCode moveDownKey;
    private KeyCode moveRightKey;
    private KeyCode moveLeftKey;
    private KeyCode possessKey;
    private KeyCode pauseKey;
    #endregion

    void Awake()
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
        possessKey = KeyCode.Q;
        pauseKey = KeyCode.Escape;

    }

    void Update()
    {
        #region Reading inputs and broadcasting relevant events
        //Movement
        //Up
        if (Input.GetKeyDown(moveUpKey))
        {
            em.BroadcastInputEvent(EInputType.MOVEUP_KEYDOWN);
        }
        //if (Input.GetKey(moveUpKey))
        //{
        //    em.BroadcastInputEvent(EInputType.MOVEUP_KEY);
        //}
        if (Input.GetKeyUp(moveUpKey))
        {
            em.BroadcastInputEvent(EInputType.MOVEUP_KEYUP);
        }
        //Down
        if (Input.GetKeyDown(moveDownKey))
        {
            em.BroadcastInputEvent(EInputType.MOVEDOWN_KEYDOWN);
        }
        //if (Input.GetKey(moveUpKey))
        //{
        //    em.BroadcastInputEvent(EInputType.MOVEDOWN_KEY);
        //}
        if (Input.GetKeyUp(moveDownKey))
        {
            em.BroadcastInputEvent(EInputType.MOVEDOWN_KEYUP);
        }
        //Right
        if (Input.GetKeyDown(moveRightKey))
        {
            em.BroadcastInputEvent(EInputType.MOVERIGHT_KEYDOWN);
        }
        //if (Input.GetKey(moveRightKey))
        //{
        //    em.BroadcastInputEvent(EInputType.MOVERIGHT_KEY);
        //}
        if (Input.GetKeyUp(moveRightKey))
        {
            em.BroadcastInputEvent(EInputType.MOVERIGHT_KEYUP);
        }
        //Left
        if (Input.GetKeyDown(moveLeftKey))
        {
            em.BroadcastInputEvent(EInputType.MOVELEFT_KEYDOWN);
        }
        //if (Input.GetKey(moveLeftKey))
        //{
        //    em.BroadcastInputEvent(EInputType.MOVELEFT_KEY);
        //}
        if (Input.GetKeyUp(moveLeftKey))
        {
            em.BroadcastInputEvent(EInputType.MOVELEFT_KEYUP);
        }

        //Other inputs
        if (Input.GetKeyDown(possessKey))
        {
            em.BroadcastInputEvent(EInputType.POSSESS_KEYDOWN);
        }
        //if (Input.GetKey(possessKey))
        //{
        //    em.BroadcastInputEvent(EInputType.POSSESS_KEY);
        //}
        if (Input.GetKeyUp(possessKey))
        {
            em.BroadcastInputEvent(EInputType.POSSESS_KEYUP);
        }
        #endregion

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

        if (Input.GetKeyDown(pauseKey))
        {
            em.BroadcastInputEvent(EInputType.PAUSE_KEYDOWN);
        }
        if (Input.GetKeyUp(pauseKey))
        {
            em.BroadcastInputEvent(EInputType.PAUSE_KEYUP);
        }
    }
}

public enum EInputType
{
    MOVEUP_KEYDOWN,
    MOVEDOWN_KEYDOWN,
    MOVERIGHT_KEYDOWN,
    MOVELEFT_KEYDOWN,
    POSSESS_KEYDOWN,
    //MOVEUP_KEY,
    //MOVEDOWN_KEY,
    //MOVERIGHT_KEY,
    //MOVELEFT_KEY,
    //POSSESS_KEY,
    MOVEUP_KEYUP,
    MOVEDOWN_KEYUP,
    MOVERIGHT_KEYUP,
    MOVELEFT_KEYUP,
    POSSESS_KEYUP,
    PAUSE_KEYDOWN,
    PAUSE_KEYUP,

}
