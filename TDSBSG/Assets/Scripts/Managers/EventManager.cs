﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager _instance;

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

    public delegate void EmptyVoid();
    public delegate void InputVoid(EInputType inputType);
    public delegate void IntBoolVoid(int integer, bool boolean);
    public delegate GameObject EmptyGameObject();

    public event EmptyVoid OnGameStarted;
    public void BroadcastGameStarted()
    {
        if (OnGameStarted != null)
        {
            OnGameStarted();
        }
    }

    public event InputVoid OnInputEvent;
    public void BroadcastInputEvent(EInputType newInput)
    {
        if (OnInputEvent != null)
        {
            OnInputEvent(newInput);
        }
    }

    public event EmptyGameObject OnRequestPlayerReference;
    public GameObject BroadcastRequestPlayerReference()
    {
        if (OnRequestPlayerReference != null)
        {
            return OnRequestPlayerReference();
        }
        else
        {
            return FindObjectOfType<Player>().gameObject;
        }
    }

    public event IntBoolVoid OnRoomEntered;
    public void BroadcastRoomEntered(int roomSecurityLevel, bool isAllowed)
    {
        if (OnRoomEntered != null)
        {
            OnRoomEntered(roomSecurityLevel, isAllowed);
        }
    }
}
