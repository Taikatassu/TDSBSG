using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideableObject : MonoBehaviour
{
    Toolbox toolbox;
    EventManager em;
    Renderer renderer = null;

    [SerializeField]
    float hiddenAlphaMultiplier = 0.5f;
    [SerializeField]
    float hideRecoveryDuration = 0.5f;
    float hideRecoveryTimer = 0f;
    bool isHidden = false;
    bool useTimer = false;
    Color originalColor;

    private void Awake()
    {
        toolbox = FindObjectOfType<Toolbox>();
        em = toolbox.GetComponent<EventManager>();
    }

    private void OnEnable()
    {
        renderer = GetComponent<Renderer>();

        em.OnHideEnvironmentStateChange += OnHideEnvironmentStateChange;
    }

    private void OnDisable()
    {
        em.OnHideEnvironmentStateChange -= OnHideEnvironmentStateChange;
    }

    private void OnHideEnvironmentStateChange(bool newState)
    {
        if (newState)
        {
            HideObject();
        }
        else
        {
            UnHideObject();
        }
    }

    public void HideObjectWithTimer()
    {
        if (!isHidden)
        {
            isHidden = true;
            useTimer = true;
            originalColor = renderer.material.color;
        }

        hideRecoveryTimer = hideRecoveryDuration;
    }

    public void HideObject()
    {
        if (!isHidden)
        {
            isHidden = true;

            if (renderer == null)
            {
                renderer = GetComponent<Renderer>();
            }

            originalColor = renderer.material.color;
            Color hiddenColor = originalColor;
            hiddenColor.a = 0; //hiddenColor.a * hiddenAlphaMultiplier;
            renderer.material.color = hiddenColor;
        }
    }

    public void UnHideObject()
    {
        if (isHidden)
        {
            isHidden = false;

            if (renderer == null)
            {
                renderer = GetComponent<Renderer>();
            }
            
            renderer.material.color = originalColor;
        }
    }

    private void FixedUpdate()
    {
        if (isHidden && useTimer)
        {
            hideRecoveryTimer -= Time.fixedDeltaTime;

            if (hideRecoveryTimer <= 0)
            {
                UnHideObject();
            }
        }
    }

}
