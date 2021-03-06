﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    Toolbox toolbox;
    EventManager em;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);


        toolbox = FindObjectOfType<Toolbox>();
        em = toolbox.GetComponent<EventManager>();
    }

    void Start()
    {
        em.BroadcastGameStarted();
    }
    
}
