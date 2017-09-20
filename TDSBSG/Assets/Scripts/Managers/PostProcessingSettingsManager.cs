using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class PostProcessingSettingsManager : MonoBehaviour
{
    Toolbox toolbox;
    EventManager em;
    [SerializeField]
    PostProcessingProfile profile;

    bool antiAliasing = false;
    bool ambientOcclusion = false;

    private void OnEnable()
    {
        toolbox = FindObjectOfType<Toolbox>();
        em = toolbox.GetComponent<EventManager>();

        em.OnPostProcessingSettingChange += OnPostProcessingSettingChange;
        em.OnRequestPostProcessingSettingState += OnRequestPostProcessingSettingState;

        antiAliasing = profile.antialiasing.enabled;
        ambientOcclusion = profile.ambientOcclusion.enabled;
    }

    private void OnDisable()
    {
        em.OnPostProcessingSettingChange -= OnPostProcessingSettingChange;
        em.OnRequestPostProcessingSettingState -= OnRequestPostProcessingSettingState;
    }

    private void OnPostProcessingSettingChange(EPostProcessingSetting settingToChange, bool newState)
    {
        switch (settingToChange)
        {
            case EPostProcessingSetting.ANTIALIASING:
                if (newState != antiAliasing)
                {
                    antiAliasing = newState;
                    profile.antialiasing.enabled = antiAliasing;
                }
                break;
            case EPostProcessingSetting.AMBIENTOCCLUSION:
                if (newState != ambientOcclusion)
                {
                    ambientOcclusion = newState;
                    profile.ambientOcclusion.enabled = ambientOcclusion;
                }
                break;
            default:
                break;
        }
    }

    private bool OnRequestPostProcessingSettingState(EPostProcessingSetting settingToGetTheStateOf)
    {
        switch (settingToGetTheStateOf)
        {
            case EPostProcessingSetting.ANTIALIASING:
                return profile.antialiasing.enabled;
            case EPostProcessingSetting.AMBIENTOCCLUSION:
                return profile.ambientOcclusion.enabled;
        }

        Debug.LogWarning("No case set for " + settingToGetTheStateOf + ", returning false!");
        return false;
    }
}

public enum EPostProcessingSetting
{
    ANTIALIASING,
    AMBIENTOCCLUSION,

}
