using System.Collections;
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
    public delegate void BoolVoid(bool boolean);
    public delegate void IntVoid(int integer);
    public delegate void FloatVoid(float floatingPoint);
    public delegate void StringVoid(string str);
    public delegate void Vector3Void(Vector3 vec3);
    public delegate void InputVoid(EInputType inputType);
    public delegate void RobotTypeVoid(ERobotType eRobotType);
    public delegate void SoundEffectVoid(SoundEffect soundEffect);
    public delegate void IntRobotTypeVoid(int integer, ERobotType eRobotType);
    public delegate void IntBoolVector3Void(int integer, bool boolean, Vector3 vec3);
    public delegate int EmptyInt();
    public delegate float EmptyFloat();
    public delegate Vector3 EmptyVector3();
    public delegate ERobotType EmptyRobotType();
    public delegate GameObject EmptyGameObject();

    public event EmptyVoid OnInitializeGame;
    public void BroadcastInitializeGame()
    {
        if (OnInitializeGame != null)
        {
            OnInitializeGame();
        }
    }

    public event EmptyVoid OnStartGame;
    public void BroadcastStartGame()
    {
        if (OnStartGame != null)
        {
            OnStartGame();
        }
    }

    public event IntBoolVector3Void OnMouseInputEvent;
    public void BroadcastMouseInputEvent(int button, bool down, Vector3 mousePosition)
    {
        if (OnMouseInputEvent != null)
        {
            OnMouseInputEvent(button, down, mousePosition);
        }
    }

    public event Vector3Void OnMousePositionChange;
    public void BroadcastMousePositionChange(Vector3 newPosition)
    {
        if (OnMousePositionChange != null)
        {
            OnMousePositionChange(newPosition);
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

    public event EmptyGameObject OnRequestCameraReference;
    public GameObject BroadcastRequestCameraReference()
    {
        if (OnRequestCameraReference != null)
        {
            return OnRequestCameraReference();
        }
        else
        {
            return FindObjectOfType<Player>().gameObject;
        }
    }

    public event EmptyVector3 OnRequestSceneIndices;
    public Vector3 BroadcastRequestSceneIndices()
    {
        if (OnRequestSceneIndices != null)
        {
            return OnRequestSceneIndices();
        }
        else
        {
            return Vector3.zero;
        }
    }

    public event IntVoid OnRequestLoadLevel;
    public void BroadcastRequestLoadLevel(int desiredSceneBuildIndex)
    {
        if (OnRequestLoadLevel != null)
        {
            OnRequestLoadLevel(desiredSceneBuildIndex);
        }
    }

    public event BoolVoid OnLoadingScreenStateChange;
    public void BroadcastLoadingScreenStateChange(bool isOpen)
    {
        if (OnLoadingScreenStateChange != null)
        {
            OnLoadingScreenStateChange(isOpen);
        }
    }

    public event EmptyInt OnRequestCurrentSceneIndex;
    public int BroadcastRequestCurrentSceneIndex()
    {
        if (OnRequestCurrentSceneIndex != null)
        {
            return OnRequestCurrentSceneIndex();
        }

        return -1;
    }

    public event BoolVoid OnPauseStateChange;
    public void BroadcastPauseStateChange(bool newPauseState)
    {
        if (OnPauseStateChange != null)
        {
            OnPauseStateChange(newPauseState);
        }
    }

    public event BoolVoid OnRequestPauseStateChange;
    public void BroadcastRequestPauseStateChange(bool newPauseState)
    {
        if (OnRequestPauseStateChange != null)
        {
            OnRequestPauseStateChange(newPauseState);
        }
    }

    public event EmptyVoid OnRequestExitApplication;
    public void BroadcastRequestExitApplication()
    {
        if (OnRequestExitApplication != null)
        {
            OnRequestExitApplication();
        }
    }

    public event BoolVoid OnPauseActorsStateChange;
    public void BroadcastPauseActorsStateChange(bool newState)
    {
        if (OnPauseActorsStateChange != null)
        {
            OnPauseActorsStateChange(newState);
        }
    }

    public event IntRobotTypeVoid OnLevelCompleted;
    public void BroadcastLevelCompleted(int sceneIndex, ERobotType lastPossessedRobotType)
    {
        if (OnLevelCompleted != null)
        {
            OnLevelCompleted(sceneIndex, lastPossessedRobotType);
        }
    }

    public event RobotTypeVoid OnSpawnPlayer;
    public void BroadcastSpawnPlayer(ERobotType robotTypeToSpawnAs)
    {
        if (OnSpawnPlayer != null)
        {
            OnSpawnPlayer(robotTypeToSpawnAs);
        }
    }

    public event EmptyVoid OnPlayerCatched;
    public void BroadcastPlayerCatched()
    {
        Debug.Log("BroadcastPlayerCatched");
        if (OnPlayerCatched != null)
        {
            OnPlayerCatched();
        }
    }

    public event EmptyRobotType OnRequestSpawningRobotType;
    public ERobotType BroadcastRequestSpawningRobotType()
    {
        if (OnRequestSpawningRobotType != null)
        {
            return OnRequestSpawningRobotType();
        }
        else
        {
            return ERobotType.NONE;
        }
    }

    public event StringVoid OnRequestAudio;
    public void BroadcastRequestAudio(string audioName)
    {
        if (OnRequestAudio != null)
        {
            OnRequestAudio(audioName);
        }
    }

    public event SoundEffectVoid OnRegisterSoundEffect;
    public void BroadcastRegisterSoundEffect(SoundEffect newSoundEffect)
    {
        if (OnRegisterSoundEffect != null)
        {
            OnRegisterSoundEffect(newSoundEffect);
        }
    }

    public event BoolVoid OnPossessablePossessed;
    public void BroadcastPossessablePossessed(bool stationary)
    {
        if (OnPossessablePossessed != null)
        {
            OnPossessablePossessed(stationary);
        }
    }

    public event FloatVoid OnVolumeSliderValueChange;
    public void BroadcastVolumeSliderValueChange(float newValue)
    {
        if (OnVolumeSliderValueChange != null)
        {
            OnVolumeSliderValueChange(newValue);
        }
    }
    
    public event EmptyFloat OnRequestVolumeLevel;
    public float BroadcastRequestVolumeLevel()
    {
        if (OnRequestVolumeLevel != null)
        {
            return OnRequestVolumeLevel();
        }
        else
        {
            return -1f;
        }
    }

}
