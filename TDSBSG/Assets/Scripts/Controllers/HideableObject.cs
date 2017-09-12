using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideableObject : MonoBehaviour
{
    Toolbox toolbox;
    EventManager em;
    Renderer _renderer = null;

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
        _renderer = GetComponent<Renderer>();

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
            originalColor = _renderer.material.color;
        }

        hideRecoveryTimer = hideRecoveryDuration;
    }

    public void HideObject()
    {
        if (!isHidden)
        {
            isHidden = true;

            if (_renderer == null)
            {
                _renderer = GetComponent<Renderer>();
            }

            originalColor = _renderer.material.color;
            Color hiddenColor = originalColor;
            hiddenColor.a = hiddenColor.a * hiddenAlphaMultiplier;
            _renderer.material.color = hiddenColor;
        }
    }

    public void UnHideObject()
    {
        if (isHidden)
        {
            isHidden = false;

            if (_renderer == null)
            {
                _renderer = GetComponent<Renderer>();
            }
            
            _renderer.material.color = originalColor;
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
