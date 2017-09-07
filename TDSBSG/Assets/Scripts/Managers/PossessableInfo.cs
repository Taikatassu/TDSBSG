using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossessableInfo : MonoBehaviour
{
    Toolbox toolbox;
    EventManager em;
    public static PossessableInfo instance;
    public List<IPossessable> possessables = new List<IPossessable>();

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
        //ResetAll();
    }

    private void ResetAll()
    {
        Debug.Log("PossessableInfo: ResetAll");
        possessables = new List<IPossessable>();
    }

    private void RegisterPossessable(IPossessable newPossessable)
    {
        possessables.Add(newPossessable);
    }
}
